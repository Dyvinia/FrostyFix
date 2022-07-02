using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using PropertyChanged;
using Newtonsoft.Json;
using DyviniaUtils;
using DyviniaUtils.Dialogs;

namespace FrostyFix {

    [AddINotifyPropertyChangedInterface]
    public class Config : SettingsManager<Config> {
        public bool LaunchGame { get; set; } = false;
        public bool BackgroundThread { get; set; } = true;
        public bool UpdateChecker { get; set; } = true;

        public string FrostyPath { get; set; }
        public string CustomGamePath { get; set; }

        public int SelectedGame { get; set; } = -1;
        public int SelectedPlatform { get; set; } = -1;
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public static readonly string Version = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString()[..5];
        public static readonly string AppName = Assembly.GetEntryAssembly().GetName().Name;

        public App() {
            Config.Load();

            if (Config.Settings.UpdateChecker) 
                GitHub.CheckVersion("Dyvinia", "FrostyFix");

            DispatcherUnhandledException += Application_DispatcherUnhandledException;
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            e.Handled = true;
            string title = "FrostyFix";
            ExceptionDialog.Show(e.Exception, title, true);
        }

    }
}
