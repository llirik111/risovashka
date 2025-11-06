using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private Point startPoint;
        private bool isDrawing = false;
        private string currentTool = "Pencil";
        private Brush currentColor = Brushes.Black;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            isDrawing = true;
            startPoint = e.GetPosition(DrawingCanvas);
        }

        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing || currentTool != "Pencil") return;

            var currentPoint = e.GetPosition(DrawingCanvas);

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
        }

        private void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
        }
    }
}