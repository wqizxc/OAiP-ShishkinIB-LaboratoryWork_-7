using System;
using System.Collections.Generic;

namespace Lab7
{
    public static class ShapeContainer
    {
        public static List<Figure> Figures { get; } = new List<Figure>();
        public static double CanvasWidth { get; set; } = 800;
        public static double CanvasHeight { get; set; } = 400;

        public static void AddFigure(Figure figure) => Figures.Add(figure);
        public static void RemoveFigure(Figure figure) => Figures.Remove(figure);
        public static void Clear() => Figures.Clear();

        public static void MoveSelectedFigure(int index, double deltaX, double deltaY)
        {
            if (index < 0 || index >= Figures.Count) return;

            var figure = Figures[index];
            var bounds = figure.GetBounds();
            double newX = figure.X + deltaX;
            double newY = figure.Y + deltaY;
            double offsetX = bounds.Left - figure.X;
            double offsetY = bounds.Top - figure.Y;
            if (newX + offsetX < 0)
            {
                newX = -offsetX;
            }

            if (newY + offsetY < 0)
            {
                newY = -offsetY;
            }
            if (newX + offsetX + bounds.Width > CanvasWidth)
            {
                newX = CanvasWidth - offsetX - bounds.Width;
            }

            if (newY + offsetY + bounds.Height > CanvasHeight)
            {
                newY = CanvasHeight - offsetY - bounds.Height;
            }
            figure.Move(newX - figure.X, newY - figure.Y);
        }

        public static Figure? GetFigure(int index)
        {
            return index >= 0 && index < Figures.Count ? Figures[index] : null;
        }
    }
}