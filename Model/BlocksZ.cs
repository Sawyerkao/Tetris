using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tetris.Model
{
    public class BlocksZ : BlockSet
    {
        private static readonly Brush BrushZ = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private static readonly Dictionary<Face, bool[,]> BlockMapZ = new Dictionary<Face, bool[,]>()
        {
            {Face.Up,
                new bool[,]
                {
                    { false, false, false },
                    { true, true, false },
                    { false, true, true }
                }
            },

            {Face.Right,
                new bool[,]
                {
                    { false, false, true },
                    { false, true, true },
                    { false, true, false }
                }
            },

            {Face.Down,
                new bool[,]
                {
                    { false, false, false },
                    { true, true, false },
                    { false, true, true }
                }
            },

            {Face.Left,
                new bool[,]
                {
                    { false, false, true },
                    { false, true, true },
                    { false, true, false }
                }
            },
        };

        public override Brush Brush => BrushZ;
        public override int Length => 3;
        public override Dictionary<Face, bool[,]> BlockMap => BlockMapZ;

        public BlocksZ() : base(0, 0) { }
        public BlocksZ(int x, int y) : base(x, y) { }

        public override string ToString() => "BlockSet_Z";
    }
}
