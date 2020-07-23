using BarcodeReader.Misc;
using NoRe.Core;
using System;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace BarcodeReader.Windows
{
    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        internal Settings Settings { get; set; }

        private string StartupLinkPath => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "BarcodeReader.lnk");
        private bool CloseForm { get; set; }

        public SettingsWindow()
        {
            InitializeComponent();
            Settings = new Settings
            {
                Path = System.IO.Path.Combine(Pathmanager.StartupDirectory, "Settings.json")
            };

            if (!System.IO.File.Exists(Settings.Path)) Settings.Write();

            Settings.Read();

            StartAfterBootRadioButton.IsChecked = Settings.StartAtStartup;
            StartAsNotifyIconRadioButton.IsChecked = Settings.StartAsNotifyIcon;

            ScanHotkeyComboBox.ItemsSource = Enum.GetValues(typeof(Keys));
            ScanHotkeyComboBox.SelectedItem = Settings.ScanHotkey;

            RescanHotkeyComboBox.ItemsSource = Enum.GetValues(typeof(Keys));
            RescanHotkeyComboBox.SelectedItem = Settings.RescanHotkey;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.StartAsNotifyIcon = (bool)StartAsNotifyIconRadioButton.IsChecked;
            Settings.StartAtStartup = (bool)StartAfterBootRadioButton.IsChecked;

            if (Settings.ScanHotkey != (Keys)ScanHotkeyComboBox.SelectedItem ||
               Settings.RescanHotkey != (Keys)RescanHotkeyComboBox.SelectedItem)
                MessageBox.Show("Some changes will only take effect after restarting the application", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

            Settings.ScanHotkey = (Keys)ScanHotkeyComboBox.SelectedItem;
            Settings.RescanHotkey = (Keys)RescanHotkeyComboBox.SelectedItem;

            Settings.Write();

            if (Settings.StartAtStartup) CreateStartupShortcut();
            else RemoveStartupPath();

            Hide();
        }

        private void CreateStartupShortcut()
        {
            dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell"));
            dynamic link = shell.CreateShortcut(StartupLinkPath);

            link.TargetPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            link.WindowStyle = 1;
            link.Save();
        }

        private void RemoveStartupPath()
        {
            if (System.IO.File.Exists(StartupLinkPath)) System.IO.File.Delete(StartupLinkPath);
        }

        public void Close(bool hide = true)
        {
            CloseForm = !hide;
            base.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!CloseForm)
            {
                e.Cancel = true;
                Hide();
            }
        }
    }
}
