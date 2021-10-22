using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace FrostyFix4 {
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window {
        public SettingsWindow() {
            InitializeComponent();
            loadSettings();
            refresh();

            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 5);
            txt_Version.Text = "v" + version;
        }


        public void loadSettings() {
            chkbLaunchGame.IsChecked = Properties.Settings.Default.launchGame;
        }

        public void refresh() {
            Properties.Settings.Default.launchGame = (bool)chkbLaunchGame.IsChecked;
            Properties.Settings.Default.Save();
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

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            TabItem ti = Tabs.SelectedItem as TabItem;
            this.Title = "FrostyFix 4: " + ti.Header;
        }
    }
}
