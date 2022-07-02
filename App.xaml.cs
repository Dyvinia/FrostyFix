using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using PropertyChanged;
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

            DispatcherUnhandledException += ExceptionDialog.UnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e) {
            new MainWindow().Show();

            if (Config.Settings.UpdateChecker)
                GitHub.CheckVersion("Dyvinia", "FrostyFix");
        }

    }
}
