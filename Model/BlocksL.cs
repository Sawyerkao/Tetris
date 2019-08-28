using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tetris.Model
{
    public class BlocksL : BlockSet
    {
        private static readonly Brush BrushL = new SolidColorBrush(Color.FromRgb(255, 166, 0));
        private static readonly Dictionary<Face, bool[,]> BlockMapL = new Dictionary<Face, bool[,]>()
        {
            {Face.Up,
                new bool[,]
                {
                    { false, true, false },
                    { false, true, false },
                    { false, true, true }
                }
            },

            {Face.Right,
                new bool[,]
                {
                    { false, false, false },
                    { true, true, true },
                    { true, false, false }
                }
            },

            {Face.Down,
                new bool[,]
                {
                    { true, true, false },
                    { false, true, false },
                    { false, true, false }
                }
            },

            {Face.Left,
                new bool[,]
                {
                    { false, false, false },
                    { false, false, true },
                    { true, true, true }
                }
            }
        };

        public override Brush Brush => BrushL;
        public override int Length => 3;
        public override Dictionary<Face, bool[,]> BlockMap => BlockMapL;

        public BlocksL() : base(0, 0) { }
        public BlocksL(int x, int y) : base(x, y) { }

        public override string ToString() => "BlockSet_L";
    }
}
