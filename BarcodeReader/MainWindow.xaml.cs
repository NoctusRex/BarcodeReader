using BarcodeReader.Windows;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using IronBarCode;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using NoRe.Database.SqLite;
using NoRe.Database.Core.Models;

namespace BarcodeReader
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Misc.GlobalHotkey ScanTextboxHotkey { get; set; }
        private Misc.GlobalHotkey ScanScreenshotHotkey { get; set; }
        private BarcodeHistoryUserControl CurrentBarcode { get; set; }
        private SqLiteWrapper Database { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Database = new SqLiteWrapper("./History.db", "3");

            Database.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS history (id INTEGER PRIMARY KEY AUTOINCREMENT, value TEXT, type TEXT)");
        }

        private void ScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            HandleScan(ReadBarcodeFromImage(TakeScreenshot()));
        }

        private void ScanScreenshotHotkeyTriggered(object sender, EventArgs e)
        {
            HandleScan(ReadBarcodeFromImage(TakeScreenshot()));
        }

        private void ScanTextBoxHotkeyTriggered(object sender, EventArgs e)
        {
            AddBarcode();
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
                temp.Deleted += OnHistoryDeleted;
                HistoryStackpanel.Children.Insert(0, temp);
                Database.ExecuteNonQuery("INSERT INTO history (value, type) VALUES (@0, @1)", barcode.Value, barcode.BarcodeType);
            }

            ScrollToTarget(temp);
            ScanTextBox.Text = barcode.Value;

            WriteText(barcode.Value);
        }

        private void ScrollToTarget(BarcodeHistoryUserControl target)
        {
            if (CurrentBarcode != null) CurrentBarcode.MainGrid.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF004799");
            CurrentBarcode = target;
            CurrentBarcode.MainGrid.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFC8706");
            HistoryScrollViewer.ScrollToVerticalOffset(target.TranslatePoint(new System.Windows.Point(), HistoryStackpanel).Y);
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
            ScanScreenshotHotkey = new Misc.GlobalHotkey(Misc.HotkeyConstants.NOMOD, Keys.End, this);
            ScanScreenshotHotkey.Triggered += ScanScreenshotHotkeyTriggered;
            ScanScreenshotHotkey.Register();

            ScanTextboxHotkey = new Misc.GlobalHotkey(Misc.HotkeyConstants.NOMOD, Keys.Home, this);
            ScanTextboxHotkey.Triggered += ScanTextBoxHotkeyTriggered;
            ScanTextboxHotkey.Register();
        }

        private void SetInfoLabel()
        {
            AssemblyName assembly = GetType().Assembly.GetName();

            InfoLabel.Content = assembly.Name + " - " + assembly.Version;
        }

        private void LoadHistory()
        {
            HistoryStackpanel.Children.Clear();

            foreach (Row r in Database.ExecuteReader("SELECT * FROM history ORDER BY id DESC").Rows)
            {
                BarcodeHistoryUserControl temp = new BarcodeHistoryUserControl(BarcodeWriter.CreateBarcode(r.GetValue<string>("value"), (BarcodeEncoding)Enum.Parse(typeof(BarcodeEncoding), r.GetValue<string>("type"))));
                temp.Deleted += OnHistoryDeleted;
                HistoryStackpanel.Children.Add(temp);
            }
        }

        private void OnHistoryDeleted(object sender, EventArgs e)
        {
            DeleteHistory((BarcodeHistoryUserControl)sender);
        }

        private void DeleteHistory(BarcodeHistoryUserControl history)
        {
            Database.ExecuteNonQuery("DELETE FROM history WHERE value=@0 AND type=@1", history.Barcode.Value, history.Barcode.BarcodeType);
            HistoryStackpanel.Children.Remove(history);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterHotkeys();
            SetInfoLabel();
            LoadHistory();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ScanScreenshotHotkey.Unregister();
            ScanTextboxHotkey.Unregister();
        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            AddBarcode();
        }

        private void AddBarcode()
        {
            if (string.IsNullOrEmpty(ScanTextBox.Text)) return;
            // TODO: Encoding auswählen

            if(CurrentBarcode is null)
            {
                HandleScan(BarcodeWriter.CreateBarcode(ScanTextBox.Text, BarcodeEncoding.Code128));
            }
            else
            {
                HandleScan(BarcodeWriter.CreateBarcode(ScanTextBox.Text, CurrentBarcode.Barcode.BarcodeType));
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Ignore all of this please
            if (e.Key == Key.Enter)
            {
                AddBarcode();
                return;
            }

            if (CurrentBarcode is null)
                if (HistoryStackpanel.Children.Count > 0) CurrentBarcode = (BarcodeHistoryUserControl)HistoryStackpanel.Children[0];
                else return;

            int currentIndex = -1;
            if (HistoryStackpanel.Children.Count > 0) currentIndex = HistoryStackpanel.Children.IndexOf(CurrentBarcode);
            if (currentIndex == -1) CurrentBarcode = (BarcodeHistoryUserControl)HistoryStackpanel.Children[0];
            BarcodeHistoryUserControl temp;

            switch (e.Key)
            {
                case Key.Up:
                    if (currentIndex < 1) return;

                    temp = (BarcodeHistoryUserControl)HistoryStackpanel.Children[currentIndex - 1];
                    ScrollToTarget(temp);
                    ScanTextBox.Text = temp.Barcode.Value;

                    break;
                case Key.Down:
                    if (currentIndex >= HistoryStackpanel.Children.Count - 1) return;

                    temp = (BarcodeHistoryUserControl)HistoryStackpanel.Children[currentIndex + 1];
                    ScrollToTarget(temp);
                    ScanTextBox.Text = temp.Barcode.Value;

                    break;
                case Key.Back:
                    if (currentIndex == -1) return;
                    DeleteHistory((BarcodeHistoryUserControl)HistoryStackpanel.Children[currentIndex]);
                    ScanTextBox.Text = "";
                    break;
            }
        }
    }
}
