using BarcodeReader.BarcodeStuff;
using BarcodeReader.BarcodeStuff.Engines.Core;
using BarcodeReader.BarcodeStuff.Models;
using BarcodeReader.Database;
using BarcodeReader.Misc;
using BarcodeReader.Windows;
using NoRe.Database.Core.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using WindowsInput;
using Clipboard = System.Windows.Clipboard;

namespace BarcodeReader
{
    public partial class MainWindow : Window
    {
        #region "Properties"

        private GlobalHotkey ScanTextboxHotkey { get; set; }
        private GlobalHotkey ScanScreenshotHotkey { get; set; }
        private BarcodeHistoryUserControl CurrentBarcode { get; set; }
        private DatabaseStatements DatabaseStatements { get; set; }
        private SettingsWindow Settings { get; set; }
        private NotifyIcon NotifyIcon { get; set; }

        #endregion

        #region "Startup"

        public MainWindow()
        {
            InitializeComponent();

            Settings = new SettingsWindow();
            DatabaseStatements = new DatabaseStatements();

            DatabaseStatements.CreateTable();

            BarcodeEngineManager.SetBarcodeEngine(Settings.Settings.BarcodeEngine);
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
            NotifyIcon.DoubleClick += NotifyIconDoubleClick;

            if (Settings.Settings.StartAsNotifyIcon)
            {
                ShowInTaskbar = false;
                Hide();
            }
        }

