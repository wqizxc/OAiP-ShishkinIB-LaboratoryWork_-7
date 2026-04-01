using System.Windows;
using System.Windows.Media;

namespace Lab7
{
    public class Rectangle : Figure
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public Rectangle(double x, double y, double width, double height) : base(x, y)
        {
            Width = width;
            Height = height;
        }

        public override void Draw(DrawingContext dc)
        {
            dc.DrawRectangle(null, new Pen(StrokeBrush, StrokeThickness), new Rect(X, Y, Width, Height));
        }

        public override void Move(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public override double GetWidth() => Width;
        public override double GetHeight() => Height;
        public override Rect GetBounds() => new Rect(X, Y, Width, Height);
        public virtual void Resize(double newWidth, double newHeight)
        {
            if (newWidth > 0) Width = newWidth;
            if (newHeight > 0) Height = newHeight;
        }
    }
}