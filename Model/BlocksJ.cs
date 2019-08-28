using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tetris.Model
{
    public class BlocksJ : BlockSet
    {
        private static readonly Brush BrushJ = new SolidColorBrush(Color.FromRgb(0, 99, 255));
        private static readonly Dictionary<Face, bool[,]> BlockMapJ = new Dictionary<Face, bool[,]>()
        {
            {Face.Up,
                new bool[,]
                {
                    { false, true, false },
                    { false, true, false },
                    { true, true, false }
                }
            },

            {Face.Right,
                new bool[,]
                {
                    { false, false, false },
                    { true, false, false },
                    { true, true, true }
                }
            },

            {Face.Down,
                new bool[,]
                {
                    { false, true, true },
                    { false, true, false },
                    { false, true, false }
                }
            },

            {Face.Left,
                new bool[,]
                {
                    { false, false, false },
                    { true, true, true },
                    { false, false, true }
                }
            }
        };

        public override Brush Brush => BrushJ;
        public override int Length => 3;
        public override Dictionary<Face, bool[,]> BlockMap => BlockMapJ;

        public BlocksJ() : base(0, 0) { }
        public BlocksJ(int x, int y) : base(x, y) { }

        public override string ToString() => "BlockSet_J";
    }
}
