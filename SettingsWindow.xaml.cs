using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FrostyFix4 {
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window {
        public SettingsWindow() {
            InitializeComponent();

            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 5);
            txt_Version.Text = "v" + version;
        }

        public void refresh() {

        }

        private void chkbLaunchGame_Checked(object sender, RoutedEventArgs e) {
            refresh();
        }

        private void window_MouseDown(object sender, MouseButtonEventArgs e) {
            Keyboard.ClearFocus();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
