using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FrostyFix5.Dialogs;
using PropertyChanged;

namespace FrostyFix5 {

    [AddINotifyPropertyChangedInterface]
    public class Settings : SettingsManager<Settings> {
        public bool LaunchGame { get; set; } = false;
        public bool BackgroundThread { get; set; } = true;

        public string FrostyPath { get; set; }
        public string CustomGamePath { get; set; }

        public int SelectedGame { get; set; } = -1;
        public int SelectedPlatform { get; set; } = -1;
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public static readonly string Version = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 5);

        public App() {
            Settings.Load();

            DispatcherUnhandledException += Application_DispatcherUnhandledException;
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            e.Handled = true;
            string title = "FrostyFix 5";
            ExceptionDialog.Show(e.Exception, title, true);
        }
    }
}
