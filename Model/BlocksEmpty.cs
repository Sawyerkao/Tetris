using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tetris.Model
{
    public class BlocksEmpty : BlockSet
    {
        private static readonly Brush BrushEmpty = new SolidColorBrush(Color.FromArgb(63, 0, 0, 0));
        private static readonly Dictionary<Face, bool[,]> BlockMapEmpty = new Dictionary<Face, bool[,]>()
        {
            {Face.Up,
                new bool[,]
                {
                    { false, false, false },
                    { false, false, false },
                    { false, false, false }
                }
            },

            {Face.Right,
                new bool[,]
                {
                    { false, false, false },
                    { false, false, false },
                    { false, false, false }
                }
            },

            {Face.Down,
                new bool[,]
                {
                    { false, false, false },
                    { false, false, false },
                    { false, false, false }
                }
            },

            {Face.Left,
                new bool[,]
                {
                    { false, false, false },
                    { false, false, false },
                    { false, false, false }
                }
            }
        };

        public override Brush Brush => BrushEmpty;
        public override int Length => 3;
        public override Dictionary<Face, bool[,]> BlockMap => BlockMapEmpty;

        public BlocksEmpty() : base(0, 0) { }
        public BlocksEmpty(int x, int y) : base(x, y) { }

        public override string ToString() => "BlockSet_Empty";
    }
}
