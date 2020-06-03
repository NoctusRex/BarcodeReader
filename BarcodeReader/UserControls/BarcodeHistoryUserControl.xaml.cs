using IronBarCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        public BarcodeHistoryUserControl(GeneratedBarcode barcode)
        {
            InitializeComponent();

            Barcode = barcode;
            ContentLabel.Content = barcode.Value;
            InfoLabel.Content = string.Format("Barcode Type: {0} - Scannend at {1}", barcode.BarcodeType, DateTime.Now);
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Deleted(this, e);
        }
    }
}
