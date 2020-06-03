using BarcodeReader.Windows;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
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
using IronBarCode;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Interop;
using System.Windows.Automation.Peers;
using System.Reflection;

namespace BarcodeReader
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Misc.GlobalHotkey ScanHotkey { get; set; }
        private BarcodeHistoryUserControl CurrentBarcode { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            HandleScan(ReadBarcodeFromImage(TakeScreenshot()));
        }

        private void ScanHotkeyTriggered(object sender, EventArgs e)
        {
            HandleScan(ReadBarcodeFromImage(TakeScreenshot()));
        }

        private void HandleScan(BarcodeResult barcode)
        {
            if (barcode is null) return;
            HandleScan(BarcodeWriter.CreateBarcode(barcode.Value, barcode.BarcodeType));
        }

        private void HandleScan(GeneratedBarcode barcode)
        {
            if (barcode is null) return;

            BarcodeHistoryUserControl temp = TryGetHistory(barcode);
            if (temp is null)
            {
                temp = new BarcodeHistoryUserControl(barcode);
                HistoryStackpanel.Children.Insert(0, temp);
            }

            if (CurrentBarcode != null) CurrentBarcode.MainGrid.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF004799");
            CurrentBarcode = temp;
            CurrentBarcode.MainGrid.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFC8706");

            HistoryScrollViewer.ScrollToVerticalOffset(CurrentBarcode.TranslatePoint(new System.Windows.Point(), HistoryStackpanel).Y);

            WriteText(barcode.Value);
        }

        private BarcodeHistoryUserControl TryGetHistory(GeneratedBarcode barcode)
        {
            foreach (BarcodeHistoryUserControl history in HistoryStackpanel.Children)
            {
                if (history.Barcode.Value == barcode.Value && history.Barcode.BarcodeType == barcode.BarcodeType) return history;
            }

            return null;
        }

        private Bitmap TakeScreenshot()
        {
            ScreenShotWindow screenShotWindow = new ScreenShotWindow();
            bool? result = screenShotWindow.TakeScreenshot();
            if (result is null || !(bool)result) return null;
            return screenShotWindow.Screenshot;
        }

        private BarcodeResult ReadBarcodeFromImage(Bitmap image)
        {
            if (image is null) return null;

            return IronBarCode.BarcodeReader.QuicklyReadOneBarcode(image, BarcodeEncoding.All, true);
        }

        private void WriteText(string value)
        {
            SendKeys.SendWait(value);
        }

        private void RegisterHotkeys()
        {
            ScanHotkey = new Misc.GlobalHotkey(Misc.HotkeyConstants.NOMOD, Keys.End, this);
            ScanHotkey.Triggered += ScanHotkeyTriggered;
            ScanHotkey.Register();
        }

        private void SetInfoLabel()
        {
            AssemblyName assembly = GetType().Assembly.GetName();

            InfoLabel.Content = assembly.Name + " - " + assembly.Version;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterHotkeys();
            SetInfoLabel();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ScanHotkey.Unregister();
        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            AddBarcode();
        }

        private void AddBarcode()
        {
            if (string.IsNullOrEmpty(ScanTextBox.Text)) return;
            // TODO: Encoding auswählen
            HandleScan(BarcodeWriter.CreateBarcode(ScanTextBox.Text, BarcodeEncoding.Code128));
            ScanTextBox.Text = "";
        }

        private void ScanTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter) AddBarcode();
        }
    }
}
