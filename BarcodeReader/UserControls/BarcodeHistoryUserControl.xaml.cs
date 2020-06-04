using IronBarCode;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace BarcodeReader
{
    /// <summary>
    /// Interaktionslogik für BarcodeHistoryUserControl.xaml
    /// </summary>
    public partial class BarcodeHistoryUserControl : UserControl
    {
        public GeneratedBarcode Barcode { get; set; }
        public event EventHandler Deleted;

        public BarcodeHistoryUserControl()
        {
            InitializeComponent();
        }

        public BarcodeHistoryUserControl(GeneratedBarcode barcode, DateTime timeStamp)
        {
            InitializeComponent();

            Barcode = barcode;
            ContentLabel.Content = barcode.Value;
            InfoLabel.Content = string.Format("Barcode Type: {0} - Scannend at {1}", barcode.BarcodeType, timeStamp);
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Deleted(this, e);
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            new Windows.BarcodeWindow((Bitmap)Barcode.Image).Show();
        }
    }
}
