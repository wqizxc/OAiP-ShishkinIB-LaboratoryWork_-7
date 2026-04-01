using System;

namespace Lab7
{
    public class Square : Rectangle
    {
        public Square(double x, double y, double side) : base(x, y, side, side) { }

        public override void Resize(double newWidth, double newHeight)
        {
            double side = Math.Max(newWidth, newHeight);
            if (side > 0)
            {
                Width = side;
                Height = side;
            }
        }
    }
}