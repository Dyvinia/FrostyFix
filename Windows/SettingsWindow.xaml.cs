using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace FrostyFix5 {
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window {
        public SettingsWindow() {
            InitializeComponent();

            VersionText.Text = App.Version;
            DataContext = Settings.Instance;

            MouseDown += (s, e) => FocusManager.SetFocusedElement(this, this);

            ResetButton.Click += (s, e) => {
                Settings.Reset();
                Settings.Save();
            };
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            Process.Start(e.Uri.ToString());
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            TabItem tab = Tabs.SelectedItem as TabItem;
            this.Title = "FrostyFix 5: " + tab.Header;
        }

        private void FrostySelect_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select Frosty EXE";
            dialog.Filter = "Frosty (*.exe) |*.exe";
            dialog.FilterIndex = 2;
            if (dialog.ShowDialog() == true) {
                Settings.Instance.FrostyPath = dialog.FileName;
            }
        }

        private void CustomGameSelect_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select Game EXE";
            dialog.Filter = "Game Executable (*.exe) |*.exe";
            dialog.FilterIndex = 2;
            if (dialog.ShowDialog() == true) {
                Settings.Instance.CustomGamePath = dialog.FileName;
            }
        }
    }
}
