using System;
using System.Windows;
using System.Windows.Controls;
using ZXing;

namespace BarcodeReader
{
    /// <summary>
    /// Interaktionslogik für BarcodeHistoryUserControl.xaml
    /// </summary>
    public partial class BarcodeHistoryUserControl : UserControl
    {
        public Result Barcode { get; set; }
        public event EventHandler Deleted;

        public BarcodeHistoryUserControl()
        {
            InitializeComponent();
        }

        public BarcodeHistoryUserControl(Result barcode, DateTime timeStamp, string formattedFnc1Barcode = "")
        {
            InitializeComponent();

            Barcode = barcode;
            
            if (string.IsNullOrEmpty(formattedFnc1Barcode))
                ContentLabel.Content = barcode.Text.Replace(Misc.BarcodeConstants.FNC1.ToString(), Misc.BarcodeConstants.FNC1_DisplayPlaceholder);
            else
                ContentLabel.Content = formattedFnc1Barcode.Replace(Misc.BarcodeConstants.FNC1.ToString(), Misc.BarcodeConstants.FNC1_DisplayPlaceholder);

            InfoLabel.Content = string.Format("Barcode Type: {0} - Scannend at {1}", barcode.BarcodeFormat, timeStamp);
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Deleted(this, e);
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            BarcodeWriter bcWriter = new BarcodeWriter
            {
                Format = Barcode.BarcodeFormat
            };

            new Windows.BarcodeWindow(bcWriter.Write(Barcode.Text)).Show();
        }
    }
}
