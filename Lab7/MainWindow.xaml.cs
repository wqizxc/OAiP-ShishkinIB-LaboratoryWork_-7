using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lab7
{
    public partial class MainWindow : Window
    {
        private RenderTargetBitmap? _renderTarget;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int width = (int)Math.Max(DrawingImage.ActualWidth, 800);
            int height = (int)Math.Max(DrawingImage.ActualHeight, 400);

            ShapeContainer.CanvasWidth = width;
            ShapeContainer.CanvasHeight = height;

            _renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            DrawingImage.Source = _renderTarget;
            Redraw();
        }

        private void Redraw()
        {
            if (_renderTarget == null) return;

            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                context.DrawRectangle(null, new Pen(Brushes.Red, 1),
                    new Rect(0, 0, ShapeContainer.CanvasWidth, ShapeContainer.CanvasHeight));

                foreach (var figure in ShapeContainer.Figures)
                {
                    figure.Draw(context);
                }
            }

            _renderTarget.Clear();
            _renderTarget.Render(visual);
            DrawingImage.Source = _renderTarget;
            UpdateFigureList();
        }

        private void UpdateFigureList()
        {
            LstFigures.Items.Clear();
            for (int i = 0; i < ShapeContainer.Figures.Count; i++)
            {
                var figure = ShapeContainer.Figures[i];
                LstFigures.Items.Add($"#{i + 1} {figure.GetType().Name} ({figure.X:F1}, {figure.Y:F1})");
            }
        }

        private double ParseDouble(TextBox textBox)
        {
            try
            {
                return double.Parse(textBox.Text.Replace('.', ','));
            }
            catch
            {
                return 0;
            }
        }

        private List<Point> ParsePoints(string text)
        {
            var points = new List<Point>();
            var parts = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var coords = part.Split(new[] { ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (coords.Length >= 2)
                {
                    try
                    {
                        double x = double.Parse(coords[0].Replace('.', ','));
                        double y = double.Parse(coords[1].Replace('.', ','));
                        points.Add(new Point(x, y));
                    }
                    catch { }
                }
            }
            return points;
        }

        private bool CheckFitOnCanvas(double x, double y, double width, double height)
        {
            if (x < 0 || y < 0)
            {
                MessageBox.Show("Фигура не может быть размещена за пределами холста!",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (x + width > ShapeContainer.CanvasWidth || y + height > ShapeContainer.CanvasHeight)
            {
                MessageBox.Show($"Фигура не помещается на холсте! Максимальные координаты: ({ShapeContainer.CanvasWidth - width:F0}, {ShapeContainer.CanvasHeight - height:F0})",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (width > ShapeContainer.CanvasWidth || height > ShapeContainer.CanvasHeight)
            {
                MessageBox.Show($"Фигура слишком большая! Максимум: {ShapeContainer.CanvasWidth}x{ShapeContainer.CanvasHeight}",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9,.-]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PolygonPointsValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9,. -]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void AddRectangle_Click(object sender, RoutedEventArgs e)
        {
            double x = ParseDouble(TxtX);
            double y = ParseDouble(TxtY);
            double width = Math.Max(10, ParseDouble(TxtSize));
            double height = Math.Max(10, ParseDouble(TxtHeight));

            if (!CheckFitOnCanvas(x, y, width, height)) return;

            ShapeContainer.AddFigure(new Rectangle(x, y, width, height));
            Redraw();
        }

        private void AddSquare_Click(object sender, RoutedEventArgs e)
        {
            double x = ParseDouble(TxtX);
            double y = ParseDouble(TxtY);
            double side = Math.Max(10, ParseDouble(TxtSize));

            if (!CheckFitOnCanvas(x, y, side, side)) return;

            ShapeContainer.AddFigure(new Square(x, y, side));
            Redraw();
        }

        private void AddEllipse_Click(object sender, RoutedEventArgs e)
        {
            double x = ParseDouble(TxtX);
            double y = ParseDouble(TxtY);
            double width = Math.Max(10, ParseDouble(TxtSize));
            double height = Math.Max(10, ParseDouble(TxtHeight));

            if (!CheckFitOnCanvas(x, y, width, height)) return;

            ShapeContainer.AddFigure(new Ellipse(x, y, width, height));
            Redraw();
        }

        private void AddCircle_Click(object sender, RoutedEventArgs e)
        {
            double x = ParseDouble(TxtX);
            double y = ParseDouble(TxtY);
            double radius = Math.Max(5, ParseDouble(TxtSize));

            if (!CheckFitOnCanvas(x, y, radius * 2, radius * 2)) return;

            ShapeContainer.AddFigure(new Circle(x, y, radius));
            Redraw();
        }

        private void AddTriangle_Click(object sender, RoutedEventArgs e)
        {
            double x = ParseDouble(TxtX);
            double y = ParseDouble(TxtY);

            var points = new List<Point>
            {
                new Point(ParseDouble(TxtT1X), ParseDouble(TxtT1Y)),
                new Point(ParseDouble(TxtT2X), ParseDouble(TxtT2Y)),
                new Point(ParseDouble(TxtT3X), ParseDouble(TxtT3Y))
            };

            double minX = points.Min(p => p.X);
            double maxX = points.Max(p => p.X);
            double minY = points.Min(p => p.Y);
            double maxY = points.Max(p => p.Y);

            double triangleWidth = maxX - minX;
            double triangleHeight = maxY - minY;

            if (!CheckFitOnCanvas(x + minX, y + minY, triangleWidth, triangleHeight)) return;

            ShapeContainer.AddFigure(new Triangle(x, y, points));
            Redraw();
        }

        private void AddPolygon_Click(object sender, RoutedEventArgs e)
        {
            double x = ParseDouble(TxtX);
            double y = ParseDouble(TxtY);
            var points = ParsePoints(TxtPolyPoints.Text);

            if (points.Count < 3)
            {
                MessageBox.Show("Введите минимум 3 вершины! Формат: x1,y1 x2,y2 x3,y3...",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double minX = points.Min(p => p.X);
            double maxX = points.Max(p => p.X);
            double minY = points.Min(p => p.Y);
            double maxY = points.Max(p => p.Y);

            double polygonWidth = maxX - minX;
            double polygonHeight = maxY - minY;

            if (!CheckFitOnCanvas(x + minX, y + minY, polygonWidth, polygonHeight)) return;

            ShapeContainer.AddFigure(new Polygon(x, y, points));
            Redraw();
        }

        private void AddCar_Click(object sender, RoutedEventArgs e)
        {
            double x = ParseDouble(TxtX);
            double y = ParseDouble(TxtY);
            double width = Math.Max(80, ParseDouble(TxtSize));
            double height = Math.Max(60, ParseDouble(TxtHeight));

            if (!CheckFitOnCanvas(x, y, width, height)) return;

            ShapeContainer.AddFigure(new Car(x, y, width, height));
            Redraw();
        }

        private void DeleteFigure_Click(object sender, RoutedEventArgs e)
        {
            if (LstFigures.SelectedIndex >= 0 && LstFigures.SelectedIndex < ShapeContainer.Figures.Count)
            {
                ShapeContainer.RemoveFigure(ShapeContainer.Figures[LstFigures.SelectedIndex]);
                Redraw();
            }
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            ShapeContainer.Clear();
            Redraw();
        }

        private void ChangeRadius_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNewRadius.Text))
            {
                MessageBox.Show("Введите новый радиус", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (LstFigures.SelectedIndex >= 0 && ShapeContainer.Figures[LstFigures.SelectedIndex] is Circle circle)
            {
                double newRadius = ParseDouble(TxtNewRadius);
                double maxRadius = Math.Min(ShapeContainer.CanvasWidth - circle.X, ShapeContainer.CanvasHeight - circle.Y) / 2;
                newRadius = Math.Max(1, Math.Min(newRadius, maxRadius));
                circle.ChangeRadius(newRadius);
                Redraw();
            }
            else
            {
                MessageBox.Show("Выберите окружность", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ResizeRectangle_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNewWidth.Text) || string.IsNullOrWhiteSpace(TxtNewHeight.Text))
            {
                MessageBox.Show("Введите ширину и высоту", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (LstFigures.SelectedIndex >= 0 && ShapeContainer.Figures[LstFigures.SelectedIndex] is Rectangle rectangle)
            {
                double newWidth = Math.Max(10, ParseDouble(TxtNewWidth));
                double newHeight = Math.Max(10, ParseDouble(TxtNewHeight));

                double maxWidth = ShapeContainer.CanvasWidth - rectangle.X;
                double maxHeight = ShapeContainer.CanvasHeight - rectangle.Y;

                newWidth = Math.Min(newWidth, maxWidth);
                newHeight = Math.Min(newHeight, maxHeight);

                rectangle.Resize(newWidth, newHeight);
                Redraw();
            }
            else
            {
                MessageBox.Show("Выберите прямоугольник", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void MoveFigure_Click(object sender, RoutedEventArgs e)
        {
            if (LstFigures.SelectedIndex < 0)
            {
                MessageBox.Show("Выберите фигуру для перемещения", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double deltaX = ParseDouble(TxtDeltaX);
            double deltaY = ParseDouble(TxtDeltaY);

            var figure = ShapeContainer.Figures[LstFigures.SelectedIndex];
            double oldX = figure.X;
            double oldY = figure.Y;

            ShapeContainer.MoveSelectedFigure(LstFigures.SelectedIndex, deltaX, deltaY);

            if (Math.Abs(figure.X - oldX) < 0.001 && Math.Abs(figure.Y - oldY) < 0.001 && (Math.Abs(deltaX) > 0 || Math.Abs(deltaY) > 0))
            {
                MessageBox.Show("Невозможно переместить фигуру: она достигла границы холста!",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            Redraw();
        }
    }
}