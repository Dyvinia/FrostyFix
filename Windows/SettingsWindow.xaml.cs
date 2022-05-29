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

            VersionText.Text = App.Version;

            MouseDown += (s, e) => FocusManager.SetFocusedElement(this, this);
            LaunchGameOption.Click += (s, e) => Settings.Default.Save();
            BackgroundOption.Click += (s, e) => Settings.Default.Save();
            ResetButton.Click += (s, e) => Settings.Default.Reset();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            Process.Start(e.Uri.ToString());
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            TabItem tab = Tabs.SelectedItem as TabItem;
            this.Title = "FrostyFix 4: " + tab.Header;
        }

        private void FrostySelect_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select Frosty EXE";
            dialog.Filter = "Frosty (*.exe) |*.exe";
            dialog.FilterIndex = 2;
            if (dialog.ShowDialog() == true) {
                Settings.Default.FrostyPath = dialog.FileName;
                Settings.Default.Save();
            }
        }

        private void CustomGameSelect_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select Game EXE";
            dialog.Filter = "Game Executable (*.exe) |*.exe";
            dialog.FilterIndex = 2;
            if (dialog.ShowDialog() == true) {
                Settings.Default.CustomGamePath = dialog.FileName;
                Settings.Default.Save();
            }
        }
    }
}
