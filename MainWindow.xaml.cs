using FrostyFix4.Properties;
using Gapotchenko.FX.Diagnostics;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace FrostyFix4 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        string bf2015;
        string bf2017;
        string mea;
        string bf1;
        string nfs;
        string nfspayback;
        string gw2;
        string dai;
        string datadir;
        string origindir;
        string eaddir;
        string epicdir;
        string isenabled;
        public static bool ifLaunchGame;
        SettingsWindow settingsWindow = new SettingsWindow();

        public MainWindow() {
            InitializeComponent();
            locatePaths();
            checkStatus();
            checkLaunchEnable();
            refreshSettings();
            checkVersion();

            Thread checkGameStatus = new Thread(checkGameStatusThread);
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
                        MessageBoxResult Result = MessageBox.Show("You are using an outdated version of FrostyFix 4." + Environment.NewLine + "Would you like to download the latest version?", "Outdated", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (Result == MessageBoxResult.Yes) {
                            Process.Start("https://github.com/Dyvinia/FrostyFix/releases/latest");
                        }
                    }
                }
            }
            catch (Exception e) {
                e = e.InnerException;
                //MessageBox.Show(e.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void locatePaths() {
            // Game Paths
            using (RegistryKey bf2015key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\EA Games\STAR WARS Battlefront"))
                if (bf2015key != null) {
                    bf2015 = (string)bf2015key.GetValue("Install Dir");
                }
                else {
                    GSD_BF2015.Visibility = Visibility.Collapsed;
                }

            using (RegistryKey bf2017key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\EA Games\STAR WARS Battlefront II"))
                if (bf2017key != null) {
                    bf2017 = (string)bf2017key.GetValue("Install Dir");
                }
                else {
                    GSD_BF2017.Visibility = Visibility.Collapsed;
                }

            using (RegistryKey bf1key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\EA Games\Battlefield 1"))
                if (bf1key != null) {
                    bf1 = (string)bf1key.GetValue("Install Dir");
                }
                else {
                    GSD_BF1.Visibility = Visibility.Collapsed;
                }

            using (RegistryKey meakey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\BioWare\Mass Effect Andromeda"))
                if (meakey != null) {
                    mea = (string)meakey.GetValue("Install Dir");
                }
                else {
                    GSD_MEA.Visibility = Visibility.Collapsed;
                }

            using (RegistryKey nfskey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\EA Games\Need for Speed"))
                if (nfskey != null) {
                    nfs = (string)nfskey.GetValue("Install Dir");
                }
                else {
                    GSD_NFS.Visibility = Visibility.Collapsed;
                }

            using (RegistryKey nfspaybackkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\EA Games\Need for Speed Payback"))
                if (nfspaybackkey != null) {
                    nfspayback = (string)nfspaybackkey.GetValue("Install Dir");
                }
                else {
                    GSD_NFSPayback.Visibility = Visibility.Collapsed;
                }

            using (RegistryKey gw2key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\PopCap\Plants vs Zombies GW2"))
                if (gw2key != null) {
                    gw2 = (string)gw2key.GetValue("Install Dir");
                }
                else {
                    GSD_GW2.Visibility = Visibility.Collapsed;
                }

            using (RegistryKey daikey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Bioware\Dragon Age Inquisition"))
                if (daikey != null) {
                    dai = (string)daikey.GetValue("Install Dir");
                }
                else {
                    GSD_DAI.Visibility = Visibility.Collapsed;
                }

            //Get Launcher paths
            using (RegistryKey origindirkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Electronic Arts\EA Core"))
                if (origindirkey != null) {
                    origindir = (string)origindirkey.GetValue("EADM6InstallDir");
                }
                else {
                    OriginPlat.IsEnabled = false;
                    //OriginPlat.Foreground = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
                }
            using (RegistryKey eaddirkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Electronic Arts\EA Desktop"))
                if (eaddirkey != null) {
                    eaddir = (string)eaddirkey.GetValue("DesktopAppPath");
                }
                else {
                    EADPlat.IsEnabled = false;
                    //EADPlat.Foreground = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
                }
            using (RegistryKey epicdirkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\EpicGames\Unreal Engine"))
                if (epicdirkey != null) {
                    epicdir = (string)epicdirkey.GetValue("INSTALLDIR");
                }
                else {
                    EGSPlat.IsEnabled = false;
                    //EGSPlat.Foreground = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
                }
        }

        public void checkStatus() {
            isenabled = Environment.GetEnvironmentVariable("GAME_DATA_DIR", EnvironmentVariableTarget.User);
            Process[] origin = Process.GetProcessesByName("Origin");
            Process[] eadesktop = Process.GetProcessesByName("EADesktop");
            Process[] epicgames = Process.GetProcessesByName("EpicGamesLauncher");

            if (eadesktop.Length != 0) {
                foreach (var process in eadesktop) {
                    var env = process.ReadEnvironmentVariables();
                    isenabled = env["GAME_DATA_DIR"];
                    lbl_platform.Text = "EA Desktop";
                }
            }
            if (epicgames.Length != 0 && isenabled == null) {
                foreach (var process in epicgames) {
                    var env = process.ReadEnvironmentVariables();
                    isenabled = env["GAME_DATA_DIR"];
                    lbl_platform.Text = "Epic Games Launcher";
                }
            }
            if (origin.Length != 0 && isenabled == null) {
                foreach (var process in origin) {
                    var env = process.ReadEnvironmentVariables();
                    isenabled = env["GAME_DATA_DIR"];
                    lbl_platform.Text = "Origin";
                }
            }
            if (isenabled == Environment.GetEnvironmentVariable("GAME_DATA_DIR", EnvironmentVariableTarget.User)) {
                lbl_platform.Text = "Global";
            }
            if (isenabled == null) {
                lbl_platform.Text = "";
            }


            if (isenabled != null) {
                string frostyprofile = new DirectoryInfo(isenabled).Name;
                lbl_enabled_tooltip.Visibility = Visibility.Visible;
                lbl_enabled_tooltip.Content = isenabled;
                lbl_profile.Text = frostyprofile;
                if (isenabled == "\\ModData" || !isenabled.Contains("ModData")) {
                    lbl_enabled.Text = "User Error when selecting path. Please click Disable Mods and try again";
                }
                else if (bf2015 != null && isenabled.Contains(bf2015)) {
                    lbl_enabled.Text = "Star Wars: Battlefront";
                }
                else if (bf2017 != null && isenabled.Contains(bf2017)) {
                    lbl_enabled.Text = "Star Wars: Battlefront II";
                }
                else if (mea != null && isenabled.Contains(mea)) {
                    lbl_enabled.Text = "Mass Effect: Andromeda";
                }
                else if (bf1 != null && isenabled.Contains(bf1)) {
                    lbl_enabled.Text = "Battlefield One";
                }
                else if (nfs != null && isenabled.Contains(nfs)) {
                    lbl_enabled.Text = "Need for Speed";
                }
                else if (nfspayback != null && isenabled.Contains(nfspayback)) {
                    lbl_enabled.Text = "Need for Speed: Payback";
                }
                else if (gw2 != null && isenabled.Contains(gw2)) {
                    lbl_enabled.Text = "PvZ: Garden Warfare 2";
                }
                else if (dai != null && isenabled.Contains(dai)) {
                    lbl_enabled.Text = "Dragon Age: Inquisition";
                }
                else {
                    lbl_enabled.Text = "Custom Game";
                }
            }
            else {
                lbl_enabled.Text = "";
                lbl_enabled_tooltip.Content = "";
                lbl_enabled_tooltip.Visibility = Visibility.Hidden;
                lbl_profile.Text = "";
            }

        }

        public void checkGameStatusThread() {
            bool found = false;
            while (true) {

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

        public void checkModData() {
            Directory.CreateDirectory(datadir + "\\ModData");
            ProfileList.Items.Clear();

            if (Directory.Exists(datadir + "\\ModData\\Data") || (Directory.GetDirectories(datadir + "\\ModData").Length == 0)) ProfileList.Items.Add("ModData");

            else {
                String[] dirs = Directory.GetDirectories(datadir + "\\ModData\\");
                int i;
                for (i = 0; i < dirs.Length; i++) {
                    var dirName = new DirectoryInfo(dirs[i]).Name;
                    ProfileList.Items.Add(dirName);
                }
            }
            ProfileList.SelectedIndex = 0;
            checkLaunchEnable();
        }

        public async void checkLaunchEnable() {
            await Task.Delay(100);
            if (GameSelectorDropdown.SelectedItem != null && (EADPlat.IsChecked == true || EGSPlat.IsChecked == true || OriginPlat.IsChecked == true || GlobalPlat.IsChecked == true)) {
                LaunchButton.IsEnabled = true;
            }
            else {
                LaunchButton.IsEnabled = false;
            }
        }

        public async void launchWithMods() {
            disableMods();

            Mouse.OverrideCursor = Cursors.Wait;

            //Find ModData dir
            dynamic profile = ProfileList.SelectedItem as dynamic;
            string moddatadir = datadir + "ModData\\" + profile;
            if (profile.Contains("ModData") && ProfileList.SelectedIndex == 0) moddatadir = datadir + "ModData\\";
            

            if (GlobalPlat.IsChecked == false) {
                Process p = new Process();
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.FileName = "cmd.exe";
                if (OriginPlat.IsChecked == true) {
                    p.StartInfo.Arguments = "/C set \"GAME_DATA_DIR=" + moddatadir + "\\\" && start \"\" \"" + origindir + "\\Origin.exe\"";
                    p.StartInfo.WorkingDirectory = origindir;
                }
                else if (EADPlat.IsChecked == true) {
                    p.StartInfo.Arguments = "/C set \"GAME_DATA_DIR=" + moddatadir + "\" && start \"\" \"" + Path.GetDirectoryName(eaddir) + "\\EADesktop.exe\"";
                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(eaddir);
                }
                else if (EGSPlat.IsChecked == true) {
                    p.StartInfo.Arguments = "/C set \"GAME_DATA_DIR=" + moddatadir + "\\\" && start \"\" \"" + epicdir + "Launcher\\Portal\\Binaries\\Win32\\EpicGamesLauncher.exe\"";
                    p.StartInfo.WorkingDirectory = epicdir + "Launcher\\Portal\\Binaries\\Win32\\";
                }
                p.Start();
            }
            else {
                Environment.SetEnvironmentVariable("GAME_DATA_DIR", moddatadir, EnvironmentVariableTarget.User);
            }

            Mouse.OverrideCursor = null;
            await Task.Delay(4000);
            checkStatus();
        }

        public async void launchGame() {
            await Task.Delay(5000);

            //string[] files = Directory.GetFiles(datadir, "*.exe");
            //string game = files[0];
            //Process.Start(game);

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

        public void refreshSettings() {

            if ((bool)GlobalPlat.IsChecked) {
                if (Settings.Default.launchGame == true && Settings.Default.frostyPath != null) {
                    LaunchButton_text.Text = "Enable Mods Globally & Launch Game";
                }
                else LaunchButton_text.Text = "Enable Mods Globally";
            }
            else {
                if (Settings.Default.launchGame == true && Settings.Default.frostyPath != null) {
                    LaunchButton_text.Text = "Launch Game with Mods Enabled";
                }
                else LaunchButton_text.Text = "Launch with Mods Enabled";
            }
        }

        public async void disableMods() {
            Mouse.OverrideCursor = Cursors.Wait;
            Environment.SetEnvironmentVariable("GAME_DATA_DIR", "", EnvironmentVariableTarget.User);
            foreach (var process in Process.GetProcessesByName("EADesktop")) {
                process.Kill();
            }
            foreach (var process in Process.GetProcessesByName("Origin")) {
                process.Kill();
            }
            foreach (var process in Process.GetProcessesByName("EpicGamesLauncher")) {
                process.Kill();
            }
            foreach (var process in Process.GetProcessesByName("steam")) {
                process.Kill();
            }
            await Task.Delay(2000);
            Mouse.OverrideCursor = null;
            checkStatus();
        }


        private void window_MouseDown(object sender, MouseButtonEventArgs e) {
            Keyboard.ClearFocus();
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e) {
            settingsWindow.Show();
            settingsWindow.Focus();
        }

        private void GSD_BF2015_Selected(object sender, RoutedEventArgs e) {
            datadir = bf2015;
            checkModData();
        }

        private void GSD_BF2017_Selected(object sender, RoutedEventArgs e) {
            datadir = bf2017;
            checkModData();
        }

        private void GSD_BF1_Selected(object sender, RoutedEventArgs e) {
            datadir = bf1;
            checkModData();
        }

        private void GSD_MEA_Selected(object sender, RoutedEventArgs e) {
            datadir = mea;
            checkModData();
        }

        private void GSD_NFS_Selected(object sender, RoutedEventArgs e) {
            datadir = nfs;
            checkModData();
        }

        private void GSD_NFSPayback_Selected(object sender, RoutedEventArgs e) {
            datadir = nfspayback;
            checkModData();
        }

        private void GSD_GW2_Selected(object sender, RoutedEventArgs e) {
            datadir = gw2;
            checkModData();
        }

        private void GSD_DAI_Selected(object sender, RoutedEventArgs e) {
            datadir = dai;
            checkModData();
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e) {
            launchWithMods();
            if (Settings.Default.launchGame == true) launchGame();
        }

        private void DisableButton_Click(object sender, RoutedEventArgs e) {
            disableMods();
        }

        private void Plat_Checked(object sender, RoutedEventArgs e) {
            checkLaunchEnable();
            refreshSettings();
        }
    }
}
