using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tetris.Model
{
    public class BlocksT : BlockSet
    {
        private static readonly Brush BrushT = new SolidColorBrush(Color.FromRgb(223, 0, 255));
        private static readonly Dictionary<Face, bool[,]> BlockMapT = new Dictionary<Face, bool[,]>()
        {
            {Face.Up,
                new bool[,]
                {
                    { false, false, false },
                    { false, true, false },
                    { true, true, true }
                }
            },

            {Face.Right,
                new bool[,]
                {
                    { false, true, false },
                    { false, true, true },
                    { false, true, false }
                }
            },

            {Face.Down,
                new bool[,]
                {
                    { false, false, false },
                    { true, true, true },
                    { false, true, false }
                }
            },

            {Face.Left,
                new bool[,]
                {
                    { false, true, false },
                    { true, true, false },
                    { false, true, false }
                }
            },
        };

        public override Brush Brush => BrushT;
        public override int Length => 3;
        public override Dictionary<Face, bool[,]> BlockMap => BlockMapT;

        public BlocksT() : base(0, 0) { }
        public BlocksT(int x, int y) : base(x, y) { }

        public override string ToString() => "BlockSet_T";
    }
}
