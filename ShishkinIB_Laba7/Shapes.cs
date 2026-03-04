using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GraphicsAppWPF
{   
    public abstract class Shape
    {
        public Point Location { get; set; }
        public Color Color { get; set; } = Colors.Black;
        public bool IsSelected { get; set; }

        protected Shape(double x, double y) => Location = new Point(x, y);

        public abstract void Draw(DrawingContext dc);
        public abstract bool HitTest(Point point);
        public abstract void Move(double dx, double dy, double canvasWidth, double canvasHeight);

        protected void DrawSelectionBorder(DrawingContext dc, Rect bounds)
        {
            if (IsSelected)
            {
                var pen = new Pen(Brushes.Blue, 1);
                pen.DashStyle = DashStyles.Dash;
                dc.DrawRectangle(null, pen, bounds);
            }
        }

        protected bool CheckBounds(double x, double y, double w, double h, double cw, double ch)
            => x >= 0 && x + w <= cw && y >= 0 && y + h <= ch;
    }

    // прямоугольник
    public class MyRectangle : Shape
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public MyRectangle(double x, double y, double w, double h) : base(x, y)
        {
            Width = w;
            Height = h;
        }

        public override void Draw(DrawingContext dc)
        {
            var rect = new Rect(Location.X, Location.Y, Width, Height);
            var brush = IsSelected
                ? new SolidColorBrush(Color.FromArgb(50, Color.R, Color.G, Color.B))
                : Brushes.Transparent;
            var pen = new Pen(new SolidColorBrush(Color), IsSelected ? 3 : 1);

            dc.DrawRectangle(brush, pen, rect);
            DrawSelectionBorder(dc, rect);
        }

        public override bool HitTest(Point point)
            => point.X >= Location.X && point.X <= Location.X + Width &&
               point.Y >= Location.Y && point.Y <= Location.Y + Height;

        public override void Move(double dx, double dy, double cw, double ch)
        {
            double newX = Location.X + dx;
            double newY = Location.Y + dy;
            if (CheckBounds(newX, newY, Width, Height, cw, ch))
                Location = new Point(newX, newY);
            else
                throw new InvalidOperationException("Выход за границы");
        }

        public void Resize(double newW, double newH, double cw, double ch)
        {
            if (newW > 0 && newH > 0 && CheckBounds(Location.X, Location.Y, newW, newH, cw, ch))
            {
                Width = newW;
                Height = newH;
            }
            else throw new ArgumentException("Недопустимый размер");
        }
    }

    //  квадрат
    public class MySquare : MyRectangle
    {
        public double Side { get => Width; set { Width = value; Height = value; } }

        public MySquare(double x, double y, double side) : base(x, y, side, side) { }

        public void SetSide(double newSide, double cw, double ch)
        {
            if (newSide > 0 && CheckBounds(Location.X, Location.Y, newSide, newSide, cw, ch))
                Side = newSide;
            else throw new ArgumentException("Недопустимый размер");
        }
    }

    // эллипс
    public class MyEllipse : Shape
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public MyEllipse(double x, double y, double w, double h) : base(x, y)
        {
            Width = w;
            Height = h;
        }

        public override void Draw(DrawingContext dc)
        {
            var brush = IsSelected
                ? new SolidColorBrush(Color.FromArgb(50, Color.R, Color.G, Color.B))
                : Brushes.Transparent;
            var pen = new Pen(new SolidColorBrush(Color), IsSelected ? 3 : 1);

            dc.DrawEllipse(brush, pen,
                new Point(Location.X + Width / 2, Location.Y + Height / 2),
                Width / 2, Height / 2);

            DrawSelectionBorder(dc, new Rect(Location.X, Location.Y, Width, Height));
        }

        public override bool HitTest(Point point)
            => point.X >= Location.X && point.X <= Location.X + Width &&
               point.Y >= Location.Y && point.Y <= Location.Y + Height;

        public override void Move(double dx, double dy, double cw, double ch)
        {
            double newX = Location.X + dx;
            double newY = Location.Y + dy;
            if (CheckBounds(newX, newY, Width, Height, cw, ch))
                Location = new Point(newX, newY);
            else throw new InvalidOperationException("Выход за границы");
        }
    }

    // круг
    public class MyCircle : MyEllipse
    {
        public double Radius { get => Width / 2; set { Width = value * 2; Height = value * 2; } }

        public MyCircle(double x, double y, double r) : base(x, y, r * 2, r * 2) { }

        public void SetRadius(double newR, double cw, double ch)
        {
            if (newR > 0 && CheckBounds(Location.X, Location.Y, newR * 2, newR * 2, cw, ch))
                Radius = newR;
            else throw new ArgumentException("Недопустимый радиус");
        }
    }

    //треугольник
    public class MyTriangle : Shape
    {
        public Point[] Points { get; private set; }

        public MyTriangle(Point p1, Point p2, Point p3) : base(p1.X, p1.Y)
        {
            Points = new[] { p1, p2, p3 };
        }

        public override void Draw(DrawingContext dc)
        {
            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(Points[0], true, true);
                ctx.LineTo(Points[1], true, false);
                ctx.LineTo(Points[2], true, false);
            }
            geometry.Freeze();

            var brush = IsSelected
                ? new SolidColorBrush(Color.FromArgb(50, Color.R, Color.G, Color.B))
                : Brushes.Transparent;
            var pen = new Pen(new SolidColorBrush(Color), IsSelected ? 3 : 1);

            dc.DrawGeometry(brush, pen, geometry);

            double minX = Math.Min(Points[0].X, Math.Min(Points[1].X, Points[2].X));
            double minY = Math.Min(Points[0].Y, Math.Min(Points[1].Y, Points[2].Y));
            double maxX = Math.Max(Points[0].X, Math.Max(Points[1].X, Points[2].X));
            double maxY = Math.Max(Points[0].Y, Math.Max(Points[1].Y, Points[2].Y));
            DrawSelectionBorder(dc, new Rect(minX, minY, maxX - minX, maxY - minY));
        }

        public override bool HitTest(Point point)
        {
            double minX = Math.Min(Points[0].X, Math.Min(Points[1].X, Points[2].X)) - 5;
            double maxX = Math.Max(Points[0].X, Math.Max(Points[1].X, Points[2].X)) + 5;
            double minY = Math.Min(Points[0].Y, Math.Min(Points[1].Y, Points[2].Y)) - 5;
            double maxY = Math.Max(Points[0].Y, Math.Max(Points[1].Y, Points[2].Y)) + 5;
            return point.X >= minX && point.X <= maxX && point.Y >= minY && point.Y <= maxY;
        }

        public override void Move(double dx, double dy, double cw, double ch)
        {
            for (int i = 0; i < Points.Length; i++)
            {
                double newX = Points[i].X + dx;
                double newY = Points[i].Y + dy;
                if (newX < 0 || newX > cw || newY < 0 || newY > ch)
                    throw new InvalidOperationException("Выход за границы");
            }

            for (int i = 0; i < Points.Length; i++)
                Points[i] = new Point(Points[i].X + dx, Points[i].Y + dy);
            Location = Points[0];
        }
    }

    // машина
    public class Car : Shape
    {
        private List<Shape> parts = new List<Shape>();

        public Car(double x, double y) : base(x, y)
        {
            BuildCar(x, y);
        }

        private void BuildCar(double x, double y)
        {
            parts.Add(new MyRectangle(x + 30, y + 50, 140, 50) { Color = Colors.Blue });      // кузов
            parts.Add(new MyRectangle(x + 60, y + 20, 80, 30) { Color = Colors.Red });        // крыша
            parts.Add(new MySquare(x + 70, y + 25, 20) { Color = Colors.Cyan });              // окно 1
            parts.Add(new MySquare(x + 110, y + 25, 20) { Color = Colors.Cyan });             // окно 2
            parts.Add(new MyCircle(x + 50, y + 85, 15) { Color = Colors.Black });             // колесо 1
            parts.Add(new MyCircle(x + 130, y + 85, 15) { Color = Colors.Black });            // колесо 2
            parts.Add(new MyTriangle(new Point(x + 160, y + 60), new Point(x + 170, y + 70), new Point(x + 160, y + 70)) { Color = Colors.Yellow }); // фара 1
            parts.Add(new MyTriangle(new Point(x + 30, y + 60), new Point(x + 20, y + 70), new Point(x + 30, y + 70)) { Color = Colors.Yellow });    // фара 2
        }

        public override void Draw(DrawingContext dc)
        {
            foreach (var part in parts)
            {
                part.Color = this.Color;
                part.IsSelected = this.IsSelected;
                part.Draw(dc);
            }
        }

        public override bool HitTest(Point point)
        {
            foreach (var part in parts)
                if (part.HitTest(point)) return true;
            return false;
        }

        public override void Move(double dx, double dy, double cw, double ch)
        {
            // Проверка возможности перемещения
            foreach (var part in parts)
            {
                if (part is MyRectangle r)
                {
                    if (r.Location.X + dx < 0 || r.Location.X + dx + r.Width > cw ||
                        r.Location.Y + dy < 0 || r.Location.Y + dy + r.Height > ch)
                        throw new InvalidOperationException("Выход за границы");
                }
                else if (part is MyCircle c)
                {
                    if (c.Location.X + dx < 0 || c.Location.X + dx + c.Width > cw ||
                        c.Location.Y + dy < 0 || c.Location.Y + dy + c.Height > ch)
                        throw new InvalidOperationException("Выход за границы");
                }
                else if (part is MyTriangle t)
                {
                    foreach (var p in t.Points)
                        if (p.X + dx < 0 || p.X + dx > cw || p.Y + dy < 0 || p.Y + dy > ch)
                            throw new InvalidOperationException("Выход за границы");
                }
            }

            // Перемещение
            foreach (var part in parts)
            {
                if (part is MyRectangle r)
                    r.Location = new Point(r.Location.X + dx, r.Location.Y + dy);
                else if (part is MyCircle c)
                    c.Location = new Point(c.Location.X + dx, c.Location.Y + dy);
                else if (part is MyTriangle t)
                {
                    for (int i = 0; i < t.Points.Length; i++)
                        t.Points[i] = new Point(t.Points[i].X + dx, t.Points[i].Y + dy);
                }
            }
            Location = new Point(Location.X + dx, Location.Y + dy);
        }

        public List<MyCircle> GetWheels()
        {
            var wheels = new List<MyCircle>();
            foreach (var part in parts)
                if (part is MyCircle c) wheels.Add(c);
            return wheels;
        }

        public MyRectangle GetBody()
        {
            foreach (var part in parts)
                if (part is MyRectangle r && r.Width > 100) return r;
            return null;
        }
    }
}