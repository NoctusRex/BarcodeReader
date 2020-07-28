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
        public BarcodeWindow(Result barcode)
        {
            InitializeComponent();

            BarcodeWriter bcWriter = new BarcodeWriter
            {
                Format = barcode.BarcodeFormat
            };
            bool isGs1 = barcode.Text.Contains(BarcodeConstants.FNC1.ToString());

            BarcodeImage.Source = ConvertBitmapToImageSource(bcWriter.Write(barcode.Text));

            FormatLabel.Content = barcode.BarcodeFormat.ToString();
            GS1Label.Content = isGs1.ToString();
            ContentLabel.Content = barcode.Text.Replace(BarcodeConstants.FNC1.ToString(), BarcodeConstants.FNC1_DisplayPlaceholder);

            if (isGs1) SetGs1Controls(barcode.Text);
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

        private void SetGs1Controls(string barcode)
        {
            // restore orignal barcode
            barcode =  BarcodeConstants.FNC1_SymbologyIdentifier + barcode.TrimStart(BarcodeConstants.FNC1);

            foreach (KeyValuePair<Fnc1Parser.AII, string> ai in Fnc1Parser.Parse(barcode, false))
            {
                Gs1Stackpanel.Children.Add(new AiUserControl(ai.Key, ai.Value));
            }
        }

    }
}
