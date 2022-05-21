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
            public string Path { get; set; }
        }

        public ObservableCollection<GameListItem> gameList = new ObservableCollection<GameListItem>();

        public Dictionary<string, string> platforms = new Dictionary<string, string> {
            ["Origin"] = null,
            ["EA Desktop"] = null,
            ["Epic Games"] = null
        };

        public MainWindow() {
            InitializeComponent();

            GameSelectorDropdown.ItemsSource = gameList;
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
            Dictionary<string, string> gameKeys = new Dictionary<string, string>();

            gameKeys.Add("Star Wars: Battlefront", @"SOFTWARE\Wow6432Node\EA Games\STAR WARS Battlefront");
            gameKeys.Add("Star Wars: Battlefront II", @"SOFTWARE\EA Games\STAR WARS Battlefront II");
            gameKeys.Add("Battlefield One", @"SOFTWARE\WOW6432Node\EA Games\Battlefield 1");
            gameKeys.Add("Mass Effect: Andromeda", @"SOFTWARE\WOW6432Node\BioWare\Mass Effect Andromeda");
            gameKeys.Add("Need for Speed", @"SOFTWARE\EA Games\Need for Speed");
            gameKeys.Add("Need for Speed: Payback", @"SOFTWARE\EA Games\Need for Speed Payback");
            gameKeys.Add("Plants vs. Zombies: Garden Warfare 2", @"SOFTWARE\PopCap\Plants vs Zombies GW2");
            gameKeys.Add("Dragon Age: Inquisition", @"SOFTWARE\Wow6432Node\Bioware\Dragon Age Inquisition");

            foreach (var game in gameKeys) {
                RegistryKey path = Registry.LocalMachine.OpenSubKey(game.Value);
                if (path != null) gameList.Add(new GameListItem { DisplayName = game.Key, Path = path.GetValue("Install Dir").ToString() });
            }

            //Get Launcher paths
            using (RegistryKey origin = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Electronic Arts\EA Core"))
                if (origin != null) platforms["Origin"] = (string)origin.GetValue("EADM6InstallDir");
                else OriginPlat.IsEnabled = false;

            using (RegistryKey eaDesktop = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Electronic Arts\EA Desktop"))
                if (eaDesktop != null) platforms["EA Desktop"] = (string)eaDesktop.GetValue("DesktopAppPath");
                else EADPlat.IsEnabled = false;

            using (RegistryKey epicGames = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\EpicGames\Unreal Engine"))
                if (epicGames != null) platforms["Epic Games"] = (string)epicGames.GetValue("INSTALLDIR");
                else EGSPlat.IsEnabled = false;
        }

        public string getSelectedPath() {
            return ((GameListItem)GameSelectorDropdown.SelectedItem).Path;
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
                    Process[] swbf2 = Process.GetProcessesByName("starwarsbattlefrontii");

                    if (swbf2.Length != 0) {
                        foreach (var process in swbf2) {
                            string game = "Star Wars Battlefront 2";
                            var env = process.ReadEnvironmentVariables();
                            string profile = "";
                            if (env["GAME_DATA_DIR"] != null) profile = new DirectoryInfo(env["GAME_DATA_DIR"]).Name;
                            else profile = "None";

                            if (found != true)
                                new ToastContentBuilder()
                                    .AddText(game + " is running with profile: " + profile)
                                    .Show();
                            found = true;
                        }
                    }
                    else found = false;

                    Thread.Sleep(6000);
                }
            }
        }

        public void checkModData() {
            string path = getSelectedPath();
            Directory.CreateDirectory(path + "\\ModData");
            ProfileList.Items.Clear();

            if (Directory.Exists(path + "\\ModData\\Data") || (Directory.GetDirectories(path + "\\ModData").Length == 0)) 
                ProfileList.Items.Add("ModData");

            else {
                foreach (string dir in Directory.GetDirectories(path + "\\ModData\\"))
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
            dynamic profile = ProfileList.SelectedItem as dynamic;
            string path = getSelectedPath();
            string moddataPath = path + "ModData\\" + profile;

            if (profile.Contains("ModData") && ProfileList.SelectedIndex == 0) 
                moddataPath = path + "ModData\\";

            if (GlobalPlat.IsChecked == false) {
                Process platform = new Process();
                platform.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                platform.StartInfo.FileName = "cmd.exe";
                if (OriginPlat.IsChecked == true) {
                    platform.StartInfo.Arguments = "/C set \"GAME_DATA_DIR=" + moddataPath + "\\\" && start \"\" \"" + platforms["Origin"] + "\\Origin.exe\"";
                    platform.StartInfo.WorkingDirectory = platforms["Origin"];
                }
                else if (EADPlat.IsChecked == true) {
                    platform.StartInfo.Arguments = "/C set \"GAME_DATA_DIR=" + moddataPath + "\" && start \"\" \"" + Path.GetDirectoryName(platforms["EA Desktop"]) + "\\EADesktop.exe\"";
                    platform.StartInfo.WorkingDirectory = Path.GetDirectoryName(platforms["EA Desktop"]);
                }
                else if (EGSPlat.IsChecked == true) {
                    platform.StartInfo.Arguments = "/C set \"GAME_DATA_DIR=" + moddataPath + "\\\" && start \"\" \"" + platforms["Epic Games"] + "Launcher\\Portal\\Binaries\\Win32\\EpicGamesLauncher.exe\"";
                    platform.StartInfo.WorkingDirectory = platforms["Epic Games"] + "Launcher\\Portal\\Binaries\\Win32\\";
                }
                platform.Start();
            }
            else Environment.SetEnvironmentVariable("GAME_DATA_DIR", moddataPath, EnvironmentVariableTarget.User);

            Mouse.OverrideCursor = null;
            await Task.Delay(4000);
            checkStatus();
        }

        public async void launchGame() {
            await Task.Delay(5000);

            if (Settings.Default.launchGame == true && Settings.Default.frostyPath != null) {
                dynamic profile = ProfileList.SelectedItem as dynamic;
                using (Process frostyLaunch = new Process()) {
                    frostyLaunch.StartInfo.FileName = Settings.Default.frostyPath;
                    frostyLaunch.StartInfo.UseShellExecute = false;
                    frostyLaunch.StartInfo.WorkingDirectory = Path.GetDirectoryName(Settings.Default.frostyPath);
                    frostyLaunch.StartInfo.Arguments = "-launch \"" + profile + "\"";
                    frostyLaunch.Start();
                }
            }
        }

        public void refreshLaunchButton() {
            if ((bool)GlobalPlat.IsChecked) {
                if (Settings.Default.launchGame == true && Settings.Default.frostyPath != null)
                    LaunchButton_text.Text = "Enable Mods Globally & Launch Game";
                else 
                    LaunchButton_text.Text = "Enable Mods Globally";
            }
            else {
                if (Settings.Default.launchGame == true && Settings.Default.frostyPath != null)
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

            GameSelectorDropdown.SelectedIndex = Settings.Default.selectedGame;
            radioButtons[Settings.Default.selectedPlatform].IsChecked = true;

            Settings.Default.Save();
        }

        private void GameSelectorDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            checkModData();
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}
