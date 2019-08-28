using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Tetris.Control;
using Tetris.Model;
using Tetris.View;

namespace Tetris
{
    public enum MainState
    {
        Beginning,
        ChooseDifficulty,
        Ready,
        Playing,
        Help,
        Froze,
        GameOver
    }

    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainState _MainState;
        private MainState MainState
        {
            get { return _MainState; }
            set
            {
                _MainState = value;

                HelpBoard.Visibility = Visibility.Hidden;
                DifficultyBoard.Visibility = Visibility.Hidden;
                ReadyBoard.Visibility = Visibility.Hidden;
                ResultBoard.Visibility = Visibility.Hidden;

                switch (value)
                {
                    case MainState.Beginning: HelpBoard.Visibility = Visibility.Visible; break;
                    case MainState.ChooseDifficulty: DifficultyBoard.Visibility = Visibility.Visible; break;
                    case MainState.Ready: ReadyBoard.Visibility = Visibility.Visible; break;
                    case MainState.Playing: break;
                    case MainState.Help: HelpBoard.Visibility = Visibility.Visible; break;
                    case MainState.Froze: ResultBoard.Visibility = Visibility.Visible; FrozeTimer.Start(); break;
                    case MainState.GameOver: ResultBoard.Visibility = Visibility.Visible; break;
                }
            }
        }

        private GameHost Host;
        private DispatcherTimer FrozeTimer;
        
        public MainWindow()
        {
            InitializeComponent();

            FrozeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            FrozeTimer.Tick += Unfroze;

            this.KeyDown += new KeyEventHandler(OnButtonKeyDown);
            MainState = MainState.Beginning;
        }

