using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tetris.Model;

namespace Tetris.View
{
    public partial class BlockSetViewer : UserControl
    {
        private static readonly Brush DefaultBlockBrush = new SolidColorBrush(Color.FromArgb(63, 0, 0, 0));
        
        private int _Length;
        public int Length
        {
            get { return _Length; }
            set
            {
                _Length = value;
                this.MaxWidth = value;
                this.MaxHeight = value;
            }
        }

        private BlockSet _BlockSet;
        public BlockSet BlockSet
        {
            get { return _BlockSet; }
            set
            {
                _BlockSet = value;
                if (_BlockSet != null)
                    BlockSetInitialize();
                else
                    SetAsEmpty();
            }
        }

        public BlockSetViewer()
        {
            InitializeComponent();
        }

        private void BlockSetInitialize()
        {
            BlockContainer.Children.Clear();

            int BlocksWidth = BlockSet.Length;
            this.Length = BlocksWidth * 20 + 10;

            int BlockLength = (this.Length - 10) / BlocksWidth - 2;
            for(int y = 0; y < BlocksWidth; y++)
            {
                for(int x = 0; x < BlocksWidth; x++)
                {
                    Rectangle r = new Rectangle()
                    {
                        Width = BlockLength,
                        Height = BlockLength,
                        Margin = new Thickness(1)
                    };
                    if (!BlockSet.Blocks[y, x].Empty)
                        r.Fill = BlockSet.Brush;
                    else
                        r.Fill = DefaultBlockBrush;

                    BlockContainer.Children.Add(r);
                }
            }
        }

        private void SetAsEmpty()
        {
            BlockContainer.Children.Clear();
            Length = 100;
        }
    }
}
