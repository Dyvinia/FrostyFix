using FrostyFix4.Properties;
using Gapotchenko.FX.Diagnostics;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;

namespace FrostyFix4 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public class GameListItem {
            public string DisplayName { get; set; }
            public string FileName { get; set; }
            public string Path { get; set; }
        }
        public ObservableCollection<GameListItem> gameList = new ObservableCollection<GameListItem>();

        public class Platforms {
            public string Origin { get; set; }
            public string EADesktop { get; set; }
            public string EpicGames { get; set; }
        }
        public Platforms platforms = new Platforms();


        public MainWindow() {
            InitializeComponent();

            GameSelectorDropdown.ItemsSource = gameList;
            GameSelectorDropdown.DisplayMemberPath = "DisplayName";
            GameSelectorDropdown.SelectionChanged += (s, e) => checkModData();
            MouseDown += (s, e) => Keyboard.ClearFocus();

            checkVersion();
            locatePaths();
            checkStatus();
            checkLaunchEnable();
            refreshLaunchButton();
            loadSelections();

            Thread checkGameStatus = new Thread(gameStatusThread);
            checkGameStatus.IsBackground = true;
            checkGameStatus.Start();
        }

        public void checkVersion() {
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
                        MessageBoxResult Result = MessageBox.Show("You are using an outdated version of FrostyFix 4." + Environment.NewLine + "Would you like to download the latest version?", this.Title, MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (Result == MessageBoxResult.Yes) {
                            Process.Start("https://github.com/Dyvinia/FrostyFix/releases/latest");
                        }
                    }
                }
            }
            catch (Exception e) {
                string message = "Unable to check updates:\n" + e.Message;
                if (e.InnerException != null)
                    message += Environment.NewLine + Environment.NewLine + e.InnerException;
                Clipboard.SetText(message);

                MessageBox.Show(message, this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void locatePaths() {
            // Get Game Paths
            List<GameListItem> gameKeys = new List<GameListItem>();

            gameKeys.Add(new GameListItem { DisplayName = "Star Wars: Battlefront", FileName = "starwarsbattlefront", Path = @"SOFTWARE\Wow6432Node\EA Games\STAR WARS Battlefront" });
            gameKeys.Add(new GameListItem { DisplayName = "Star Wars: Battlefront II", FileName = "starwarsbattlefrontii", Path = @"SOFTWARE\EA Games\STAR WARS Battlefront II" });
            gameKeys.Add(new GameListItem { DisplayName = "Battlefield One", FileName = "bf1", Path = @"SOFTWARE\WOW6432Node\EA Games\Battlefield 1" });
            gameKeys.Add(new GameListItem { DisplayName = "Mass Effect: Andromeda", FileName = "MassEffectAndromeda", Path = @"SOFTWARE\WOW6432Node\BioWare\Mass Effect Andromeda" });
            gameKeys.Add(new GameListItem { DisplayName = "Need for Speed", FileName = "NFS16", Path = @"SOFTWARE\EA Games\Need for Speed" });
            gameKeys.Add(new GameListItem { DisplayName = "Need for Speed: Payback", FileName = "NeedForSpeedPayback", Path = @"SOFTWARE\EA Games\Need for Speed Payback" });
            gameKeys.Add(new GameListItem { DisplayName = "Plants vs. Zombies: Garden Warfare 2", FileName = "GW2.Main_Win64_Retail", Path = @"SOFTWARE\PopCap\Plants vs Zombies GW2" });
            gameKeys.Add(new GameListItem { DisplayName = "Dragon Age: Inquisition", FileName = "DragonAgeInquisition", Path = @"SOFTWARE\Wow6432Node\Bioware\Dragon Age Inquisition" });

            foreach (GameListItem game in gameKeys) {
                RegistryKey path = Registry.LocalMachine.OpenSubKey(game.Path);
                if (path != null) gameList.Add(new GameListItem { DisplayName = game.DisplayName, FileName = game.FileName, Path = path.GetValue("Install Dir").ToString() });
            }

            //Get Launcher paths
            using (RegistryKey origin = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Origin"))
                if (origin != null) platforms.Origin = origin.GetValue("OriginPath").ToString();
                else OriginPlat.IsEnabled = false;

            using (RegistryKey eaDesktop = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Electronic Arts\EA Desktop"))
                if (eaDesktop != null) platforms.EADesktop = eaDesktop.GetValue("ClientPath").ToString();
                else EADPlat.IsEnabled = false;

            using (RegistryKey epicGames = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\EpicGames\Unreal Engine"))
                if (epicGames != null) platforms.EpicGames = epicGames.GetValue("INSTALLDIR").ToString() + "Launcher\\Portal\\Binaries\\Win32\\EpicGamesLauncher.exe\"";
                else EGSPlat.IsEnabled = false;
        }

        public string getSelectedModData() {
            return ((GameListItem)GameSelectorDropdown.SelectedItem).Path + "ModData\\";
        }

        public void checkStatus() {
            string enabledPath = Environment.GetEnvironmentVariable("GAME_DATA_DIR", EnvironmentVariableTarget.User);
            Process[] origin = Process.GetProcessesByName("Origin");
            Process[] eadesktop = Process.GetProcessesByName("EADesktop");
            Process[] epicgames = Process.GetProcessesByName("EpicGamesLauncher");

            // Get Launcher Info
            if (eadesktop.Length != 0) {
                foreach (Process process in eadesktop) {
                    var env = process.ReadEnvironmentVariables();
                    enabledPath = env["GAME_DATA_DIR"];
                    lbl_platform.Text = "EA Desktop";
                }
            }
            if (epicgames.Length != 0 && enabledPath == null) {
                foreach (Process process in epicgames) {
                    var env = process.ReadEnvironmentVariables();
                    enabledPath = env["GAME_DATA_DIR"];
                    lbl_platform.Text = "Epic Games Launcher";
                }
            }
            if (origin.Length != 0 && enabledPath == null) {
                foreach (Process process in origin) {
                    var env = process.ReadEnvironmentVariables();
                    enabledPath = env["GAME_DATA_DIR"];
                    lbl_platform.Text = "Origin";
                }
            }
            if (enabledPath == Environment.GetEnvironmentVariable("GAME_DATA_DIR", EnvironmentVariableTarget.User))
                lbl_platform.Text = "Global";
            if (enabledPath == null)
                lbl_platform.Text = "";

            // Get Game & Pack Info
            if (enabledPath != null) {
                GameListItem match = gameList.FirstOrDefault(s => enabledPath.Contains(s.Path));
                string pack = new DirectoryInfo(enabledPath).Name;

                lbl_enabled.Visibility = Visibility.Visible;
                lbl_profile.Visibility = Visibility.Visible;
                lbl_profile.Text = pack;
                lbl_enabled_tooltip.Content = enabledPath;

                if (match != null) 
                    lbl_enabled.Text = match.DisplayName;
                else if (enabledPath == "\\ModData" || !enabledPath.Contains("ModData")) 
                    lbl_enabled.Text = "User Error when selecting path. Please click Disable Mods and try again";
                else 
                    lbl_enabled.Text = "Custom Game";
            }
            else {
                lbl_enabled.Visibility = Visibility.Collapsed;
                lbl_profile.Visibility = Visibility.Collapsed;
            }
        }

        public void gameStatusThread() {
            bool found = false;

            while (true) {
                while (Settings.Default.backgroundThread) {
                    Process[] games = gameList.SelectMany(game => Process.GetProcessesByName(game.FileName)).ToArray();

                    if (games.Length != 0) {
                        foreach (Process process in games) {
                            string game = gameList.Where(file => file.FileName == process.ProcessName).FirstOrDefault().DisplayName;
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
                    else found = false;

                    if (found == true) Thread.Sleep(15000);
                    else Thread.Sleep(5000);
                }
            }
        }

        public void checkModData() {
            string path = getSelectedModData();
            Directory.CreateDirectory(path);
            ProfileList.Items.Clear();

            if (Directory.Exists(path + "\\Data") || (Directory.GetDirectories(path).Length == 0)) 
                ProfileList.Items.Add("ModData");

            else {
                foreach (string dir in Directory.GetDirectories(path))
                    ProfileList.Items.Add(new DirectoryInfo(dir).Name);
            }

            ProfileList.SelectedIndex = 0;
            checkLaunchEnable();
        }

        public async void checkLaunchEnable() {
            await Task.Delay(100);
            bool isChecked = EADPlat.IsChecked == true || EGSPlat.IsChecked == true || OriginPlat.IsChecked == true || GlobalPlat.IsChecked == true;
            LaunchButton.IsEnabled = GameSelectorDropdown.SelectedItem != null && isChecked;
        }

        public async void launchWithMods() {
            disableMods();

            Mouse.OverrideCursor = Cursors.Wait;

            // Locate ModData
            string path = getSelectedModData();
            string pack = ProfileList.SelectedItem.ToString();
            string packPath = path + pack;

            if (pack.Contains("ModData") && ProfileList.SelectedIndex == 0) 
                packPath = path;

            if (GlobalPlat.IsChecked == false) {
                Process p = new Process();
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/C set \"GAME_DATA_DIR=" + packPath + "\" && start \"\" \"";
                if (OriginPlat.IsChecked == true) {
                    p.StartInfo.Arguments += platforms.Origin;
                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(platforms.Origin);
                }
                else if (EADPlat.IsChecked == true) {
                    p.StartInfo.Arguments += platforms.EADesktop;
                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(platforms.EADesktop);
                }
                else if (EGSPlat.IsChecked == true) {
                    p.StartInfo.Arguments += platforms.EpicGames;
                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(platforms.EpicGames);
                }
                p.Start();
            }
            else Environment.SetEnvironmentVariable("GAME_DATA_DIR", packPath, EnvironmentVariableTarget.User);

            Mouse.OverrideCursor = null;
            await Task.Delay(4000);
            checkStatus();
        }

        public async void launchGame() {
            await Task.Delay(5000);

            if (Settings.Default.launchGame == true && Settings.Default.frostyPath != null) {
                string pack = ProfileList.SelectedItem.ToString();
                using (Process frosty = new Process()) {
                    frosty.StartInfo.FileName = Settings.Default.frostyPath;
                    frosty.StartInfo.UseShellExecute = false;
                    frosty.StartInfo.WorkingDirectory = Path.GetDirectoryName(Settings.Default.frostyPath);
                    frosty.StartInfo.Arguments = "-launch \"" + pack + "\"";
                    frosty.Start();
                }
            }
        }

        public void refreshLaunchButton() {
            if (GlobalPlat.IsChecked == true) {
                if (Settings.Default.launchGame && Settings.Default.frostyPath != null)
                    LaunchButton_text.Text = "Enable Mods Globally & Launch Game";
                else 
                    LaunchButton_text.Text = "Enable Mods Globally";
            }
            else {
                if (Settings.Default.launchGame && Settings.Default.frostyPath != null)
                    LaunchButton_text.Text = "Launch Game with Mods Enabled";
                else 
                    LaunchButton_text.Text = "Launch with Mods Enabled";
            }
        }

        public async void disableMods() {
            Mouse.OverrideCursor = Cursors.Wait;
            Environment.SetEnvironmentVariable("GAME_DATA_DIR", "", EnvironmentVariableTarget.User);

            foreach (var process in Process.GetProcessesByName("EADesktop")) process.Kill();
            foreach (var process in Process.GetProcessesByName("Origin")) process.Kill();
            foreach (var process in Process.GetProcessesByName("EpicGamesLauncher")) process.Kill();
            foreach (var process in Process.GetProcessesByName("steam")) process.Kill();

            await Task.Delay(2000);
            Mouse.OverrideCursor = null;
            checkStatus();
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e) {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e) {
            saveSelections();
            launchWithMods();
            if (Settings.Default.launchGame == true) launchGame();
        }

        private void DisableButton_Click(object sender, RoutedEventArgs e) {
            disableMods();
        }

        private void Plat_Checked(object sender, RoutedEventArgs e) {
            checkLaunchEnable();
            refreshLaunchButton();
        }

        private void saveSelections() {
            List<RadioButton> radioButtons = new List<RadioButton> { EADPlat, EGSPlat, OriginPlat, GlobalPlat };

            Settings.Default.selectedGame = GameSelectorDropdown.SelectedIndex;
            Settings.Default.selectedPlatform = radioButtons.IndexOf(radioButtons.FirstOrDefault(r => (bool)r.IsChecked));

            Settings.Default.Save();
        }

        private void loadSelections() {
            List<RadioButton> radioButtons = new List<RadioButton> { EADPlat, EGSPlat, OriginPlat, GlobalPlat };

            if (Settings.Default.selectedGame > -1) {
                if (Settings.Default.selectedGame < GameSelectorDropdown.Items.Count - 1)
                    GameSelectorDropdown.SelectedIndex = Settings.Default.selectedGame;
            }
            
            if (Settings.Default.selectedPlatform > -1)
                radioButtons[Settings.Default.selectedPlatform].IsChecked = true;
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}
