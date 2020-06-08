using BarcodeReader.Misc;
using NoRe.Core;
using System;
using System.Windows;

namespace BarcodeReader.Windows
{
    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        internal Settings Settings { get; set; }

        private string StartupPath { get => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "BarcodeReader.lnk"); }

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
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.StartAsNotifyIcon = (bool)StartAsNotifyIconRadioButton.IsChecked;
            Settings.StartAtStartup = (bool)StartAfterBootRadioButton.IsChecked;
            Settings.Write();

            if (Settings.StartAtStartup) CreateStartupShortcut();
            else RemoveStartupPath();

            Hide();
        }

        private void CreateStartupShortcut()
        {
            dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell"));
            dynamic link = shell.CreateShortcut(StartupPath);

            link.TargetPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            link.WindowStyle = 1;
            link.Save();
        }

        private void RemoveStartupPath()
        {
            if (System.IO.File.Exists(StartupPath)) System.IO.File.Delete(StartupPath);
        }

    }
}
