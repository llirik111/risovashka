using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private Point startPoint;
        private bool isDrawing = false;
        private string currentTool = "Pencil";
        private Brush currentColor = Brushes.Black;
        private Shape currentShape;

        public MainWindow()
        {
            InitializeComponent();
            StatusText.Text = $"Выбран инструмент: {currentTool}";
        }

        private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            isDrawing = true;
            startPoint = e.GetPosition(DrawingCanvas);

            switch (currentTool)
            {
                case "Pencil":
                    break;
                case "Line":
                    currentShape = new Line
                    {
                        Stroke = currentColor,
                        StrokeThickness = 2,
                        X1 = startPoint.X,
                        Y1 = startPoint.Y,
                        X2 = startPoint.X,
                        Y2 = startPoint.Y
                    };
                    DrawingCanvas.Children.Add(currentShape);
                    break;
                case "Rectangle":
                    currentShape = new Rectangle
                    {
                        Stroke = currentColor,
                        StrokeThickness = 2,
                        Fill = Brushes.Transparent
                    };
                    Canvas.SetLeft(currentShape, startPoint.X);
                    Canvas.SetTop(currentShape, startPoint.Y);
                    DrawingCanvas.Children.Add(currentShape);
                    break;
                case "Ellipse":
                    currentShape = new Ellipse
                    {
                        Stroke = currentColor,
                        StrokeThickness = 2,
                        Fill = Brushes.Transparent
                    };
                    Canvas.SetLeft(currentShape, startPoint.X);
                    Canvas.SetTop(currentShape, startPoint.Y);
                    DrawingCanvas.Children.Add(currentShape);
                    break;
            }
        }

        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;

            var currentPoint = e.GetPosition(DrawingCanvas);

            switch (currentTool)
            {
                case "Pencil":
                    var line = new Line
                    {
                        Stroke = currentColor,
                        StrokeThickness = 2,
                        X1 = startPoint.X,
                        Y1 = startPoint.Y,
                        X2 = currentPoint.X,
                        Y2 = currentPoint.Y
                    };
                    DrawingCanvas.Children.Add(line);
                    startPoint = currentPoint;
                    break;

                case "Line":
                    if (currentShape is Line lineShape)
                    {
                        lineShape.X2 = currentPoint.X;
                        lineShape.Y2 = currentPoint.Y;
                    }
                    break;

                case "Rectangle":
                    if (currentShape is Rectangle rect)
                    {
                        double width = currentPoint.X - startPoint.X;
                        double height = currentPoint.Y - startPoint.Y;

                        rect.Width = Math.Abs(width);
                        rect.Height = Math.Abs(height);

                        if (width < 0) Canvas.SetLeft(rect, currentPoint.X);
                        if (height < 0) Canvas.SetTop(rect, currentPoint.Y);
                    }
                    break;

                case "Ellipse":
                    if (currentShape is Ellipse ellipse)
                    {
                        double width = currentPoint.X - startPoint.X;
                        double height = currentPoint.Y - startPoint.Y;

                        ellipse.Width = Math.Abs(width);
                        ellipse.Height = Math.Abs(height);

                        if (width < 0) Canvas.SetLeft(ellipse, currentPoint.X);
                        if (height < 0) Canvas.SetTop(ellipse, currentPoint.Y);
                    }
                    break;
            }
        }

        private void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
            currentShape = null;
        }

        private void PencilButton_Click(object sender, RoutedEventArgs e)
        {
            currentTool = "Pencil";
            StatusText.Text = "Выбран инструмент: Карандаш";
        }

        private void LineButton_Click(object sender, RoutedEventArgs e)
        {
            currentTool = "Line";
            StatusText.Text = "Выбран инструмент: Линия";
        }

        private void RectangleButton_Click(object sender, RoutedEventArgs e)
        {
            currentTool = "Rectangle";
            StatusText.Text = "Выбран инструмент: Квадрат";
        }

        private void EllipseButton_Click(object sender, RoutedEventArgs e)
        {
            currentTool = "Ellipse";
            StatusText.Text = "Выбран инструмент: Эллипс";
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            var colorMenu = new ContextMenu();

            var colors = new[]
            {
        ("Черный", Colors.Black),
        ("Красный", Colors.Red),
        ("Зеленый", Colors.Green),
        ("Синий", Colors.Blue),
        ("Желтый", Colors.Yellow),
        ("Фиолетовый", Colors.Purple),
        ("Оранжевый", Colors.Orange)
    };

            foreach (var (name, color) in colors)
            {
                var menuItem = new MenuItem
                {
                    Header = name,
                    Background = new SolidColorBrush(color)
                };
                menuItem.Click += (s, args) =>
                {
                    currentColor = new SolidColorBrush(color);
                    ColorButton.Background = currentColor;
                };
                colorMenu.Items.Add(menuItem);
            }

            colorMenu.IsOpen = true;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Children.Clear();
        }

        private void ClearMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Children.Clear();
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png|JPEG Image|*.jpg",
                Title = "Сохранить рисунок"
            };

            if (saveDialog.ShowDialog() == true)
            {
                SaveCanvasToFile(saveDialog.FileName);
            }
        }

        private void SaveCanvasToFile(string filename)
        {
            try
            {
                var renderBitmap = new RenderTargetBitmap(
                    (int)DrawingCanvas.ActualWidth,
                    (int)DrawingCanvas.ActualHeight,
                    96d, 96d, PixelFormats.Pbgra32);

                renderBitmap.Render(DrawingCanvas);

                BitmapEncoder encoder;

                if (filename.ToLower().EndsWith(".jpg") || filename.ToLower().EndsWith(".jpeg"))
                {
                    encoder = new JpegBitmapEncoder();
                }
                else
                {
                    encoder = new PngBitmapEncoder();
                }

                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                using (var fileStream = new FileStream(filename, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                MessageBox.Show("Рисунок сохранен!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}