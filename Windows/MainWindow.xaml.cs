using FrostyFix5.Dialogs;
using Gapotchenko.FX.Diagnostics;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FrostyFix5 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public class GameListItem {
            public string DisplayName { get; set; }
            public string FileName { get; set; }
            public string Path { get; set; }
        }
        public ObservableCollection<GameListItem> GameList = new ObservableCollection<GameListItem>();

        public static class Platforms {
            public static string Origin { get; set; }
            public static string EADesktop { get; set; }
            public static string EpicGames { get; set; }
        }


        public MainWindow() {
            InitializeComponent();

            GameSelectorDropdown.ItemsSource = GameList;
            GameSelectorDropdown.DisplayMemberPath = "DisplayName";
            GameSelectorDropdown.SelectionChanged += (s, e) => CheckModData();
            MouseDown += (s, e) => FocusManager.SetFocusedElement(this, this);

            CheckVersion();
            LocateInstalls();
            CheckStatus();
            CheckLaunchEnable();
            RefreshLaunchButton();
            LoadSelections();

            Thread checkGameStatus = new Thread(GameStatusThread);
            checkGameStatus.IsBackground = true;
            checkGameStatus.Start();
        }

        public void CheckVersion() {
            try {
                using (var client = new WebClient()) {
                    client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                    client.Headers.Add(HttpRequestHeader.UserAgent, "request");

                    dynamic results = JsonConvert.DeserializeObject<dynamic>(client.DownloadString("https://api.github.com/repos/Dyvinia/FrostyFix/releases/latest"));
                    string latestVersionString = results.tag_name;
                    var latestVersion = new Version(latestVersionString.Substring(1));
                    var version = new Version(Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    
                    var result = version.CompareTo(latestVersion);
                    if (result < 0) {
                        string message = "You are using an outdated version of FrostyFix. \nWould you like to download the latest version?";
                        MessageBoxResult Result = MessageBoxDialog.Show(message, this.Title, MessageBoxButton.YesNo, DialogSound.Notify);
                        if (Result == MessageBoxResult.Yes) {
                            Process.Start("https://github.com/Dyvinia/FrostyFix/releases/latest");
                        }
                    }
                }
            }
            catch (Exception e) {
                ExceptionDialog.Show(e, this.Title, false, "Unable to check for updates:");
            }
        }

        public void LocateInstalls() {
            // List of games & registry keys. Add a new line to add new game
            List<GameListItem> gameKeys = new List<GameListItem>();
            gameKeys.Add(new GameListItem { DisplayName = "Star Wars: Battlefront", FileName = "starwarsbattlefront", Path = @"SOFTWARE\Wow6432Node\EA Games\STAR WARS Battlefront" });
            gameKeys.Add(new GameListItem { DisplayName = "Star Wars: Battlefront II", FileName = "starwarsbattlefrontii", Path = @"SOFTWARE\EA Games\STAR WARS Battlefront II" });
            gameKeys.Add(new GameListItem { DisplayName = "Battlefield One", FileName = "bf1", Path = @"SOFTWARE\WOW6432Node\EA Games\Battlefield 1" });
            gameKeys.Add(new GameListItem { DisplayName = "Mass Effect: Andromeda", FileName = "MassEffectAndromeda", Path = @"SOFTWARE\WOW6432Node\BioWare\Mass Effect Andromeda" });
            gameKeys.Add(new GameListItem { DisplayName = "Need for Speed", FileName = "NFS16", Path = @"SOFTWARE\EA Games\Need for Speed" });
            gameKeys.Add(new GameListItem { DisplayName = "Need for Speed: Payback", FileName = "NeedForSpeedPayback", Path = @"SOFTWARE\EA Games\Need for Speed Payback" });
            gameKeys.Add(new GameListItem { DisplayName = "Plants vs. Zombies: Garden Warfare 2", FileName = "GW2.Main_Win64_Retail", Path = @"SOFTWARE\PopCap\Plants vs Zombies GW2" });
            gameKeys.Add(new GameListItem { DisplayName = "Dragon Age: Inquisition", FileName = "DragonAgeInquisition", Path = @"SOFTWARE\Wow6432Node\Bioware\Dragon Age Inquisition" });

            // Save selected Index, then clear
            int index = GameSelectorDropdown.SelectedIndex;
            GameSelectorDropdown.SelectedIndex = -1;
            GameList.Clear();

            // Fill Game List
            foreach (GameListItem game in gameKeys) {
                string path = Registry.LocalMachine.OpenSubKey(game.Path)?.GetValue("Install Dir")?.ToString();
                if (File.Exists(path + game.FileName + ".exe")) 
                    GameList.Add(new GameListItem { DisplayName = game.DisplayName, FileName = game.FileName, Path = path });
            }

            if (File.Exists(Settings.Instance.CustomGamePath)) {
                string fileName = Path.GetFileName(Settings.Instance.CustomGamePath);
                GameList.Add(new GameListItem { DisplayName = $"Custom Game ({fileName})", FileName = Path.GetFileNameWithoutExtension(fileName), Path = Path.GetDirectoryName(Settings.Instance.CustomGamePath) + "\\" });
            }

            // Restore index
            if (GameList.Count > index)
                GameSelectorDropdown.SelectedIndex = index;

            // Get Launchers
            string origin = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Origin")?.GetValue("OriginPath")?.ToString();
            if (File.Exists(origin))
                Platforms.Origin = origin;
            else
                OriginPlat.IsEnabled = false;
            
            string eaDesktop = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Electronic Arts\EA Desktop")?.GetValue("ClientPath")?.ToString();
            if (File.Exists(eaDesktop))
                Platforms.EADesktop = eaDesktop;
            else
                EADPlat.IsEnabled = false;

            string epicGames = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\EpicGames\Unreal Engine")?.GetValue("INSTALLDIR")?.ToString() + "Launcher\\Portal\\Binaries\\Win32\\EpicGamesLauncher.exe";
            if (File.Exists(epicGames))
                Platforms.EpicGames = epicGames;
            else
                EGSPlat.IsEnabled = false;

            OriginPlat.ToolTip = Platforms.Origin;
            EADPlat.ToolTip = Platforms.EADesktop;
            EGSPlat.ToolTip = Platforms.EpicGames;
        }

        public void CheckStatus() {
            // Check Global
            string dataDir = Environment.GetEnvironmentVariable("GAME_DATA_DIR", EnvironmentVariableTarget.User);
            if (!String.IsNullOrEmpty(dataDir))
                CurrentPlat.Text = "Global";

            // Check Launchers
            foreach (Process p in Process.GetProcessesByName("EADesktop")) {
                StringDictionary env = p.ReadEnvironmentVariables();
                if (env.ContainsKey("GAME_DATA_DIR")) {
                    dataDir = env["GAME_DATA_DIR"];
                    CurrentPlat.Text = "EA Desktop";
                }

            }
            foreach (Process p in Process.GetProcessesByName("EpicGamesLauncher")) {
                StringDictionary env = p.ReadEnvironmentVariables();
                if (env.ContainsKey("GAME_DATA_DIR")) {
                    dataDir = env["GAME_DATA_DIR"];
                    CurrentPlat.Text = "Epic Games Launcher";
                }
            }
            foreach (Process p in Process.GetProcessesByName("Origin")) {
                StringDictionary env = p.ReadEnvironmentVariables();
                if (env.ContainsKey("GAME_DATA_DIR")) {
                    dataDir = env["GAME_DATA_DIR"];
                    CurrentPlat.Text = "Origin";
                }
            }

            // Nothing found
            if (String.IsNullOrEmpty(dataDir)) {
                CurrentGame.Visibility = Visibility.Collapsed;
                CurrentPlat.Visibility = Visibility.Collapsed;
                CurrentPack.Visibility = Visibility.Collapsed;
                return;
            }

            // Check for invalid path
            if (dataDir == "\\ModData" || !dataDir.Contains("ModData")) {
                string message = "Invalid ModData path found";
                MessageBoxDialog.Show(message, this.Title, MessageBoxButton.OK, DialogSound.Error);
                DisableMods();
                return;
            }

            // Get Game & Pack Info
            GameListItem game = GameList.FirstOrDefault(s => dataDir.Contains(s.Path));

            if (game != null)
                CurrentGame.Text = game.DisplayName;
            else
                CurrentGame.Text = "Custom Game";

            CurrentGame.ToolTip = dataDir;
            CurrentPack.Text = new DirectoryInfo(dataDir).Name;

            CurrentGame.Visibility = Visibility.Visible;
            CurrentPlat.Visibility = Visibility.Visible;
            CurrentPack.Visibility = Visibility.Visible;
        }

        public void GameStatusThread() {
            bool found = false;
            while (true) {
                while (Settings.Instance.BackgroundThread) {
                    Process[] games = GameList.SelectMany(game => Process.GetProcessesByName(game.FileName)).ToArray();

                    if (games.Length != 0) {
                        try {
                            foreach (Process process in games) {
                                string game = GameList.Where(file => file.FileName == process.ProcessName).FirstOrDefault().DisplayName;
                                var env = process.ReadEnvironmentVariables();
                                string[] args = process.ReadArgumentList().ToArray();
                                string pack;
                                if (env["GAME_DATA_DIR"] != null) pack = new DirectoryInfo(env["GAME_DATA_DIR"]).Name;
                                else if (args.Length > 2) pack = new DirectoryInfo(args[2]).Name;
                                else pack = "None";

                                if (found != true)
                                    new ToastContentBuilder()
                                        .AddText(game + " is running with profile: " + pack)
                                        .Show();
                                found = true;
                            }
                        }
                        catch (Exception ex) {
                            Settings.Instance.BackgroundThread = false;
                            Settings.Save();

                            string title = "FrostyFix 5";
                            Task.Run(() => {
                                ExceptionDialog.Show(ex, title, false, "Background thread has encountered an error and has been disabled:");
                            });
                        }
                    }
                    else found = false;

                    if (found) Thread.Sleep(15000);
                    else Thread.Sleep(5000);
                }
            }
        }

        public void CheckModData() {
            PackList.Items.Clear();

            // Check to see if valid item
            if (GameSelectorDropdown.SelectedItem == null) return;

            // Fill list
            string path = (GameSelectorDropdown.SelectedItem as GameListItem).Path + "ModData\\";
            Directory.CreateDirectory(path);

            if (Directory.Exists(path + "\\Data") || (Directory.GetDirectories(path).Length == 0)) 
                PackList.Items.Add("ModData");

            else 
                foreach (string dir in Directory.GetDirectories(path))
                    PackList.Items.Add(new DirectoryInfo(dir).Name);

            PackList.SelectedIndex = 0;
            CheckLaunchEnable();
        }

        public async void CheckLaunchEnable() {
            await Task.Delay(100);
            bool isChecked = EADPlat.IsChecked == true || EGSPlat.IsChecked == true || OriginPlat.IsChecked == true || GlobalPlat.IsChecked == true;
            LaunchButton.IsEnabled = GameSelectorDropdown.SelectedItem != null && isChecked;
        }

        public async void LaunchWithMods() {
            DisableMods();

            Mouse.OverrideCursor = Cursors.Wait;

            // Locate ModData
            string path = (GameSelectorDropdown.SelectedItem as GameListItem).Path + "ModData\\";
            string pack = PackList.SelectedItem.ToString();
            string packPath = path + pack;

            if (pack.Contains("ModData") && PackList.SelectedIndex == 0) 
                packPath = path;

            if (GlobalPlat.IsChecked == false) {
                Process p = new Process();
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/C set \"GAME_DATA_DIR=" + packPath + "\" && start \"\" \"";
                if (OriginPlat.IsChecked == true) {
                    p.StartInfo.Arguments += Platforms.Origin;
                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(Platforms.Origin);
                }
                else if (EADPlat.IsChecked == true) {
                    p.StartInfo.Arguments += Platforms.EADesktop;
                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(Platforms.EADesktop);
                }
                else if (EGSPlat.IsChecked == true) {
                    p.StartInfo.Arguments += Platforms.EpicGames;
                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(Platforms.EpicGames);
                }
                p.StartInfo.Arguments += "\"";
                p.Start();
            }
            else Environment.SetEnvironmentVariable("GAME_DATA_DIR", packPath, EnvironmentVariableTarget.User);

            Mouse.OverrideCursor = null;
            await Task.Delay(4000);
            CheckStatus();
        }

        public async void LaunchGame() {
            await Task.Delay(5000);

            if (Settings.Instance.LaunchGame == true && Settings.Instance.FrostyPath != null) {
                string pack = PackList.SelectedItem.ToString();
                using (Process frosty = new Process()) {
                    frosty.StartInfo.FileName = Settings.Instance.FrostyPath;
                    frosty.StartInfo.UseShellExecute = false;
                    frosty.StartInfo.WorkingDirectory = Path.GetDirectoryName(Settings.Instance.FrostyPath);
                    frosty.StartInfo.Arguments = "-launch \"" + pack + "\"";
                    frosty.Start();
                }
            }
        }

        public void RefreshLaunchButton() {
            if (GlobalPlat.IsChecked == true) {
                if (Settings.Instance.LaunchGame && Settings.Instance.FrostyPath != null)
                    LaunchButtonContent.Text = "Enable Mods Globally & Launch Game";
                else 
                    LaunchButtonContent.Text = "Enable Mods Globally";
            }
            else {
                if (Settings.Instance.LaunchGame && Settings.Instance.FrostyPath != null)
                    LaunchButtonContent.Text = "Launch Game with Mods Enabled";
                else 
                    LaunchButtonContent.Text = "Launch with Mods Enabled";
            }
        }

        public async void DisableMods() {
            Mouse.OverrideCursor = Cursors.Wait;
            Environment.SetEnvironmentVariable("GAME_DATA_DIR", "", EnvironmentVariableTarget.User);

            foreach (Process process in Process.GetProcessesByName("EADesktop")) process.Kill();
            foreach (Process process in Process.GetProcessesByName("Origin")) process.Kill();
            foreach (Process process in Process.GetProcessesByName("EpicGamesLauncher")) process.Kill();
            foreach (Process process in Process.GetProcessesByName("steam")) process.Kill();

            await Task.Delay(2000);
            Mouse.OverrideCursor = null;
            CheckStatus();
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e) {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
            Settings.Save();
            LocateInstalls();
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e) {
            SaveSelections();
            LaunchWithMods();
            if (Settings.Instance.LaunchGame == true) LaunchGame();
        }

        private void DisableButton_Click(object sender, RoutedEventArgs e) {
            SaveSelections();
            DisableMods();
        }

        private void Plat_Checked(object sender, RoutedEventArgs e) {
            CheckLaunchEnable();
            RefreshLaunchButton();
        }

        private void SaveSelections() {
            List<RadioButton> radioButtons = new List<RadioButton> { EADPlat, EGSPlat, OriginPlat, GlobalPlat };

            Settings.Instance.SelectedGame = GameSelectorDropdown.SelectedIndex;
            Settings.Instance.SelectedPlatform = radioButtons.IndexOf(radioButtons.FirstOrDefault(r => (bool)r.IsChecked));

            Settings.Save();
        }

        private void LoadSelections() {
            List<RadioButton> radioButtons = new List<RadioButton> { EADPlat, EGSPlat, OriginPlat, GlobalPlat };

            if (Settings.Instance.SelectedGame > -1)
                if (Settings.Instance.SelectedGame < GameSelectorDropdown.Items.Count - 1)
                    GameSelectorDropdown.SelectedIndex = Settings.Instance.SelectedGame;

            if (Settings.Instance.SelectedPlatform > -1)
                radioButtons[Settings.Instance.SelectedPlatform].IsChecked = true;
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}
