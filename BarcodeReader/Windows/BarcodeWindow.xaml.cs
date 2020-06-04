using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BarcodeReader.Windows
{
    /// <summary>
    /// Interaktionslogik für BarcodeWindow.xaml
    /// </summary>
    public partial class BarcodeWindow : Window
    {
        public BarcodeWindow(Bitmap barcodeImage)
        {
            InitializeComponent();

            BarcodeImage.Source = ConvertBitmapToImageSource(barcodeImage);
        }

        private BitmapImage ConvertBitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

    }
}
