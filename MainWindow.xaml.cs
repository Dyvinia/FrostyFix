using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using Gapotchenko.FX.Diagnostics;

namespace FrostyFix2 {
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
        string steamdir;
        bool customChoose = false;
        string isenabled;

        public void checkStatus() {
            isenabled = null;
            lbl_platform.Foreground = Brushes.LightGreen;
            Process[] origin = Process.GetProcessesByName("Origin");
            Process[] eadesktop = Process.GetProcessesByName("EADesktop");
            Process[] epicgames = Process.GetProcessesByName("EpicGamesLauncher");
            Process[] steam = Process.GetProcessesByName("steam");

            if (eadesktop.Length != 0) {
                foreach (var process in eadesktop) {
                    var env = process.ReadEnvironmentVariables();
                    isenabled = env["GAME_DATA_DIR"];
                    lbl_platform.Text = "Platform: EA Desktop";
                }
            }
            if (epicgames.Length != 0 && isenabled == null) {
                foreach (var process in epicgames) {
                    var env = process.ReadEnvironmentVariables();
                    isenabled = env["GAME_DATA_DIR"];
                    lbl_platform.Text = "Platform: Epic Games Launcher";
                }
            }
            if (steam.Length != 0 && isenabled == null) {
                foreach (var process in steam) {
                    var env = process.ReadEnvironmentVariables();
                    isenabled = env["GAME_DATA_DIR"];
                    lbl_platform.Text = "Platform: Steam";
                }
            }
            if (origin.Length != 0 && isenabled == null) {
                foreach (var process in origin) {
                    var env = process.ReadEnvironmentVariables();
                    isenabled = env["GAME_DATA_DIR"];
                    lbl_platform.Text = "Platform: Origin";
                }
            }
            if (isenabled == null) {
                isenabled = null;
                lbl_platform.Text = "";
            }

            if (isenabled != null) {
                string frostyprofile = new DirectoryInfo(isenabled).Name;
                lbl_enabled.Foreground = Brushes.LightGreen;
                lbl_profile.Foreground = Brushes.LightGreen;
                lbl_profile.Text = "Frosty Profile: " + frostyprofile;
                if (isenabled == "\\ModData" || !isenabled.Contains("ModData")) {
                    lbl_enabled.Text = "User Error when selecting path. Please click Disable Mods and try again";
                    lbl_enabled.Foreground = Brushes.Orange;
                }
                else if (bf2015 != null && isenabled.Contains(bf2015)) {
                    lbl_enabled.Text = "Game: Star Wars: Battlefront (2015)";
                }
                else if (bf2017 != null && isenabled.Contains(bf2017)) {
                    lbl_enabled.Text = "Game: Star Wars: Battlefront II (2017)";
                }
                else if (mea != null && isenabled.Contains(mea)) {
                    lbl_enabled.Text = "Game: Mass Effect: Andromeda";
                }
                else if (bf1 != null && isenabled.Contains(bf1)) {
                    lbl_enabled.Text = "Game: Battlefield One";
                }
                else if (nfs != null && isenabled.Contains(nfs)) {
                    lbl_enabled.Text = "Game: Need for Speed";
                }
                else if (nfspayback != null && isenabled.Contains(nfspayback)) {
                    lbl_enabled.Text = "Game: Need for Speed: Payback";
                }
                else if (gw2 != null && isenabled.Contains(gw2)) {
                    lbl_enabled.Text = "Game: PvZ: Garden Warfare 2";
                }
                else if (dai != null && isenabled.Contains(dai)) {
                    lbl_enabled.Text = "Game: Dragon Age: Inquisition";
                }
                else {
                    lbl_enabled.Text = "Game: Custom";
                }
            }
            else {
                lbl_enabled.Text = "Mods are Currently NOT Enabled";
                lbl_enabled.Foreground = Brushes.LightSalmon;
                lbl_profile.Text = "";
            }
        }

        public void checkEnabled() {
            bool success =
                (rbtn_bf2015.IsChecked == true || rbtn_bf2017.IsChecked == true || rbtn_bf1.IsChecked == true || rbtn_mea.IsChecked == true || rbtn_nfs.IsChecked == true || rbtn_nfspayback.IsChecked == true || rbtn_gw2.IsChecked == true || rbtn_dai.IsChecked == true || customChoose == true) &&
                (rbtn_origin.IsChecked == true || rbtn_eadesk.IsChecked == true || rbtn_epicgames.IsChecked == true || rbtn_steam.IsChecked == true);
            if (success == true) {
                btn_enable.IsEnabled = true;
            }
            else {
                btn_enable.IsEnabled = false;
            }
        }

