using System.Collections.Generic;
using System.Windows.Media;

namespace Tetris.Model
{
    public enum Turn
    {
        Clockwise,
        CounterClockwise
    }

    public enum Face
    {
        Up,
        Right,
        Down,
        Left
    }

    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    public abstract class BlockSet
    {
        public Block[,] Blocks { get; protected set; }
        public int OriginX { get; protected set; }
        public int OriginY { get; protected set; }

        private Face _Face;
        public Face Face
        {
            get { return _Face; }
            protected set
            {
                _Face = value;
                SetBlocks(value);
            }
        }

        public abstract Brush Brush { get; }
        public abstract int Length { get; }
        public abstract Dictionary<Face, bool[,]> BlockMap { get; }

        protected BlockSet(int originX, int originY)
        {
            Initial();
            SetOrigin(originX, originY);
            Blocks = new Block[Length, Length];
            for(int y = 0; y < Blocks.GetLength(0); y++)
            {
                for(int x = 0; x < Blocks.GetLength(1); x++)
                {
                    Blocks[y, x] = new Block();
                }
            }
            Face = Face.Up;
        }

        public void SetOrigin(int x, int y)
        {
            OriginX = x;
            OriginY = y;
        }

        public void Initial()
        {
            SetOrigin(0, 0);
            Face = Face.Up;
        }

        public void Rotate(Turn turn)
        {
            NextFace(turn);
        }

        public void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    this.OriginY -= 1;
                    break;
                case Direction.Right:
                    this.OriginX += 1;
                    break;
                case Direction.Down:
                    this.OriginY += 1;
                    break;
                case Direction.Left:
                    this.OriginX -= 1;
                    break;
            }
        }

        private void SetBlocks(Face face)
        {
            bool[,] map = BlockMap[face];
            if (Blocks == null) return;

            for(int y = 0; y < map.GetLength(0); y++)
            {
                for(int x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x])
                        Blocks[y, x].Brush = Brush;
                    else
                        Blocks[y, x].Empty = true;
                }
            }
        }

        private void NextFace(Turn turn)
        {
            switch (turn)
            {
                case Turn.Clockwise:
                    switch (Face)
                    {
                        case Face.Up:
                            Face = Face.Right;
                            break;
                        case Face.Right:
                            Face = Face.Down;
                            break;
                        case Face.Down:
                            Face = Face.Left;
                            break;
                        case Face.Left:
                            Face = Face.Up;
                            break;
                    }
                    break;
                case Turn.CounterClockwise:
                    switch (Face)
                    {
                        case Face.Up:
                            Face = Face.Left;
                            break;
                        case Face.Right:
                            Face = Face.Up;
                            break;
                        case Face.Down:
                            Face = Face.Right;
                            break;
                        case Face.Left:
                            Face = Face.Down;
                            break;
                    }
                    break;
            }
        }

        public Face GetNextFace(Turn turn)
        {
            Face result = Face.Up;
            switch (turn)
            {
                case Turn.Clockwise:
                    switch (Face)
                    {
                        case Face.Up:
                            result = Face.Right;
                            break;
                        case Face.Right:
                            result = Face.Down;
                            break;
                        case Face.Down:
                            result = Face.Left;
                            break;
                        case Face.Left:
                            result = Face.Up;
                            break;
                    }
                    break;
                case Turn.CounterClockwise:
                    switch (Face)
                    {
                        case Face.Up:
                            result = Face.Left;
                            break;
                        case Face.Right:
                            result = Face.Up;
                            break;
                        case Face.Down:
                            result = Face.Right;
                            break;
                        case Face.Left:
                            result = Face.Down;
                            break;
                    }
                    break;
            }
            return result;
        }
    }
}