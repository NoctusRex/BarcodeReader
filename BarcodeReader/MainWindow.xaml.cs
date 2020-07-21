using BarcodeReader.Windows;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using NoRe.Database.SqLite;
using NoRe.Database.Core.Models;
using ZXing;
using NoRe.Core;
using Application = System.Windows.Application;
using System.Text;
using BarcodeReader.Misc;
using System.Text.RegularExpressions;
using Clipboard = System.Windows.Clipboard;
using System.Collections.Generic;

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
        private SettingsWindow Settings { get; set; }
        private NotifyIcon NotifyIcon { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Database = new SqLiteWrapper(System.IO.Path.Combine(Pathmanager.StartupDirectory, "History.db"), "3");

            Database.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS history (id INTEGER PRIMARY KEY AUTOINCREMENT, value TEXT, type TEXT, date TEXT)");
            Settings = new SettingsWindow();
        }

        private void InitializeNotifyIcon()
        {
            NotifyIcon = new NotifyIcon()
            {
                Visible = Settings.Settings.StartAsNotifyIcon,
                BalloonTipText = "Barcode Reader",
                BalloonTipTitle = "Barcode Reader",
                Text = "Barcode Reader",
                Icon = Properties.Resources.barcode1
            };

            if (Settings.Settings.StartAsNotifyIcon)
            {
                ShowInTaskbar = false;
                Hide();
            }

            NotifyIcon.DoubleClick += NotifyIconDoubleClick;
        }

        private void NotifyIconDoubleClick(object sender, EventArgs e)
        {
            NotifyIcon.Visible = false;
            ShowInTaskbar = true;
            Show();
            BringIntoView();
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

        private void HandleScan(Result barcode)
        {
            if (barcode is null) return;
            string barcodeText = HandleFnc1(barcode.Text);

            BarcodeHistoryUserControl temp = TryGetHistory(barcode, barcodeText);
            if (temp is null)
            {
                temp = new BarcodeHistoryUserControl(barcode, DateTime.Now, barcodeText);
                temp.Deleted += OnHistoryDeleted;
                HistoryStackpanel.Children.Insert(0, temp);
                Database.ExecuteNonQuery("INSERT INTO history (value, type, date) VALUES (@0, @1, @2)", barcodeText, barcode.BarcodeFormat, DateTime.Now.ToString());
            }

            ScrollToTarget(temp, barcodeText);

            WriteText(barcodeText);
        }

        private string HandleFnc1(string barcode)
        {
            if (!barcode.StartsWith(BarcodeConstants.FNC1_SymbologyIdentifier)) return barcode;

            string newBarcode = "";
            foreach (KeyValuePair<Fnc1Parser.AII, string> ai in Fnc1Parser.Parse(barcode, true))
            {
                newBarcode += BarcodeConstants.FNC1 + ai.Key.AI + ai.Value;
            }
            return newBarcode;
        }

        private void ScrollToTarget(BarcodeHistoryUserControl target, string textToSet = "")
        {
            if (CurrentBarcode != null) CurrentBarcode.MainGrid.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF004799");
            CurrentBarcode = target;
            CurrentBarcode.MainGrid.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFC8706");
            HistoryScrollViewer.ScrollToVerticalOffset(target.TranslatePoint(new System.Windows.Point(), HistoryStackpanel).Y);

            if (string.IsNullOrEmpty(textToSet))
                ScanTextBox.Text = target.Barcode.Text;
            else
                ScanTextBox.Text = textToSet;

            BarcodeTypeComboBox.SelectedItem = target.Barcode.BarcodeFormat;
            ScanTextBox.SelectAll();
        }

        private BarcodeHistoryUserControl TryGetHistory(Result barcode, string barcodeText)
        {
            foreach (BarcodeHistoryUserControl history in HistoryStackpanel.Children)
            {
                if (history.Barcode.Text == barcodeText && history.Barcode.BarcodeFormat == barcode.BarcodeFormat) return history;
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

        private Result ReadBarcodeFromImage(Bitmap image)
        {
            if (image is null) return null;
            ZXing.BarcodeReader reader = new ZXing.BarcodeReader();
            reader.Options.AssumeGS1 = true;
            reader.Options.TryHarder = true;
            reader.Options.CharacterSet = "UTF-16";
            return reader.Decode(image);
        }

        private void WriteText(string value)
        {
            // use copy and paste, cause its faster and writes FNC1-Char
            Clipboard.SetText(value);

            if (ScanTextBox.IsFocused) ScanButton.Focus();

            SendKeys.SendWait("^{v}");
        }

        private void RegisterHotkeys()
        {
            ScanScreenshotHotkey = new GlobalHotkey(HotkeyConstants.NOMOD, Keys.End, this);
            ScanScreenshotHotkey.Triggered += ScanScreenshotHotkeyTriggered;
            ScanScreenshotHotkey.Register();

            ScanTextboxHotkey = new GlobalHotkey(HotkeyConstants.NOMOD, Keys.Home, this);
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
                BarcodeHistoryUserControl temp = new BarcodeHistoryUserControl(
                    CreateAndScanBarcode(
                        r.GetValue<string>("value"),
                        (BarcodeFormat)Enum.Parse(typeof(BarcodeFormat), r.GetValue<string>("type"))),
                        DateTime.Parse(r.GetValue<string>("date")));

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
            Database.ExecuteNonQuery("DELETE FROM history WHERE value=@0 AND type=@1", history.Barcode.Text, history.Barcode.BarcodeFormat);
            HistoryStackpanel.Children.Remove(history);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterHotkeys();
            SetInfoLabel();
            LoadHistory();
            BarcodeTypeComboBox.ItemsSource = Enum.GetValues(typeof(BarcodeFormat));
            BarcodeTypeComboBox.SelectedItem = BarcodeFormat.CODE_128;

            ScanTextBox.Focus();
            if (HistoryStackpanel.Children.Count > 0) ScrollToTarget((BarcodeHistoryUserControl)HistoryStackpanel.Children[0]);

            InitializeNotifyIcon();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ScanScreenshotHotkey.Unregister();
            ScanTextboxHotkey.Unregister();
            Settings.Close();
        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            AddBarcode();
        }

        private void AddBarcode()
        {
            if (string.IsNullOrEmpty(ScanTextBox.Text)) return;

            HandleScan(CreateAndScanBarcode(ReplaceFnc1Placeholder(ScanTextBox.Text), (BarcodeFormat)BarcodeTypeComboBox.SelectedItem));
        }

        private string ReplaceFnc1Placeholder(string barcode)
        {
            return barcode.Replace(BarcodeConstants.FNC1_DisplayPlaceholder, BarcodeConstants.FNC1.ToString());
        }

        private Result CreateAndScanBarcode(string text, BarcodeFormat format)
        {
            BarcodeWriter bcWriter = new BarcodeWriter
            {
                Format = format
            };

            return ReadBarcodeFromImage(bcWriter.Write(text));
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
                if (HistoryStackpanel.Children.Count > 0)
                {
                    CurrentBarcode = (BarcodeHistoryUserControl)HistoryStackpanel.Children[0];
                    ScrollToTarget(CurrentBarcode);
                    return;
                }
                else return;

            int currentIndex = HistoryStackpanel.Children.IndexOf(CurrentBarcode);
            if (currentIndex == -1) return;
            BarcodeHistoryUserControl temp;

            switch (e.Key)
            {
                case Key.Up:
                    if (currentIndex < 1) return;

                    temp = (BarcodeHistoryUserControl)HistoryStackpanel.Children[currentIndex - 1];
                    ScrollToTarget(temp);

                    break;
                case Key.Down:
                    if (currentIndex >= HistoryStackpanel.Children.Count - 1) return;

                    temp = (BarcodeHistoryUserControl)HistoryStackpanel.Children[currentIndex + 1];
                    ScrollToTarget(temp);

                    break;
                case Key.Delete:
                    if (currentIndex == -1) return;
                    DeleteHistory((BarcodeHistoryUserControl)HistoryStackpanel.Children[currentIndex]);
                    CurrentBarcode = null;
                    ScanTextBox.Text = "";
                    BarcodeTypeComboBox.SelectedItem = BarcodeFormat.CODE_128;
                    break;
            }
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.ShowDialog();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                NotifyIcon.Visible = true;
                ShowInTaskbar = false;
                Hide();
            }
        }

        private void Fnc1Button_Click(object sender, RoutedEventArgs e)
        {
            ScanTextBox.Text = ScanTextBox.Text.Insert(ScanTextBox.SelectionStart, BarcodeConstants.FNC1_DisplayPlaceholder);
        }
    }
}
