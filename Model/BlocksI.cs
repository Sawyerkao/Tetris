using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tetris.Model
{
    public class BlocksI : BlockSet
    {
        private static readonly Brush BrushI = new SolidColorBrush(Color.FromRgb(0, 236, 255));
        private static readonly Dictionary<Face, bool[,]> BlockMapI = new Dictionary<Face, bool[,]>()
        {
            {Face.Up,
                new bool[,]
                {
                    { false, false, true, false },
                    { false, false, true, false },
                    { false, false, true, false },
                    { false, false, true, false }
                }
            },

            {Face.Right,
                new bool[,]
                {
                    { false, false, false, false },
                    { false, false, false, false },
                    { true, true, true, true },
                    { false, false, false, false }
                }
            },

            {Face.Down,
                new bool[,]
                {
                    { false, false, true, false },
                    { false, false, true, false },
                    { false, false, true, false },
                    { false, false, true, false }
                }
            },

            {Face.Left,
                new bool[,]
                {
                    { false, false, false, false },
                    { false, false, false, false },
                    { true, true, true, true },
                    { false, false, false, false }
                }
            }
        };

        public override Brush Brush => BrushI;
        public override int Length => 4;
        public override Dictionary<Face, bool[,]> BlockMap => BlockMapI;

        public BlocksI() : base(0, 0) { }
        public BlocksI(int x, int y) : base(x, y) { }

        public override string ToString() => "BlockSet_I";
    }
}