        private async void btn_enable_Click(object sender, RoutedEventArgs e) {
            Directory.CreateDirectory(datadir + "\\ModData");
            if (Directory.GetDirectories(datadir + "\\ModData").Length == 0 || Directory.Exists(datadir + "\\ModData\\Data")) {
                if (Directory.GetDirectories(datadir + "\\ModData").Length == 0) {
                    string message = "ModData is Empty. After mods are enabled, launch the game from Frosty to generate ModData";
                    string title = "Empty ModData";
                    MessageBoxButton buttons = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBoxResult result = MessageBox.Show(message, title, buttons, icon);
                }
                Mouse.OverrideCursor = Cursors.Wait;

                //Kill all Launcher processes
                foreach (var process in Process.GetProcessesByName("EADesktop")) {
                    process.Kill();
                }
                ServiceController service = new ServiceController("EABackgroundService");
                if (service.Status != ServiceControllerStatus.Stopped) {
                    service.Stop();
                }
                foreach (var process in Process.GetProcessesByName("Origin")) {
                    process.Kill();
                }
                if (rbtn_epicgames.IsChecked == true) {
                    foreach (var process in Process.GetProcessesByName("EpicGamesLauncher")) {
                        process.Kill();
                    }
                }
                if (rbtn_steam.IsChecked == true) {
                    foreach (var process in Process.GetProcessesByName("steam")) {
                        process.Kill();
                    }
                }
                await Task.Delay(1000);

                Process p = new Process();
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.FileName = "cmd.exe";
                if (rbtn_origin.IsChecked == true) {
                    p.StartInfo.Arguments = "/C set \"GAME_DATA_DIR=" + datadir + "\\ModData\" && start \"\" \"" + origindir + "\\Origin.exe\"";
                    p.StartInfo.WorkingDirectory = origindir;
                }
                else if (rbtn_eadesk.IsChecked == true) {
                    p.StartInfo.Arguments = "/C set \"GAME_DATA_DIR=" + datadir + "\\ModData\" && start \"\" \"" + Path.GetDirectoryName(eaddir) + "\\EADesktop.exe\"";
                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(eaddir);
                }
                else if (rbtn_epicgames.IsChecked == true) {
                    p.StartInfo.Arguments = "/C set \"GAME_DATA_DIR=" + datadir + "\\ModData\" && start \"\" \"" + epicdir + "Launcher\\Portal\\Binaries\\Win32\\EpicGamesLauncher.exe\"";
                    p.StartInfo.WorkingDirectory = epicdir + "Launcher\\Portal\\Binaries\\Win32\\";
                }
                else if (rbtn_steam.IsChecked == true) {
                    p.StartInfo.Arguments = "/C set \"GAME_DATA_DIR=" + datadir + "\\ModData\" && start \"\" \"" + steamdir + "\\steam.exe\"";
                    p.StartInfo.WorkingDirectory = steamdir;
                }
                p.Start();

                Mouse.OverrideCursor = null;
                await Task.Delay(5000);
                checkStatus();
            }
            else {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.Title = "Select Profile";
                dialog.InitialDirectory = datadir + "ModData\\";
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                    Mouse.OverrideCursor = Cursors.Wait;

                    //Kill all Launcher processes
                    foreach (var process in Process.GetProcessesByName("EADesktop")) {
                        process.Kill();
                    }
                    ServiceController service = new ServiceController("EABackgroundService");
                    if (service.Status != ServiceControllerStatus.Stopped) {
                        service.Stop();
                    }
                    foreach (var process in Process.GetProcessesByName("Origin")) {
                        process.Kill();
                    }
                    if (rbtn_epicgames.IsChecked == true) {
                        foreach (var process in Process.GetProcessesByName("EpicGamesLauncher")) {
                            process.Kill();
                        }
                    }
                    if (rbtn_steam.IsChecked == true) {
                        foreach (var process in Process.GetProcessesByName("steam")) {
                            process.Kill();
                        }
                    }
                    await Task.Delay(1000);

                    Process p = new Process();
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    p.StartInfo.FileName = "cmd.exe";
                    if (rbtn_origin.IsChecked == true) {
                        p.StartInfo.Arguments = "/C set \"GAME_DATA_DIR=" + dialog.FileName + "\" && start \"\" \"" + origindir + "\\Origin.exe\"";
                        p.StartInfo.WorkingDirectory = origindir;
                    }
                    else if (rbtn_eadesk.IsChecked == true) {
                        p.StartInfo.Arguments = "/C set \"GAME_DATA_DIR=" + dialog.FileName + "\" && start \"\" \"" + Path.GetDirectoryName(eaddir) + "\\EADesktop.exe\"";
                        p.StartInfo.WorkingDirectory = Path.GetDirectoryName(eaddir);
                    }
                    else if (rbtn_epicgames.IsChecked == true) {
                        p.StartInfo.Arguments = "/C set \"GAME_DATA_DIR=" + dialog.FileName + "\" && start \"\" \"" + epicdir + "Launcher\\Portal\\Binaries\\Win32\\EpicGamesLauncher.exe\"";
                        p.StartInfo.WorkingDirectory = epicdir + "Launcher\\Portal\\Binaries\\Win32\\";
                    }
                    else if (rbtn_steam.IsChecked == true) {
                        p.StartInfo.Arguments = "/C set \"GAME_DATA_DIR=" + dialog.FileName + "\" && start \"\" \"" + steamdir + "\\steam.exe\"";
                        p.StartInfo.WorkingDirectory = steamdir;
                    }
                    p.Start();

                    Mouse.OverrideCursor = null;
                    await Task.Delay(4000);
                    checkStatus();
                }
            }
        }

        private async void btn_disable_Click(object sender, RoutedEventArgs e) {
            Mouse.OverrideCursor = Cursors.Wait;
            foreach (var process in Process.GetProcessesByName("EADesktop")) {
                process.Kill();
            }
            ServiceController service = new ServiceController("EABackgroundService");
            if (service.Status != ServiceControllerStatus.Stopped) {
                service.Stop();
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

        public MainWindow() {
            InitializeComponent();
            MinimizeButton.Click += (s, e) => WindowState = WindowState.Minimized;
            CloseButton.Click += (s, e) => Application.Current.Shutdown();

            //Get Paths using Registry
            using (RegistryKey bf2015key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\EA Games\STAR WARS Battlefront"))
                if (bf2015key != null) {
                    bf2015 = (string)bf2015key.GetValue("Install Dir");
                    rbtn_bf2015.IsEnabled = true;
                }
                else {
                    rbtn_bf2015.IsEnabled = false;
                    rbtn_bf2015.Foreground = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
                }

            using (RegistryKey bf2017key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\EA Games\STAR WARS Battlefront II"))
                if (bf2017key != null) {
                    bf2017 = (string)bf2017key.GetValue("Install Dir");
                    rbtn_bf2017.IsEnabled = true;
                }
                else {
                    rbtn_bf2017.IsEnabled = false;
                    rbtn_bf2017.Foreground = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
                }

            using (RegistryKey bf1key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\EA Games\Battlefield 1"))
                if (bf1key != null) {
                    bf1 = (string)bf1key.GetValue("Install Dir");
                    rbtn_bf1.IsEnabled = true;
                }
                else {
                    rbtn_bf1.IsEnabled = false;
                    rbtn_bf1.Foreground = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
                }

            using (RegistryKey meakey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\BioWare\Mass Effect Andromeda"))
                if (meakey != null) {
                    mea = (string)meakey.GetValue("Install Dir");
                    rbtn_mea.IsEnabled = true;
                }
                else {
                    rbtn_mea.IsEnabled = false;
                    rbtn_mea.Foreground = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
                }

            using (RegistryKey nfskey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\EA Games\Need for Speed"))
                if (nfskey != null) {
                    nfs = (string)nfskey.GetValue("Install Dir");
                    rbtn_nfs.IsEnabled = true;
                }
                else {
                    rbtn_nfs.IsEnabled = false;
                    rbtn_nfs.Foreground = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
                }

            using (RegistryKey nfspaybackkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\EA Games\Need for Speed Payback"))
                if (nfspaybackkey != null) {
                    nfspayback = (string)nfspaybackkey.GetValue("Install Dir");
                    rbtn_nfspayback.IsEnabled = true;
                }
                else {
                    rbtn_nfspayback.IsEnabled = false;
                    rbtn_nfspayback.Foreground = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
                }

            using (RegistryKey gw2key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\PopCap\Plants vs Zombies GW2"))
                if (gw2key != null) {
                    gw2 = (string)gw2key.GetValue("Install Dir");
                    rbtn_gw2.IsEnabled = true;
                }
                else {
                    rbtn_gw2.IsEnabled = false;
                    rbtn_gw2.Foreground = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
                }

            using (RegistryKey daikey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Bioware\Dragon Age Inquisition"))
                if (daikey != null) {
                    dai = (string)daikey.GetValue("Install Dir");
                    rbtn_dai.IsEnabled = true;
                }
                else {
                    rbtn_dai.IsEnabled = false;
                    rbtn_dai.Foreground = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
                }

            //Get Launcher paths
            using (RegistryKey origindirkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Electronic Arts\EA Core"))
                if (origindirkey != null) {
                    origindir = (string)origindirkey.GetValue("EADM6InstallDir");
                }
                else {
                    rbtn_origin.IsEnabled = false;
                    rbtn_origin.Foreground = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
                }
            using (RegistryKey eaddirkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Electronic Arts\EA Desktop"))
                if (eaddirkey != null) {
                    eaddir = (string)eaddirkey.GetValue("DesktopAppPath");
                }
                else {
                    rbtn_eadesk.IsEnabled = false;
                    rbtn_eadesk.Foreground = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
                }
            using (RegistryKey epicdirkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\EpicGames\Unreal Engine"))
                if (epicdirkey != null) {
                    epicdir = (string)epicdirkey.GetValue("INSTALLDIR");
                }
                else {
                    rbtn_epicgames.IsEnabled = false;
                    rbtn_epicgames.Foreground = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
                }
            using (RegistryKey steamdirkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam"))
                if (steamdirkey != null) {
                    steamdir = (string)steamdirkey.GetValue("InstallPath");
                }
                else {
                    rbtn_steam.IsEnabled = false;
                    rbtn_steam.Foreground = new SolidColorBrush(Color.FromArgb(255, 140, 140, 140));
                }

            checkStatus();
            checkEnabled();
        }

        public void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
            if (this.WindowState == WindowState.Maximized) {
                this.BorderThickness = new System.Windows.Thickness(8);
            }
            else {
                this.BorderThickness = new System.Windows.Thickness(0);
            }
        }

        private void ButtonGithub(object sender, RoutedEventArgs e) {
            AboutWindow about = new AboutWindow();
            about.Show();
        }

        private void rbtn_custom_Checked(object sender, RoutedEventArgs e) {
            btn_customchoose.IsEnabled = true;
        }

        private void rbtn_custom_Unchecked(object sender, RoutedEventArgs e) {
            btn_customchoose.IsEnabled = false;
        }

        private void btn_customchoose_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            {
                openFileDlg.Filter = "Game executable (*.exe)|*.exe";
                openFileDlg.FilterIndex = 2;
                openFileDlg.RestoreDirectory = true;

                Nullable<bool> result = openFileDlg.ShowDialog();

                if (result == true) {
                    datadir = System.IO.Path.GetDirectoryName(openFileDlg.FileName);
                    txtb_custompath.Text = datadir;
                    customChoose = true;
                }
            }
        }

        private void rbtn_bf2015_Checked(object sender, RoutedEventArgs e) {
            datadir = bf2015;
            checkEnabled();
        }

        private void rbtn_bf2017_Checked(object sender, RoutedEventArgs e) {
            datadir = bf2017;
            checkEnabled();
        }

        private void rbtn_bf1_Checked(object sender, RoutedEventArgs e) {
            datadir = bf1;
            checkEnabled();
        }

        private void rbtn_mea_Checked(object sender, RoutedEventArgs e) {
            datadir = mea;
            checkEnabled();
        }

        private void rbtn_nfs_Checked(object sender, RoutedEventArgs e) {
            datadir = nfs;
            checkEnabled();
        }

        private void rbtn_nfspayback_Checked(object sender, RoutedEventArgs e) {
            datadir = nfspayback;
            checkEnabled();
        }

        private void rbtn_gw2_Checked(object sender, RoutedEventArgs e) {
            datadir = gw2;
            checkEnabled();
        }

        private void rbtn_dai_Checked(object sender, RoutedEventArgs e) {
            datadir = dai;
            checkEnabled();
        }

        private void rbtn_platform_Checked(object sender, RoutedEventArgs e) {
            checkEnabled();
        }

        private void btn_refresh_Click(object sender, RoutedEventArgs e) {
            checkStatus();
        }
    }
}