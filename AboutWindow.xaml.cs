using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace FrostyFix2 {
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window {
        public AboutWindow() {
            InitializeComponent();
            CloseButton_About.Click += (s, e) => Close();
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 5);
            txt_Version.Text = "v" + version;
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        private void ButtonEnvVar(object sender, RoutedEventArgs e) {
            (Application.Current.MainWindow as MainWindow).checkStatus();
        }

        private void ButtonGameDir(object sender, RoutedEventArgs e) {
            (Application.Current.MainWindow as MainWindow).openGameDir();
        }

        private void ButtonDelModData(object sender, RoutedEventArgs e) {
            (Application.Current.MainWindow as MainWindow).delModData();
        }

        private async void ButtonClearEnvVar(object sender, RoutedEventArgs e) {
            Mouse.OverrideCursor = Cursors.Wait;
            Environment.SetEnvironmentVariable("GAME_DATA_DIR", "", EnvironmentVariableTarget.User);
            await Task.Delay(10);
            Mouse.OverrideCursor = null;
        }
    }
}
