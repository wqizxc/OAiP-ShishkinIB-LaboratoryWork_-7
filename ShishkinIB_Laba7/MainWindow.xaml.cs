using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GraphicsAppWPF
{
    public partial class MainWindow : Window
    {
        private List<Shape> shapes = new List<Shape>();
        private Shape selectedShape;
        private Point dragStart;
        private bool isDragging;

        public MainWindow()
        {
            InitializeComponent();
            DrawingCanvas.Loaded += DrawingCanvas_Loaded;
        }

        private void DrawingCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            Redraw();
        }

        private void Redraw()
        {
            if (DrawingCanvas.ActualWidth <= 0 || DrawingCanvas.ActualHeight <= 0)
                return;
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                dc.DrawRectangle(Brushes.White, null,
                    new Rect(0, 0, DrawingCanvas.ActualWidth, DrawingCanvas.ActualHeight));
                foreach (var shape in shapes)
                {
                    shape.Draw(dc);
                }
            }
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)DrawingCanvas.ActualWidth,
                (int)DrawingCanvas.ActualHeight,
                96, 96, PixelFormats.Pbgra32);

            rtb.Render(visual);
            DrawingCanvas.Children.Clear();
            Image img = new Image();
            img.Source = rtb;
            DrawingCanvas.Children.Add(img);
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var pos = e.GetPosition(DrawingCanvas);
                foreach (var s in shapes) s.IsSelected = false;

                selectedShape = null;
                for (int i = shapes.Count - 1; i >= 0; i--)
                {
                    if (shapes[i].HitTest(pos))
                    {
                        selectedShape = shapes[i];
                        selectedShape.IsSelected = true;
                        break;
                    }
                }

                if (selectedShape != null)
                {
                    dragStart = pos;
                    isDragging = true;
                    txtStatus.Text = $"Выбрано: {selectedShape.GetType().Name}";
                }

                Redraw();
            }
            catch (Exception ex)
            {
                ShowError("Ошибка выбора", ex);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var pos = e.GetPosition(DrawingCanvas);
                txtCoordinates.Text = $"X:{pos.X:F0} Y:{pos.Y:F0}";

                if (isDragging && selectedShape != null && e.LeftButton == MouseButtonState.Pressed)
                {
                    double dx = pos.X - dragStart.X;
                    double dy = pos.Y - dragStart.Y;

                    selectedShape.Move(dx, dy, DrawingCanvas.ActualWidth, DrawingCanvas.ActualHeight);
                    dragStart = pos;
                    Redraw();
                }
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception ex)
            {
                ShowError("Ошибка перемещения", ex);
                isDragging = false;
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
        }

        private void AddCar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var car = new Car(200, 150) { Color = GetSelectedColor() };
                shapes.Add(car);
                Redraw();
                txtStatus.Text = "Машина добавлена";
            }
            catch (Exception ex) { ShowError("Ошибка добавления машины", ex); }
        }

        private void AddCircle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int size = int.Parse((cmbSize.SelectedItem as ComboBoxItem).Content.ToString());
                var circle = new MyCircle(200, 150, size * 8) { Color = GetSelectedColor() };
                shapes.Add(circle);
                Redraw();
                txtStatus.Text = "Круг добавлен";
            }
            catch (Exception ex) { ShowError("Ошибка добавления круга", ex); }
        }

        private void AddSquare_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int size = int.Parse((cmbSize.SelectedItem as ComboBoxItem).Content.ToString());
                var square = new MySquare(200, 150, size * 15) { Color = GetSelectedColor() };
                shapes.Add(square);
                Redraw();
                txtStatus.Text = "Квадрат добавлен";
            }
            catch (Exception ex) { ShowError("Ошибка добавления квадрата", ex); }
        }

        private void AddTriangle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var triangle = new MyTriangle(
                    new Point(200, 150),
                    new Point(250, 200),
                    new Point(150, 200)
                )
                { Color = GetSelectedColor() };
                shapes.Add(triangle);
                Redraw();
                txtStatus.Text = "Треугольник добавлен";
            }
            catch (Exception ex) { ShowError("Ошибка добавления треугольника", ex); }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedShape != null)
                {
                    shapes.Remove(selectedShape);
                    selectedShape = null;
                    Redraw();
                    txtStatus.Text = "Фигура удалена";
                }
                else MessageBox.Show("Фигура не выбрана!", "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { ShowError("Ошибка удаления", ex); }
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить все фигуры?", "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                shapes.Clear();
                selectedShape = null;
                Redraw();
                txtStatus.Text = "Все фигуры удалены";
            }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            shapes.Clear();
            selectedShape = null;
            Redraw();
            txtStatus.Text = "Новый файл";
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Close();

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Графический редактор\nВерсия 2.0\nРисование без использования встроенных фигур WPF\nИспользуется Geometry и DrawingContext",
                "О программе", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private Color GetSelectedColor()
        {
            var colorItem = cmbColor.SelectedItem as ComboBoxItem;
            return (Color)ColorConverter.ConvertFromString(colorItem.Tag.ToString());
        }

        private void ShowError(string message, Exception ex)
        {
            MessageBox.Show($"{message}: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}