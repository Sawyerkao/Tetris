using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Tetris.Model;

namespace Tetris.Control
{
    public enum State
    {
        Beginning,
        Playing,
        Pause,
        Dead
    }

    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }

    public class GameHost : INotifyPropertyChanged
    {
        #region Constant
        private static readonly int[] FallingTimes = new int[] { 1000, 800, 600, 500, 400, 300, 250, 200, 150, 120, 100, 80, 70, 60, 50 };
        private static readonly int MaxLevel = FallingTimes.Length - 1;
        private static readonly Dictionary<Difficulty, int> LevelupDuration = new Dictionary<Difficulty, int>()
        {
            {Difficulty.Easy, 7 },
            {Difficulty.Normal, 6 },
            {Difficulty.Hard, 5 }
        };
        private static readonly Random random = new Random();
        #endregion

        #region Game States
        private int _Score = 0;
        public int Score
        {
            get { return _Score; }
            private set
            {
                _Score = value;
                OnPropertyChanged("Score");
            }
        }

        private int _Level = 0;
        public int Level
        {
            get { return _Level; }
            private set
            {
                if (value > MaxLevel)
                    _Level = MaxLevel;
                else
                    _Level = value;
                
                if (FallingTimer.IsEnabled)
                {
                    FallingTimer.Stop();
                    FallingTime = FallingTimes[_Level];
                    FallingTimer.Start();
                }
                else
                {
                    FallingTime = FallingTimes[_Level];
                }
                OnPropertyChanged("Level");
            }
        }

        private int _LevelupCounter = 0;
        private int LevelupCounter
        {
            get { return _LevelupCounter; }
            set
            {
                _LevelupCounter = value;
                LevelupProgress = value % LevelupDuration[Difficulty];
                OnPropertyChanged("LevelupCounter");
            }
        }

        private int _LevelupProgress;
        public int LevelupProgress
        {
            get { return _LevelupProgress; }
            private set
            {
                _LevelupProgress = value;
                OnPropertyChanged("LevelupProgress");
            }
        }

        private int _LevelupMaximum;
        public int LevelupMaximum
        {
            get { return _LevelupMaximum; }
            private set
            {
                _LevelupMaximum = value;
                OnPropertyChanged("LevelupMaximum");
            }
        }

        private Difficulty _Difficulty;
        public Difficulty Difficulty
        {
            get { return _Difficulty; }
            private set
            {
                _Difficulty = value;
                LevelupMaximum = LevelupDuration[value]; 
                OnPropertyChanged("Difficulty");
            }
        }

        private State _State = State.Beginning;
        public State State
        {
            get { return _State; }
            private set
            {
                _State = value;
                if(value == State.Beginning)
                {
                    FallingTimer.Stop();
                    Score = 0;
                    Level = 0;
                    LevelupCounter = 0;
                    TotalBlockSets = 0;
                    Held = false;
                    FallingBlockSet = null;
                    BlockSetHold = null;
                    BlockSetQueue = new Queue<BlockSet>();
                }
                OnPropertyChanged("State");
            }
        }

        private bool _Held = false;
        public bool Held
        {
            get { return _Held; }
            private set
            {
                _Held = value;
                OnPropertyChanged("Held");
            }
        }

        private BlockSet _BlockSetHold = null;
        public BlockSet BlockSetHold
        {
            get { return _BlockSetHold; }
            private set
            {
                _BlockSetHold = value;
                OnPropertyChanged("BlockSetHold");
            }
        }

        private Queue<BlockSet> _BlockSetQueue = new Queue<BlockSet>();
        private Queue<BlockSet> BlockSetQueue
        {
            get { return _BlockSetQueue; }
            set
            {
                _BlockSetQueue = value;
                OnPropertyChanged("BlockSetQueue");
                OnGotNextBlockSet();
            }
        }
        public List<BlockSet> BlockSetQueueList
        {
            get { return _BlockSetQueue.ToList(); }
        }

        private int TotalBlockSets = 0;
        private int _TotalClearRows = 0;
        public int TotalClearRows
        {
            get { return _TotalClearRows; }
            set
            {
                _TotalClearRows = value;
                OnPropertyChanged("TotalClearRows");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string info) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        #endregion

        #region Stage Stuffs
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int BlocksLength { get; private set; }
        public int BlocksWidth { get; private set; }
        public int BlocksHeight { get; private set; }
        private int BombingPointX { get { return BlocksWidth / 2 - 2; } }
        private Model.Block[,] Blocks { get; set; }
        private View.Block[,] Stage { get; set; }
        public List<Rectangle> StageView { get; private set; } = new List<Rectangle>();
        #endregion

        #region Falling BlockSet Stuffs
        private BlockSet FallingBlockSet;
        private DispatcherTimer FallingTimer = new DispatcherTimer();
        private int _FallingTime;
        private int FallingTime
        {
            get { return _FallingTime; }
            set
            {
                _FallingTime = value;
                FallingTimer.Interval = TimeSpan.FromMilliseconds(value);
            }
        }
        #endregion

        #region Constructor
        public GameHost(int width, int height, int length, Difficulty difficulty)
        {
            StageInitialize(width, height, length);
            FallingTimerInitialize();
            State = State.Beginning;
            Difficulty = difficulty;
        }
        public GameHost() : this(300, 600, 30, Difficulty.Hard) { }
        #endregion

        #region Initialize
        private void StageInitialize(int width, int height, int length)
        {
            Width = width;
            Height = height;
            BlocksLength = length;
            BlocksWidth = Width / BlocksLength;
            BlocksHeight = Height / BlocksLength;
            Blocks = new Model.Block[BlocksHeight, BlocksWidth];
            Stage = new View.Block[BlocksHeight, BlocksWidth];

            for (int y = 0; y < BlocksHeight; y++)
            {
                for (int x = 0; x < BlocksWidth; x++)
                {
                    Blocks[y, x] = new Block();
                    Stage[y, x] = new View.Block(BlocksLength - 2);

                    Rectangle rectangle = new Rectangle
                    {
                        Margin = new Thickness(1)
                    };
                    Binding brushBinding = new Binding("Brush")
                    {
                        Source = Stage[y, x]
                    };
                    rectangle.SetBinding(Rectangle.FillProperty, brushBinding);
                    Binding lengthBinding = new Binding("Length")
                    {
                        Source = Stage[y, x]
                    };
                    rectangle.SetBinding(Rectangle.HeightProperty, lengthBinding);
                    rectangle.SetBinding(Rectangle.WidthProperty, lengthBinding);

                    StageView.Add(rectangle);
                }
            }
        }
        private void FallingTimerInitialize()
        {
            FallingTimer.Tick += FallDown;
            FallingTime = FallingTimes[0];
        }
        #endregion

        #region Game Controls
        public void Start()
        {
            FallingTimer.Start();
            State = State.Playing;
        }

        public void Pause()
        {
            FallingTimer.Stop();
            State = State.Pause;
        }

        public void Restart()
        {
            FallingTimer.Stop();
            State = State.Beginning;

            foreach(Model.Block block in Blocks)
            {
                block.Empty = true;
            }
            FallingBlockSet = null;

            Show();
        }
        #endregion

        #region View Controls
        private void Show()
        {
            /* Draw Exist BLocks */
            for (int y = 0; y < BlocksHeight; y++)
            {
                for (int x = 0; x < BlocksWidth; x++)
                {
                    if (Blocks[y, x].Empty)
                        Stage[y, x].Empty = true;
                    else
                        Stage[y, x].Brush = Blocks[y, x].Brush;
                }
            }

            if (FallingBlockSet != null)
            {
                bool[] shadow = new bool[BlocksWidth];
                /* Find Shadow */
                for (int y = 0; y < FallingBlockSet.Length; y++)
                {
                    for (int x = 0; x < FallingBlockSet.Length; x++)
                    {
                        int TargetX = x + FallingBlockSet.OriginX;
                        if (TargetX < 0 || TargetX >= BlocksWidth) continue;

                        if (!FallingBlockSet.Blocks[y, x].Empty)
                            shadow[TargetX] = true;
                    }
                }

                /* Draw Shadow */
                Brush ShadowBrush = FallingBlockSet.Brush.Clone();
                ShadowBrush.Opacity = 0.125;
                for (int y = 0; y < BlocksHeight; y++)
                {
                    for (int x = 0; x < BlocksWidth; x++)
                    {
                        if (Blocks[y, x].Empty & shadow[x])
                            Stage[y, x].Brush = ShadowBrush;
                    }
                }

                /* Draw FallingBlockSet */
                for (int y = 0; y < FallingBlockSet.Length; y++)
                {
                    for (int x = 0; x < FallingBlockSet.Length; x++)
                    {
                        int TargetX = x + FallingBlockSet.OriginX;
                        int TargetY = y + FallingBlockSet.OriginY;
                        if (TargetX < 0 || TargetX >= BlocksWidth) continue;
                        if (TargetY < 0 || TargetY >= BlocksHeight) continue;

                        if (FallingBlockSet.Blocks[y, x].Empty)
                        {
                            if (!Blocks[TargetY, TargetX].Empty)
                                Stage[TargetY, TargetX].Brush = Blocks[TargetY, TargetX].Brush;
                        }
                        else
                        {
                            Stage[TargetY, TargetX].Brush = FallingBlockSet.Brush;
                        }
                    }
                }
            }
        }

        private void SetBlocks()
        {
            if (FallingBlockSet == null) return;
            for(int y = 0; y < FallingBlockSet.Length; y++)
            {
                for(int x = 0; x < FallingBlockSet.Length; x++)
                {
                    if (!FallingBlockSet.Blocks[y, x].Empty)
                        Blocks[FallingBlockSet.OriginY + y, FallingBlockSet.OriginX + x].Brush = FallingBlockSet.Brush;
                }
            }
        }
        #endregion

        #region Check Crash Functions
        /// <summary>
        /// Check if BlockSet crash edge or other block in specific direction.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>return true if crash, or false if not.</returns>
        private bool CheckCrash(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:    return CheckCrash(0, 0, -1);
                case Direction.Right: return CheckCrash(0, 1, 0);
                case Direction.Down:  return CheckCrash(0, 0, 1);
                case Direction.Left:  return CheckCrash(0, -1, 0);
                default: return true;
            }
        }

        /// <summary>
        /// Check if BlockSet crash edge or other block while rotate.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>return true if crash, or false if not.</returns>
        private bool CheckCrash(Turn turn)
        {
            switch (turn)
            {
                case Turn.Clockwise: return CheckCrash(1, 0, 0);
                case Turn.CounterClockwise: return CheckCrash(-1, 0, 0);
                default: return true;
            }
        }

        /// <summary>
        /// Check if BlockSet crash edge or other block while rotate and toward specific direction.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>return true if crash, or false if not.</returns>
        private bool CheckCrash(Turn turn, Direction direction)
        {
            switch (turn)
            {
                case Turn.Clockwise:
                    switch (direction)
                    {
                        case Direction.Up:    return CheckCrash(1, 0, -1);
                        case Direction.Right: return CheckCrash(1, 1, 0);
                        case Direction.Down:  return CheckCrash(1, 0, 1);
                        case Direction.Left:  return CheckCrash(1, -1, 0);
                        default: return true;
                    }
                case Turn.CounterClockwise:
                    switch (direction)
                    {
                        case Direction.Up:    return CheckCrash(-1, 0, -1);
                        case Direction.Right: return CheckCrash(-1, 1, 0);
                        case Direction.Down:  return CheckCrash(-1, 0, 1);
                        case Direction.Left:  return CheckCrash(-1, -1, 0);
                        default: return true;
                    }
                default: return true;
            }
        }

        private bool CheckCrash(int rotate, int offsetX, int offsetY)
        {
            bool[,] newMap;

            if (FallingBlockSet == null) return false;
            switch (rotate)
            {
                case 1:
                    newMap = FallingBlockSet.BlockMap[FallingBlockSet.GetNextFace(Turn.Clockwise)];
                    break;
                case -1:
                    newMap = FallingBlockSet.BlockMap[FallingBlockSet.GetNextFace(Turn.CounterClockwise)];
                    break;
                case 0:
                default:
                    newMap = FallingBlockSet.BlockMap[FallingBlockSet.Face];
                    break;
            }

            for (int y = 0; y < FallingBlockSet.Length; y++)
            {
                for (int x = 0; x < FallingBlockSet.Length; x++)
                {
                    if (newMap[y, x])
                    {
                        int TargetX = FallingBlockSet.OriginX + offsetX + x;
                        int TargetY = FallingBlockSet.OriginY + offsetY + y;

                        /* Edge checking */
                        if (TargetX < 0 || TargetX >= BlocksWidth) return true;
                        if (TargetY < 0 || TargetY >= BlocksHeight) return true;

                        /* Block checking */
                        if (!Blocks[TargetY, TargetX].Empty) return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Block Controls
        public void Rotate(Model.Turn turn)
        {
            if (FallingBlockSet == null) return;

            if (!CheckCrash(turn))
            {
                FallingBlockSet.Rotate(turn);
            }
            else if(!CheckCrash(turn, Direction.Left))
            {
                FallingBlockSet.Move(Direction.Left);
                FallingBlockSet.Rotate(turn);
            }
            else if(!CheckCrash(turn, Direction.Right))
            {
                FallingBlockSet.Move(Direction.Right);
                FallingBlockSet.Rotate(turn);
            }
            else if(FallingBlockSet is BlocksI)
            {
                if (!CheckCrash(1, 2, 0))
                {
                    FallingBlockSet.Move(Direction.Right);
                    FallingBlockSet.Move(Direction.Right);
                    FallingBlockSet.Rotate(turn);
                }
            }
            
            Show();
        }

        public void MoveLeft()
        {
            if (FallingBlockSet == null) return;
            if (!CheckCrash(Direction.Left)) FallingBlockSet.Move(Direction.Left);
            Show();
        }

        public void MoveRight()
        {
            if (FallingBlockSet == null) return;
            if (!CheckCrash(Direction.Right)) FallingBlockSet.Move(Direction.Right);
            Show();
        }

        public void HoldNow()
        {
            if (FallingBlockSet == null) return;
            if (!Held)
            {
                FallingTimer.Stop();

                FallingBlockSet.Initial();
                if (FallingBlockSet is BlocksS || FallingBlockSet is BlocksT || FallingBlockSet is BlocksZ)
                    FallingBlockSet.SetOrigin(BombingPointX, -1);
                else
                    FallingBlockSet.SetOrigin(BombingPointX, 0);

                if (BlockSetHold != null)
                {
                    BlockSet bs = BlockSetHold;
                    BlockSetHold = FallingBlockSet;
                    FallingBlockSet = bs;
                }
                else
                {
                    BlockSetHold = FallingBlockSet;
                    FallingBlockSet = NextBlockSet();
                }

                Held = true;
                Show();
                FallingTimer.Start();
            }
        }

        public void ForceFall()
        {
            if (FallingBlockSet == null) return;
            FallingTimer.Stop();

            if (!CheckCrash(Direction.Down))
            {
                FallingBlockSet.Move(Direction.Down);
                Score += 1;
            }
            else
            {
                BlockSetStopMoving();
            }

            Show();
            FallingTimer.Start();
        }

        public void FallToBottom()
        {
            if (FallingBlockSet == null) return;
            FallingTimer.Stop();

            while (!CheckCrash(Direction.Down))
            {
                FallingBlockSet.Move(Direction.Down);
                Score += 2;
            }
            BlockSetStopMoving();

            Show();
            FallingTimer.Start();
        }

        private void BlockSetStopMoving()
        {
            SetBlocks();
            FallingBlockSet = NextBlockSet();
            Held = false;

            LevelupCounter++;
            int count = CheckComplete();
            Score += GetScore(count);
            LevelupCounter -= ((count + 1) * count / 2) + ((2 - (int)Difficulty) * count);
            if (LevelupCounter < 0) LevelupCounter = 0;
            Level = LevelupCounter / LevelupDuration[Difficulty];
            if (LevelupCounter > LevelupDuration[Difficulty] * MaxLevel) LevelupCounter = LevelupDuration[Difficulty] * MaxLevel;

            /* Check End Game */
            if (CheckCrash(0, 0, 0))
            {
                State = State.Dead;
                FallingTimer.Stop();
            }
        }
        #endregion

        #region Row Measure
        private void DeleteRow(int row)
        {
            if (row < 0 || row >= BlocksHeight) return;
            for(int y = row; y > 0; y--)
            {
                for(int x = 0; x < BlocksWidth; x++)
                {
                    if (!Blocks[y - 1, x].Empty)
                        Blocks[y, x].Brush = Blocks[y - 1, x].Brush;
                    else
                        Blocks[y, x].Empty = true;
                }
            }

            for (int x = 0; x < BlocksWidth; x++)
            {
                Blocks[0, x].Empty = true;
            }
        }

        private bool RowComplete(int row)
        {
            if (row < 0 || row >= BlocksHeight) return false;
            for(int colume = 0; colume < BlocksWidth; colume++)
            {
                if (Blocks[row, colume].Empty) return false;
            }
            return true;
        }

        private int CheckComplete()
        {
            int count = 0;
            for(int row = 0; row < BlocksHeight; row++)
            {
                if (RowComplete(row))
                {
                    DeleteRow(row);
                    count++;
                }
            }

            TotalClearRows += count;
            return count;
        }
        #endregion

        #region BlockSet Generator
        private BlockSet GetBlockSet()
        {
            int index = random.Next(70) / 10;
            switch (index)
            {
                case 0: return new BlocksI(BombingPointX, 0);
                case 1: return new BlocksJ(BombingPointX, 0);
                case 2: return new BlocksL(BombingPointX, 0);
                case 3: return new BlocksO(BombingPointX, 0);
                case 4: return new BlocksS(BombingPointX, -1);
                case 5: return new BlocksT(BombingPointX, -1);
                case 6: return new BlocksZ(BombingPointX, -1);
                default: return new BlocksO(BombingPointX, 0);
            }
        }
        #endregion

        #region BlockSetQueue Functions
        private BlockSet NextBlockSet()
        {
            while(BlockSetQueue.Count <= 6)
            {
                BlockSetQueue.Enqueue(GetBlockSet());
            }
            BlockSet result = BlockSetQueue.Dequeue();
            TotalBlockSets++;
            OnGotNextBlockSet();

            return result;
        }
        public event EventHandler GotNextBlockSet;
        private void OnGotNextBlockSet() => GotNextBlockSet?.Invoke(this, new EventArgs());
        #endregion

        #region Score Calc
        private int GetScore(int count)
        {
            if (count == 0) return 0;
            return 5 * ((3 + count) * count / 2 + TotalClearRows / 5) * (2 + (int)Difficulty);
        }
        #endregion

        #region Timer Routine
        private void FallDown(object sender, EventArgs e)
        {
            if (FallingBlockSet != null)
            {
                if (!CheckCrash(Direction.Down))
                {
                    FallingBlockSet.Move(Direction.Down);
                }
                else
                {
                    BlockSetStopMoving();
                }
            }
            else
            {
                FallingBlockSet = NextBlockSet();
            }

            /* Check End Game */
            if(CheckCrash(0, 0, 0))
            {
                State = State.Dead;
                FallingTimer.Stop();
            }
                
            Show();
        }
        #endregion
    }
}
