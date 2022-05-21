using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FrostyFix4 {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public static string Version;

        public App() {
            Version = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 5);

            DispatcherUnhandledException += Application_DispatcherUnhandledException;
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            string title = "FrostyFix 4";
            string message = e.Exception.Message;
            if (e.Exception.InnerException != null)
                message += Environment.NewLine + Environment.NewLine + e.Exception.InnerException;
            Clipboard.SetText(message);

            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);

            e.Handled = true;
            App.Current.Shutdown();
        }
    }
}
