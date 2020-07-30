using BarcodeReader.BarcodeStuff;
using BarcodeReader.BarcodeStuff.Engines.Core;
using BarcodeReader.BarcodeStuff.Models;
using BarcodeReader.Misc;
using BarcodeReader.UserControls;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using ZXing;

namespace BarcodeReader.Windows
{
    /// <summary>
    /// Interaktionslogik für BarcodeWindow.xaml
    /// </summary>
    public partial class BarcodeWindow : Window
    {
        public BarcodeWindow(Barcode barcode)
        {
            InitializeComponent();

            BarcodeImage.Source = ConvertBitmapToImageSource(BarcodeEngineLoader.BarcodeEngine.Write(barcode.Text, barcode.Format));

            FormatLabel.Content = barcode.Format.ToString();
            GS1Label.Content = barcode.IsGS1.ToString();
            ContentLabel.Content = barcode.Text.Replace(BarcodeConstants.FNC1.ToString(), BarcodeConstants.FNC1_DisplayPlaceholder);

            if (barcode.IsGS1) SetGs1Controls(barcode);
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

        private void SetGs1Controls(Barcode barcode)
        {
            foreach (ApplicationIdentifier ai in barcode.ApplicationIdentifer)
            {
                Gs1Stackpanel.Children.Add(new AiUserControl(ai));
            }
        }

    }
}
