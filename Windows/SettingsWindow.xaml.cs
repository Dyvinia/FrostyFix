using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace FrostyFix {
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window {
        public SettingsWindow() {
            InitializeComponent();

            VersionText.Text = App.Version;
            DataContext = Settings.Instance;

            MouseDown += (s, e) => FocusManager.SetFocusedElement(this, this);
            KeyDown += new KeyEventHandler(KeyHandler);

            ResetButton.Click += (s, e) => {
                Settings.Reset();
                Settings.Save();
            };
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            Process.Start(new ProcessStartInfo(e.Uri.ToString()) { UseShellExecute = true });
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            TabItem tab = Tabs.SelectedItem as TabItem;
            Title = "FrostyFix: " + tab.Header;
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

        private void CreditButton_Click(object sender, RoutedEventArgs e) {
            Button button = sender as Button;
            string url;
            switch (button.Content) {
                case "Dyvinia": url = "https://github.com/Dyvinia/"; break;
                case "BattleDash": url = "https://battleda.sh/"; break;
                case "VictorPL": url = "https://twitter.com/VictorPL2003/"; break;
                default: return;
            }
            ProcessStartInfo p = new() {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(p);
        }

        private void KeyHandler(object sender, KeyEventArgs e) {
            if (e.Key == Key.F12) {
                Process.Start("explorer.exe", $"/select, {Settings.ConfigPath}");
            }
        }
    }
}
