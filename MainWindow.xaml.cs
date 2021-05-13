using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;

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

        public void checkStatus() {
            var isenabled = Environment.GetEnvironmentVariable("GAME_DATA_DIR", EnvironmentVariableTarget.User);
            if (isenabled != null) {
                string frostyprofile = new DirectoryInfo(isenabled).Name;
                lbl_enabled.Foreground = Brushes.LightGreen;
                if (isenabled == "\\ModData") {
                    lbl_enabled.Text = "Registry Key is Currently Broken";
                    lbl_enabled.Foreground = Brushes.Orange;
                }
                else if (isenabled.Contains(bf2015)) {
                    lbl_enabled.Text = "Mods are Currently Enabled for Star Wars: Battlefront (2015) using profile: " + frostyprofile;
                }
                else if (isenabled.Contains(bf2017)) {
                    lbl_enabled.Text = "Mods are Currently Enabled for Star Wars: Battlefront II (2017) using profile: " + frostyprofile;
                }
                else if (isenabled.Contains(mea)) {
                    lbl_enabled.Text = "Mods are Currently Enabled for Mass Effect: Andromeda using profile: " + frostyprofile;
                }
                else if (isenabled.Contains(bf1)) {
                    lbl_enabled.Text = "Mods are Currently Enabled for Battlefield One using profile: " + frostyprofile;
                }
                else if (isenabled.Contains(nfs)) {
                    lbl_enabled.Text = "Mods are Currently Enabled for Need for Speed using profile: " + frostyprofile;
                }
                else if (isenabled.Contains(nfspayback)) {
                    lbl_enabled.Text = "Mods are Currently Enabled for Need for Speed: Payback using profile: " + frostyprofile;
                }
                else if (isenabled.Contains(gw2)) {
                    lbl_enabled.Text = "Mods are Currently Enabled for PvZ: Garden Warfare 2 using profile: " + frostyprofile;
                }
                else if (isenabled.Contains(dai)) {
                    lbl_enabled.Text = "Mods are Currently Enabled for Dragon Age: Inquisition using profile: " + frostyprofile;
                }
                else {
                    lbl_enabled.Text = "Mods are Currently Enabled for Custom Game using profile: " + frostyprofile;
                }
            }
            else {
                lbl_enabled.Text = "Mods are Currently NOT Enabled";
                lbl_enabled.Foreground = Brushes.LightSalmon;
            }
        }

        private async void btn_enable_Click(object sender, RoutedEventArgs e) {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.Title = "Select Profile";
            dialog.InitialDirectory = datadir + "ModData\\";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                Mouse.OverrideCursor = Cursors.Wait;
                Environment.SetEnvironmentVariable("GAME_DATA_DIR", dialog.FileName, EnvironmentVariableTarget.User);
                await Task.Delay(10);
                Mouse.OverrideCursor = null;
                checkStatus();
                AfterPatchWindow afterpatch = new AfterPatchWindow();
                afterpatch.Show();
            }
        }

        private async void btn_disable_Click(object sender, RoutedEventArgs e) {
            Mouse.OverrideCursor = Cursors.Wait;
            Environment.SetEnvironmentVariable("GAME_DATA_DIR", "", EnvironmentVariableTarget.User);
            await Task.Delay(10);
            Mouse.OverrideCursor = null;
            checkStatus();
            AfterPatchWindow afterpatch = new AfterPatchWindow();
            afterpatch.Show();
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

            checkStatus();
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
            btn_enable.IsEnabled = true;
            btn_disable.IsEnabled = true;
            OpenFileDialog openFileDlg = new OpenFileDialog();
            {
                openFileDlg.Filter = "Game executable (*.exe)|*.exe";
                openFileDlg.FilterIndex = 2;
                openFileDlg.RestoreDirectory = true;

                Nullable<bool> result = openFileDlg.ShowDialog();

                if (result == true) {
                    datadir = System.IO.Path.GetDirectoryName(openFileDlg.FileName);
                    txtb_custompath.Text = datadir;
                }
            }
        }

        private void rbtn_bf2015_Checked(object sender, RoutedEventArgs e) {
            datadir = bf2015;
            btn_enable.IsEnabled = true;
            btn_disable.IsEnabled = true;
        }

        private void rbtn_bf2017_Checked(object sender, RoutedEventArgs e) {
            datadir = bf2017;
            btn_enable.IsEnabled = true;
            btn_disable.IsEnabled = true;
        }

        private void rbtn_bf1_Checked(object sender, RoutedEventArgs e) {
            datadir = bf1;
            btn_enable.IsEnabled = true;
            btn_disable.IsEnabled = true;
        }

        private void rbtn_mea_Checked(object sender, RoutedEventArgs e) {
            datadir = mea;
            btn_enable.IsEnabled = true;
            btn_disable.IsEnabled = true;
        }

        private void rbtn_nfs_Checked(object sender, RoutedEventArgs e) {
            datadir = nfs;
            btn_enable.IsEnabled = true;
            btn_disable.IsEnabled = true;
        }

        private void rbtn_nfspayback_Checked(object sender, RoutedEventArgs e) {
            datadir = nfspayback;
            btn_enable.IsEnabled = true;
            btn_disable.IsEnabled = true;
        }

        private void rbtn_gw2_Checked(object sender, RoutedEventArgs e) {
            datadir = gw2;
            btn_enable.IsEnabled = true;
            btn_disable.IsEnabled = true;
        }

        private void rbtn_dai_Checked(object sender, RoutedEventArgs e) {
            datadir = dai;
            btn_enable.IsEnabled = true;
            btn_disable.IsEnabled = true;
        }
    }
}