        #region Basic Window Functions
        private void TitleBarPressed(object sender, MouseButtonEventArgs e) => this.DragMove();
        private void BtnCloseClick(object sender, RoutedEventArgs e) => this.Close();
        private void BtnMinimizeClick(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;
        private void BtnInfoClick(object sender, RoutedEventArgs e)
        {
            Window window = new InfoBox();
            window.Show();
        }
        #endregion

        private void GameHostInitialize(Difficulty difficulty)
        {
            Host = new GameHost(300, 600, 30, difficulty);
            Wall.Children.Clear();
            foreach (Rectangle rectangle in Host.StageView)
            {
                Wall.Children.Add(rectangle);
            }
            HoldViewer.Length = 100;
            Host.PropertyChanged += HostPropertyChanged;
            Host.GotNextBlockSet += HostGotNextBlockSet;
            BindingInitialize();
        }

        private void BindingInitialize()
        {
            Binding DifficultyBinding = new Binding("Difficulty")
            {
                Source = Host
            };
            DifficultyLabel.SetBinding(Label.ContentProperty, DifficultyBinding);

            Binding LevelBinding = new Binding("Level")
            {
                Source = Host
            };
            LevelLabel.SetBinding(Label.ContentProperty, LevelBinding);

            Binding ScoreBinding = new Binding("Score")
            {
                Source = Host
            };
            ScoreLabel.SetBinding(Label.ContentProperty, ScoreBinding);
            ResultScore.SetBinding(Label.ContentProperty, ScoreBinding);

            Binding LevelupProgressBinding = new Binding("LevelupProgress")
            {
                Source = Host
            };
            Binding LevelupMaximumBinding = new Binding("LevelupMaximum")
            {
                Source = Host
            };
            LevelupProgressBar.SetBinding(ProgressBar.ValueProperty, LevelupProgressBinding);
            LevelupProgressBar.SetBinding(ProgressBar.MaximumProperty, LevelupMaximumBinding);

            Binding TotalClearRowsBinding = new Binding("TotalClearRows")
            {
                Source = Host
            };
            ResultClearRows.SetBinding(Label.ContentProperty, TotalClearRowsBinding);
        }

        private void HostPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "BlockSetHold":
                    HoldViewer.BlockSet = Host.BlockSetHold;
                    break;
                case "State":
                    if (Host.State == State.Dead)
                    {
                        MainState = MainState.Froze;
                        HoldViewer.BlockSet = null;
                        QueueViewer.Children.Clear();
                    }
                    break;
            }
        }

        private void HostGotNextBlockSet(object sender, EventArgs e)
        {
            List<BlockSet> QueueList = Host.BlockSetQueueList;
            QueueViewer.Children.Clear();

            int minus = (int)Host.Difficulty;
            int count = 0;
            foreach(BlockSet bs in QueueList)
            {
                count++;
                if (count >= QueueList.Count - minus) break;
                BlockSetViewer bsv = new BlockSetViewer()
                {
                    BlockSet = bs,
                    Margin = new Thickness(5)
                };
                QueueViewer.Children.Add(bsv);
            }
        }

        /* Keyboard Controller */
        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            string key = e.Key.ToString();

            if (key.Equals("Escape")) this.Close();

            if (MainState == MainState.Beginning)
            {
                MainState = MainState.ChooseDifficulty;
            }
            else if (MainState == MainState.ChooseDifficulty)
            {
                switch (key)
                {
                    case "Left": GameHostInitialize(Difficulty.Easy); MainState = MainState.Ready; break;
                    case "Down": GameHostInitialize(Difficulty.Normal); MainState = MainState.Ready; break;
                    case "Right": GameHostInitialize(Difficulty.Hard); MainState = MainState.Ready; break;
                }
            }
            else if (MainState == MainState.Ready)
            {
                Host.Start();
                MainState = MainState.Playing;
            }
            else if (MainState == MainState.Playing)
            {
                if (Host.State == State.Beginning)
                {
                    Host.Start();
                }
                else if (Host.State == State.Playing)
                {
                    switch (key)
                    {
                        case "Up": Host.HoldNow(); break;
                        case "Down": Host.ForceFall(); break;
                        case "Left": Host.MoveLeft(); break;
                        case "Right": Host.MoveRight(); break;
                        case "Q": Host.Rotate(Turn.CounterClockwise); break;
                        case "W": Host.Rotate(Turn.Clockwise); break;
                        case "Space": Host.FallToBottom(); break;
                        case "Enter": Host.Pause(); break;
                        case "H": Host.Pause(); MainState = MainState.Help; break;
                    }
                }
                else if (Host.State == State.Pause)
                {
                    switch (key)
                    {
                        case "H": MainState = MainState.Help; break;
                        default: Host.Start(); break;
                    }
                }
            }
            else if ( MainState == MainState.Help)
            {
                MainState = MainState.Playing;
            }
            else if (MainState == MainState.GameOver)
            {
                MainState = MainState.ChooseDifficulty;
            }
        }

        /* Froze While Game Over */
        private void Unfroze(object sender, EventArgs e)
        {
            FrozeTimer.Stop();
            MainState = MainState.GameOver;
        }

        #region Game Mechanic Testing
        private void BtnPlay(object sender, RoutedEventArgs e)
        {
            if(Host.State == State.Playing)
            {
                Host.Pause();
            }
            else if(Host.State == State.Dead)
            {
                Host.Start();
            }
            else
            {
                Host.Start();
            }
        }
        private void BtnLeft(object sender, RoutedEventArgs e) => Host.MoveLeft();
        private void BtnRotateLeft(object sender, RoutedEventArgs e) => Host.Rotate(Turn.CounterClockwise);
        private void BtnForceFall(object sender, RoutedEventArgs e) => Host.ForceFall();
        private void BtnRotateRight(object sender, RoutedEventArgs e) => Host.Rotate(Turn.Clockwise);
        private void BtnRight(object sender, RoutedEventArgs e) => Host.MoveRight();
        private void BtnReset(object sender, RoutedEventArgs e) => Host.Restart();
        private void BtnHold(object sender, RoutedEventArgs e) => Host.HoldNow();
        private void BtnFallToBottom(object sender, RoutedEventArgs e) => Host.FallToBottom();
        #endregion
    }
}
