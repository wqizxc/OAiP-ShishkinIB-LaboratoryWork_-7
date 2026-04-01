using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Lab7
{
    public class Car : Figure
    {
        private List<Figure> _components;

        public Car(double x, double y, double width, double height) : base(x, y)
        {
            _components = new List<Figure>();
            BuildCar(x, y, width, height);
        }

        private void BuildCar(double startX, double startY, double totalWidth, double totalHeight)
        {
            double bodyHeight = totalHeight * 0.45;
            double roofHeight = totalHeight * 0.35;
            double wheelRadius = totalHeight * 0.16;
            double windowWidth = totalWidth * 0.22;
            double windowHeight = roofHeight * 0.55;
            double lightSize = totalHeight * 0.15;
            double lightWidth = totalHeight * 0.2;

            double bodyY = startY + roofHeight;

            double bodyWidth = totalWidth + 10;
            double bodyX = startX - 5;
            _components.Add(new Rectangle(bodyX, bodyY, bodyWidth, bodyHeight));

            double roofWidth = totalWidth * 0.60;
            double roofX = startX + (totalWidth - roofWidth) / 2;
            _components.Add(new Rectangle(roofX, startY, roofWidth, roofHeight));

            double windowY = startY + roofHeight * 0.25;
            double windowSpacing = (roofWidth - windowWidth * 2) / 3;
            double firstWindowX = roofX + windowSpacing;
            double secondWindowX = roofX + windowSpacing * 2 + windowWidth;

            _components.Add(new Rectangle(firstWindowX, windowY, windowWidth, windowHeight));
            _components.Add(new Rectangle(secondWindowX, windowY, windowWidth, windowHeight));

            double wheelOffset = wheelRadius * 0.5;
            double wheelY = bodyY + bodyHeight - wheelRadius;
            double leftWheelX = bodyX + wheelOffset;
            double rightWheelX = bodyX + bodyWidth - wheelRadius * 2 - wheelOffset;

            _components.Add(new Circle(leftWheelX, wheelY, wheelRadius));
            _components.Add(new Circle(rightWheelX, wheelY, wheelRadius));
            double lightY = bodyY + bodyHeight * 0.25 - lightSize / 2; 

            var frontLightPoints = new List<Point>
            {
                new Point(startX + totalWidth + lightWidth, lightY),
                new Point(startX + totalWidth + lightWidth, lightY + lightSize),
                new Point(startX + totalWidth, lightY + lightSize / 2)
            };
            _components.Add(new Triangle(startX + totalWidth, lightY, frontLightPoints));
            var rearLightPoints = new List<Point>
            {
                new Point(startX - lightWidth, lightY),
                new Point(startX - lightWidth, lightY + lightSize),
                new Point(startX, lightY + lightSize / 2)
            };
            _components.Add(new Triangle(startX - lightWidth, lightY, rearLightPoints));
        }

        public override void Draw(DrawingContext dc)
        {
            foreach (var component in _components)
            {
                component.Draw(dc);
            }
        }

        public override void Move(double dx, double dy)
        {
            X += dx;
            Y += dy;
            foreach (var component in _components)
            {
                component.Move(dx, dy);
            }
        }

        public override double GetWidth()
        {
            if (_components.Count == 0) return 0;
            double maxX = double.MinValue;
            double minX = double.MaxValue;

            foreach (var component in _components)
            {
                var bounds = component.GetBounds();
                minX = Math.Min(minX, bounds.Left);
                maxX = Math.Max(maxX, bounds.Right);
            }
            return maxX - minX;
        }

        public override double GetHeight()
        {
            if (_components.Count == 0) return 0;
            double maxY = double.MinValue;
            double minY = double.MaxValue;

            foreach (var component in _components)
            {
                var bounds = component.GetBounds();
                minY = Math.Min(minY, bounds.Top);
                maxY = Math.Max(maxY, bounds.Bottom);
            }
            return maxY - minY;
        }

        public override Rect GetBounds()
        {
            if (_components.Count == 0) return new Rect(X, Y, 0, 0);
            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            foreach (var component in _components)
            {
                var bounds = component.GetBounds();
                minX = Math.Min(minX, bounds.Left);
                minY = Math.Min(minY, bounds.Top);
                maxX = Math.Max(maxX, bounds.Right);
                maxY = Math.Max(maxY, bounds.Bottom);
            }
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }
    }
}