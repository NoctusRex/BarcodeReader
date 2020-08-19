using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace BarcodeReader
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private Mutex mutex;
        private bool handleExceptions = false;

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = handleExceptions;

            MessageBox.Show(e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            CheckAlreadyRunning();

            base.OnStartup(e);
        }

        private void CheckAlreadyRunning()
        {
            mutex = new Mutex(false, "Barcode Reader - 133742069");

            if (!mutex.WaitOne(0, false))
                throw new Exception("The application is already running.");
            
            handleExceptions = true;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            mutex?.Close();
            mutex?.Dispose();
            mutex = null;

            base.OnExit(e);
        }

    }
}
