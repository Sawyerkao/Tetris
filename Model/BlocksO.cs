using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tetris.Model
{
    public class BlocksO : BlockSet
    {
        private static readonly Brush BrushO = new SolidColorBrush(Color.FromRgb(255, 223, 0));
        private static readonly Dictionary<Face, bool[,]> BlockMapO = new Dictionary<Face, bool[,]>()
        {
            {Face.Up,
                new bool[,]
                {
                    { true, true },
                    { true, true }
                }
            },

            {Face.Right,
                new bool[,]
                {
                    { true, true },
                    { true, true }
                }
            },

            {Face.Down,
                new bool[,]
                {
                    { true, true },
                    { true, true }
                }
            },

            {Face.Left,
                new bool[,]
                {
                    { true, true },
                    { true, true }
                }
            },
        };

        public override Brush Brush => BrushO;
        public override int Length => 2;
        public override Dictionary<Face, bool[,]> BlockMap => BlockMapO;

        public BlocksO() : base(0, 0) { }
        public BlocksO(int x, int y) : base(x, y) { }

        public override string ToString() => "BlockSet_O";
    }
}