        private void RegisterHotkeys()
        {
            ScanScreenshotHotkey = new GlobalHotkey(HotkeyConstants.NOMOD, Settings.Settings.ScanHotkey, this);
            ScanScreenshotHotkey.Triggered += ScanScreenshotHotkeyTriggered;
            ScanScreenshotHotkey.Register();

            ScanTextboxHotkey = new GlobalHotkey(HotkeyConstants.NOMOD, Settings.Settings.RescanHotkey, this);
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

            foreach (Row r in DatabaseStatements.GetHistory())
            {
                BarcodeHistoryUserControl temp = new BarcodeHistoryUserControl(
                  new Barcode(
                      r.GetValue<string>("value"),
                      (BarcodeFormat)Enum.Parse(typeof(BarcodeFormat), r.GetValue<string>("type"))),
                      DateTime.Parse(r.GetValue<string>("date")));

                temp.Deleted += OnHistoryDeleted;
                temp.Clicked += OnHistoryClicked;

                HistoryStackpanel.Children.Add(temp);
            }
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

        #endregion

        #region "Click Events"

        private void NotifyIconDoubleClick(object sender, EventArgs e)
        {
            NotifyIcon.Visible = false;
            ShowInTaskbar = true;
            Show();
            BringIntoView();
        }

        private void ScreenshotButton_Click(object sender, RoutedEventArgs e) => HandleScan(ReadBarcodeFromImage(TakeScreenshot()));

        private void ScanButton_Click(object sender, RoutedEventArgs e) => AddBarcode();

        private void ConfigButton_Click(object sender, RoutedEventArgs e) => Settings.ShowDialog();

        private void Fnc1Button_Click(object sender, RoutedEventArgs e) => ScanTextBox.Text = ScanTextBox.Text.Insert(ScanTextBox.SelectionStart, BarcodeConstants.FNC1_DisplayPlaceholder);

        private void ScanScreenshotHotkeyTriggered(object sender, EventArgs e) => HandleScan(ReadBarcodeFromImage(TakeScreenshot()));

        private void ScanTextBoxHotkeyTriggered(object sender, EventArgs e) => AddBarcode();

        #endregion

        #region "Misc Events"

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            switch (System.Windows.MessageBox.Show("Do you realy want to close this application?" + Environment.NewLine + "Yes: Close" + Environment.NewLine + "No: Minimize", "Close?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes:
                    ScanScreenshotHotkey.Unregister();
                    ScanTextboxHotkey.Unregister();
                    Settings.Close(false);
                    DatabaseStatements?.Dispose();
                    break;

                case MessageBoxResult.No:
                    MinimizeToIcon();
                    e.Cancel = true;
                    break;
                default:
                    e.Cancel = true;

                    break;
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

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized) MinimizeToIcon();
        }

        #endregion

        #region "Barcode/Scan handling"

        private void AddBarcode()
        {
            if (string.IsNullOrEmpty(ScanTextBox.Text)) return;

            HandleScan(new Barcode(ScanTextBox.Text.Replace(BarcodeConstants.FNC1_DisplayPlaceholder, BarcodeConstants.FNC1.ToString()), (BarcodeFormat)BarcodeTypeComboBox.SelectedItem));
        }

        private Bitmap TakeScreenshot()
        {
            ScreenShotWindow screenShotWindow = new ScreenShotWindow();
            bool? result = screenShotWindow.TakeScreenshot();

            if (result is null || !(bool)result) return null;
            return screenShotWindow.Screenshot;
        }

        private Barcode ReadBarcodeFromImage(Bitmap image) => BarcodeEngineManager.CurrentBarcodeEngine.Read(image);

        private void HandleScan(Barcode barcode)
        {
            if (barcode is null) return;

            BarcodeHistoryUserControl temp = TryGetHistory(barcode, barcode.Text);
            if (temp is null)
            {
                temp = new BarcodeHistoryUserControl(barcode, DateTime.Now, barcode.Text);
                temp.Deleted += OnHistoryDeleted;
                temp.Clicked += OnHistoryClicked;
                HistoryStackpanel.Children.Insert(0, temp);
                DatabaseStatements.InsertBarcode(barcode.Text, barcode.Format);
            }

            ScrollToTarget(temp, barcode.Text);

            WriteText(barcode.Text);
        }

        private void WriteText(string value)
        {
            if (ScanTextBox.IsFocused) ScanButton.Focus();

            if(Settings.Settings.SimulateKeyPress)
            {
                // use simulator for actual keypress events
                InputSimulator simulator = new InputSimulator();
                foreach (char c in value.ToCharArray())
                    simulator.Keyboard.TextEntry(c);
            }

            if(Settings.Settings.UseCopyPaste)
            {
                // use copy and paste, cause its faster and writes FNC1-Char which SendKeys does not
                Clipboard.SetText(value);
                SendKeys.SendWait("^{v}");
            }
      
        }

        #endregion

        #region "History handling"

        private void OnHistoryDeleted(object sender, EventArgs e) => DeleteHistory((BarcodeHistoryUserControl)sender);

        private void OnHistoryClicked(object sender, EventArgs e)
        {
            BarcodeHistoryUserControl target = sender as BarcodeHistoryUserControl;
            if (CurrentBarcode != null && target == CurrentBarcode) return;

            ScrollToTarget(target);
        }

        private BarcodeHistoryUserControl TryGetHistory(Barcode barcode, string barcodeText) =>
             HistoryStackpanel.Children.OfType<BarcodeHistoryUserControl>().
             FirstOrDefault(x => x.Barcode.Text == barcodeText && x.Barcode.Format == barcode.Format);

        private void ScrollToTarget(BarcodeHistoryUserControl target, string textToSet = "")
        {
            if (CurrentBarcode != null) CurrentBarcode.MainGrid.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(ColorConstants.Blue);
            CurrentBarcode = target;
            CurrentBarcode.MainGrid.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(ColorConstants.Orange);
            HistoryScrollViewer.ScrollToVerticalOffset(target.TranslatePoint(new System.Windows.Point(), HistoryStackpanel).Y);

            if (string.IsNullOrEmpty(textToSet))
                ScanTextBox.Text = target.Barcode.Text.Replace(BarcodeConstants.FNC1.ToString(), BarcodeConstants.FNC1_DisplayPlaceholder);
            else
                ScanTextBox.Text = textToSet.Replace(BarcodeConstants.FNC1.ToString(), BarcodeConstants.FNC1_DisplayPlaceholder);

            BarcodeTypeComboBox.SelectedItem = target.Barcode.Format;
            ScanTextBox.SelectAll();
        }

        private void DeleteHistory(BarcodeHistoryUserControl history)
        {
            DatabaseStatements.DeleteHistory(history.Barcode.Text, history.Barcode.Format);
            HistoryStackpanel.Children.Remove(history);
        }

        #endregion

        #region "Misc"

        private void MinimizeToIcon()
        {
            NotifyIcon.Visible = true;
            ShowInTaskbar = false;
            Hide();
        }

        #endregion
    }
}
