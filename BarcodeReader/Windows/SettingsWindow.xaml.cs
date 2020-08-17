using BarcodeReader.BarcodeStuff.Engines.Core;
using BarcodeReader.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
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
                Path = PathManager.SettingsPath
            };

            if (!System.IO.File.Exists(Settings.Path)) Settings.Write();

            Settings.Read();

            LoadCheckboxes();
            LoadHotKeyComboboxes();
            LoadBarcodeEngineCombobox();
        }

        private void LoadCheckboxes()
        {
            StartAfterBootRadioButton.IsChecked = Settings.StartAtStartup;
            StartAsNotifyIconRadioButton.IsChecked = Settings.StartAsNotifyIcon;
            UseCopyPasteRadioButton.IsChecked = Settings.UseCopyPaste;
            UseSimulationRadioButton.IsChecked = Settings.SimulateKeyPress;

            if (!(bool)UseCopyPasteRadioButton.IsChecked && !(bool)UseSimulationRadioButton.IsChecked)
            {
                UseCopyPasteRadioButton.IsChecked = true;
                Settings.UseCopyPaste = true;
                Settings.Write();
            }

            if ((bool)UseCopyPasteRadioButton.IsChecked && (bool)UseSimulationRadioButton.IsChecked)
            {
                UseCopyPasteRadioButton.IsChecked = false;
                Settings.UseCopyPaste = false;
                Settings.Write();
            }

        }

        private void LoadHotKeyComboboxes()
        {
            ScanHotkeyComboBox.ItemsSource = Enum.GetValues(typeof(Keys));
            ScanHotkeyComboBox.SelectedItem = Settings.ScanHotkey;

            RescanHotkeyComboBox.ItemsSource = Enum.GetValues(typeof(Keys));
            RescanHotkeyComboBox.SelectedItem = Settings.RescanHotkey;
        }

        private void LoadBarcodeEngineCombobox()
        {
            // set current engine from settings file or set the first engine found if no engine is configured
            IEnumerable<string> engines = BarcodeEngineManager.GetAllBarcodeEngines().Select(x => x.Identifier);
            string currentEngine = engines.SingleOrDefault(x => x == Settings.BarcodeEngine);
            BarcodeEngineComboBox.ItemsSource = engines;
            if (string.IsNullOrEmpty(currentEngine))
            {
                BarcodeEngineComboBox.SelectedItem = engines.First();
                Settings.BarcodeEngine = engines.First();
                SaveButton_Click(this, new RoutedEventArgs());
            }
            else
            {
                BarcodeEngineComboBox.SelectedItem = currentEngine;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCheckboxes();

            ShowRestartHint();

            SaveHotkeys();

            SaveBarcodeEngine();

            Settings.Write();

            if (Settings.StartAtStartup) CreateStartupShortcut();
            else RemoveStartupPath();

            Hide();
        }

        private void SaveCheckboxes()
        {
            Settings.StartAsNotifyIcon = (bool)StartAsNotifyIconRadioButton.IsChecked;
            Settings.StartAtStartup = (bool)StartAfterBootRadioButton.IsChecked;
            Settings.UseCopyPaste = (bool)UseCopyPasteRadioButton.IsChecked;
            Settings.SimulateKeyPress = (bool)UseSimulationRadioButton.IsChecked;
        }

        private void ShowRestartHint()
        {
            if (Settings.ScanHotkey != (Keys)ScanHotkeyComboBox.SelectedItem ||
                Settings.RescanHotkey != (Keys)RescanHotkeyComboBox.SelectedItem ||
                Settings.BarcodeEngine != (string)BarcodeEngineComboBox.SelectedItem)
                MessageBox.Show("Some changes will only take effect after restarting the application", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveHotkeys()
        {
            Settings.ScanHotkey = (Keys)ScanHotkeyComboBox.SelectedItem;
            Settings.RescanHotkey = (Keys)RescanHotkeyComboBox.SelectedItem;
        }

        private void SaveBarcodeEngine() => Settings.BarcodeEngine = BarcodeEngineComboBox.SelectedItem.ToString();

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

        private void UseCopyPasteRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)UseSimulationRadioButton.IsChecked) UseSimulationRadioButton.IsChecked = false;
        }

        private void UseCopyPasteRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!(bool)UseSimulationRadioButton.IsChecked) UseSimulationRadioButton.IsChecked = true;
        }

        private void UseSimulationRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)UseCopyPasteRadioButton.IsChecked) UseCopyPasteRadioButton.IsChecked = false;
        }

        private void UseSimulationRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!(bool)UseCopyPasteRadioButton.IsChecked) UseCopyPasteRadioButton.IsChecked = true;
        }
    }
}
