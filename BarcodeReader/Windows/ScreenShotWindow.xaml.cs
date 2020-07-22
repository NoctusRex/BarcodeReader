using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
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
        /// <summary>
        /// For CopyFromScreen the point of origin is 0,0 of the primary display.
        /// There for if the primary screen is not the first, the X coordinate has to be moved
        /// </summary>
        private int XOffset { get; set; } = 0;
        private bool Shown { get; set; } = false;

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
            rect.MouseLeftButtonUp += GuiCanvas_MouseLeftButtonUp;
            rect.KeyDown += GuiCanvas_KeyDown;

            Canvas.SetLeft(rect, X.X);
            Canvas.SetTop(rect, X.Y);
            
            GuiCanvas.Children.Add(rect);
        }

        private System.Drawing.Point ConvertPointToPoint(System.Windows.Point point)
        {
            return new System.Drawing.Point((int)point.X, (int)point.Y);
        }

        private void CaptureScreenArea()
        {
            Rectangle temp = new Rectangle(X.X - XOffset, X.Y, GetAreaWidth, GetAreaHeight);
            Screenshot = new Bitmap(GetAreaWidth, GetAreaHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(Screenshot))
            {
                g.CopyFromScreen(temp.Left, temp.Top, 0, 0, Screenshot.Size);
            }
           // Screenshot.Save("C:\\temp\\" + DateTime.Now.Ticks + "-" + X.X + "-" + X.Y + " _ "+ temp.X + "-" + temp.Y + ".png", ImageFormat.Png);
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

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (Shown) return;
            Shown = true;

            // make window span over all screens
            Rectangle totalSize = Rectangle.Empty;
            XOffset = 0;
            foreach (Screen screen in Screen.AllScreens)
            {
                // if bounds smaller 0 the screens are left next to primary screen
                if (screen.Bounds.X < 0 )
                    if(screen.Bounds.X < XOffset) XOffset = screen.Bounds.X;

                totalSize = Rectangle.Union(totalSize, screen.Bounds);
            }

            XOffset = Math.Abs(XOffset);

            Width = totalSize.Width;
            Height = totalSize.Height;
            Left = totalSize.Left;
            Top = totalSize.Top;

            UpdateLayout();
        }

        private void GuiCanvas_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
