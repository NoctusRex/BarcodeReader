using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Rectangle = System.Drawing.Rectangle;

namespace BarcodeReader.Windows
{
    /// <summary>
    /// Interaktionslogik für ScreenShotWindow.xaml
    /// </summary>
    public partial class ScreenShotWindow : Window
    {

        public Bitmap Screenshot { get; set; }
        private System.Drawing.Point X { get; set; }
        private System.Drawing.Point Y { get; set; }

        private int GetAreaWidth { get => Math.Abs(X.X - Y.X); }
        private int GetAreaHeight { get => Math.Abs(X.Y - Y.Y); }

        public ScreenShotWindow()
        {
            InitializeComponent();
        }

        public bool? TakeScreenshot()
        {
            return ShowDialog();
        }

        private void RefreshRectangleSelection()
        {
            if (X == System.Drawing.Point.Empty || Y == System.Drawing.Point.Empty) return;

            GuiCanvas.Children.Clear();

            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
            {
                Stroke = new SolidColorBrush(Colors.Red),
                Fill = new SolidColorBrush(Colors.Transparent),
                Width = GetAreaWidth,
                Height = GetAreaHeight,
                StrokeThickness = 3
            };

            // -1 because mouse is on rectangle and does not trigger mouse up event
            Canvas.SetLeft(rect, X.X - 1);
            Canvas.SetTop(rect, X.Y - 1);

            GuiCanvas.Children.Add(rect);
        }

        private System.Drawing.Point ConvertPointToPoint(System.Windows.Point point)
        {
            return new System.Drawing.Point((int)point.X, (int)point.Y);
        }

        private void CaptureScreenArea()
        {
            Rectangle temp = new Rectangle(X.X, X.Y, GetAreaWidth, GetAreaHeight);
            Screenshot = new Bitmap(GetAreaWidth, GetAreaHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(Screenshot))
            {
                g.CopyFromScreen(temp.Left, temp.Top, 0, 0, Screenshot.Size);
            }
        }

        private void GuiCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Y = ConvertPointToPoint(e.GetPosition(this));
            RefreshRectangleSelection();
        }

        private void GuiCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            X = ConvertPointToPoint(e.GetPosition(this));
        }

        private void GuiCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DialogResult = true;

            Hide();

            CaptureScreenArea();

            Close();
        }

    }
}
