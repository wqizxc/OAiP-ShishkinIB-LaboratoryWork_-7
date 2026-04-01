using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Lab7
{
    public class Polygon : Figure
    {
        public List<Point> RelativePoints { get; set; }
        public List<Point> OriginalPoints { get; set; }

        public Polygon(double x, double y, List<Point> points) : base(x, y)
        {
            OriginalPoints = new List<Point>(points);
            RelativePoints = NormalizePoints(points);
        }

        private List<Point> NormalizePoints(List<Point> points)
        {
            if (points.Count < 3) return points;
            double minX = points.Min(p => p.X);
            double minY = points.Min(p => p.Y);

            var normalized = points.Select(p => new Point(p.X - minX, p.Y - minY)).ToList();

            return normalized;
        }

        public override void Draw(DrawingContext dc)
        {
            if (RelativePoints.Count < 3) return;

            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                var startPoint = new Point(X + RelativePoints[0].X, Y + RelativePoints[0].Y);
                context.BeginFigure(startPoint, true, true);

                for (int i = 1; i < RelativePoints.Count; i++)
                {
                    context.LineTo(new Point(X + RelativePoints[i].X, Y + RelativePoints[i].Y), true, false);
                }
            }
            dc.DrawGeometry(null, new Pen(StrokeBrush, StrokeThickness), geometry);
        }

        public override void Move(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public override double GetWidth()
        {
            if (RelativePoints.Count == 0) return 0;
            return RelativePoints.Max(p => p.X) - RelativePoints.Min(p => p.X);
        }

        public override double GetHeight()
        {
            if (RelativePoints.Count == 0) return 0;
            return RelativePoints.Max(p => p.Y) - RelativePoints.Min(p => p.Y);
        }

        public override Rect GetBounds()
        {
            if (RelativePoints.Count == 0) return new Rect(X, Y, 0, 0);
            double minX = RelativePoints.Min(p => p.X);
            double minY = RelativePoints.Min(p => p.Y);
            double maxX = RelativePoints.Max(p => p.X);
            double maxY = RelativePoints.Max(p => p.Y);
            return new Rect(X + minX, Y + minY, maxX - minX, maxY - minY);
        }
    }
}