using System.Windows;
using System.Windows.Media;

namespace Lab7
{
    public class Ellipse : Figure
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public Ellipse(double x, double y, double width, double height) : base(x, y)
        {
            Width = width;
            Height = height;
        }

        public override void Draw(DrawingContext dc)
        {
            dc.DrawEllipse(null, new Pen(StrokeBrush, StrokeThickness),
                new Point(X + Width / 2, Y + Height / 2), Width / 2, Height / 2);
        }

        public override void Move(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public override double GetWidth() => Width;
        public override double GetHeight() => Height;
        public override Rect GetBounds() => new Rect(X, Y, Width, Height);
    }
}