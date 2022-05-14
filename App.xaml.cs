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
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            if (e.Exception.InnerException != null)
                MessageBox.Show(e.Exception.Message + Environment.NewLine + Environment.NewLine + "Inner Exception:" + Environment.NewLine + e.Exception.InnerException, "Frosty Fix 4", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show(e.Exception.Message, "Frosty Fix 4", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            App.Current.Shutdown();
        }
    }
}
