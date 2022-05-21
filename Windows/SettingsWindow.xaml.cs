using FrostyFix4.Properties;
using Microsoft.Win32;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
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

            txt_Version.Text = App.Version;

            MouseDown += (s, e) => Keyboard.ClearFocus();
            chkbLaunchGame.Click += (s, e) => Settings.Default.Save();
            chkbBackground.Click += (s, e) => Settings.Default.Save();
            btn_reset.Click += (s, e) => Settings.Default.Reset();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            Process.Start(e.Uri.ToString());
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            TabItem ti = Tabs.SelectedItem as TabItem;
            this.Title = "FrostyFix 4: " + ti.Header;
        }

        private void btn_frostyselect_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select Frosty EXE";
            dialog.Filter = "Frosty (*.exe) |*.exe";
            dialog.FilterIndex = 2;

            Nullable<bool> result = dialog.ShowDialog();
            if (result == true) {
                Settings.Default.frostyPath = dialog.FileName;
                Settings.Default.Save();
            }
        }
    }
}
