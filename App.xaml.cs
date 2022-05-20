using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FrostyFix4 {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public App() {
            DispatcherUnhandledException += Application_DispatcherUnhandledException;
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            string title = "FrostyFix 4";
            if (e.Exception.InnerException != null)
                MessageBox.Show(e.Exception.Message + Environment.NewLine + Environment.NewLine + e.Exception.InnerException, title, MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show(e.Exception.Message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            App.Current.Shutdown();
        }
    }
}
