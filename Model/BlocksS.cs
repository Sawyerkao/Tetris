using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tetris.Model
{
    public class BlocksS : BlockSet
    {
        private static readonly Brush BrushS = new SolidColorBrush(Color.FromRgb(155, 255, 0));
        private static readonly Dictionary<Face, bool[,]> BlockMapS = new Dictionary<Face, bool[,]>()
        {
            {Face.Up,
                new bool[,]
                {
                    { false, false, false },
                    { false, true, true },
                    { true, true, false }
                }
            },

            {Face.Right,
                new bool[,]
                {
                    { true, false, false },
                    { true, true, false },
                    { false, true, false }
                }
            },

            {Face.Down,
                new bool[,]
                {
                    { false, false, false },
                    { false, true, true },
                    { true, true, false }
                }
            },

            {Face.Left,
                new bool[,]
                {
                    { true, false, false },
                    { true, true, false },
                    { false, true, false }
                }
            },
        };

        public override Brush Brush => BrushS;
        public override int Length => 3;
        public override Dictionary<Face, bool[,]> BlockMap => BlockMapS;

        public BlocksS() : base(0, 0) { }
        public BlocksS(int x, int y) : base(x, y) { }

        public override string ToString() => "BlockSet_S";
    }
}
