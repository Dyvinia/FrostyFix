﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Win32;

namespace FrostyFix {
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window {
        public SettingsWindow() {
            InitializeComponent();

            VersionText.Text = App.Version;
            DataContext = Config.Settings;

            MouseDown += (s, e) => FocusManager.SetFocusedElement(this, this);

            ResetButton.Click += (s, e) => {
                Config.Reset();
                Config.Save();
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
            OpenFileDialog dialog = new() {
                Title = "Select Frosty EXE",
                Filter = "Frosty (*.exe) |*.exe",
                FilterIndex = 2
            };
            if (dialog.ShowDialog() == true) {
                Config.Settings.FrostyPath = dialog.FileName;
            }
        }

        private void CustomGameSelect_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new() {
                Title = "Select Game EXE",
                Filter = "Game Executable (*.exe) |*.exe",
                FilterIndex = 2
            };
            if (dialog.ShowDialog() == true) {
                Config.Settings.CustomGamePath = dialog.FileName;
            }
        }

        private void CreditButton_Click(object sender, RoutedEventArgs e) {
            string url;
            switch ((sender as Button).Content) {
                case "Dyvinia": url = "https://github.com/Dyvinia/"; break;
                case "BattleDash": url = "https://battleda.sh/"; break;
                case "VictorPL": url = "https://twitter.com/VictorPL2003/"; break;
                default: return;
            }
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);

            if (e.Key == Key.F12)
                Process.Start("explorer.exe", $"/select, {Config.FilePath}");
        }
    }
}
