using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tetris.View
{
    public class Block : INotifyPropertyChanged
    {
        private static readonly Brush DefaultBlockBrush = new SolidColorBrush(Color.FromArgb(63, 0, 0, 0));

        private bool _Empty;
        public bool Empty
        {
            get { return _Empty; }
            set
            {
                _Empty = value;
                if (_Empty)
                    Brush = DefaultBlockBrush;
            }
        }

        /* Constructors */
        public Block()
        {
            Empty = true;
            Length = 30;
        }
        public Block(int length)
        {
            Empty = true;
            Length = length;
        }
        public Block(Brush brush)
        {
            Brush = brush;
            Length = 30;
        }
        public Block(Brush brush, int length)
        {
            Brush = brush;
            Length = length;
        }

        private Brush _Brush { get; set; }
        public Brush Brush
        {
            get { return _Brush; }
            set
            {
                _Brush = value;
                Empty = false;
                OnPropertyChanged("Brush");
            }
        }
        private int _Length { get; set; }
        public int Length
        {
            get { return _Length; }
            set
            {
                _Length = value;
                OnPropertyChanged("Length");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string info) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
    }
}
