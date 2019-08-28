using System.Windows.Media;

namespace Tetris.Model
{
    public class Block
    {
        /* Constructors */
        public Block()
        {
            Empty = true;
        }
        public Block(Color color)
        {
            Brush = new SolidColorBrush(color);
        }
        public Block(Brush brush)
        {
            Brush = brush;
        }

        public bool Empty { get; set; } = true;
        private Brush _Brush;
        public Brush Brush
        {
            get { return _Brush; }
            set
            {
                _Brush = value;
                Empty = false;
            }
        }

        /* Property Setters */
        public void SetColor(Color color)
        {
            Brush = new SolidColorBrush(color);
        }
    }
}
