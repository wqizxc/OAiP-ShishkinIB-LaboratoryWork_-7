using System.Windows;
using System.Windows.Media;

namespace Lab7
{
    public abstract class Figure
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Brush StrokeBrush { get; set; } = Brushes.Black;
        public double StrokeThickness { get; set; } = 2;

        protected Figure(double x, double y)
        {
            X = x;
            Y = y;
        }

        public abstract void Draw(DrawingContext dc);
        public abstract void Move(double dx, double dy);
        public abstract double GetWidth();
        public abstract double GetHeight();
        public abstract Rect GetBounds();
    }
}