using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FrostyFix2 {
    /// <summary>
    /// Interaction logic for AfterPatchWindow.xaml
    /// </summary>
    public partial class AfterPatchWindow : Window {
        string origindir;
        string eaddir;

        public AfterPatchWindow() {
            InitializeComponent();
            //Get Origin and/or EA Desktop paths
            using (RegistryKey origindirkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Electronic Arts\EA Core"))
                if (origindirkey != null) {
                    origindir = (string)origindirkey.GetValue("EADM6InstallDir");
                }
                else {
                    btn_restartOrigin.IsEnabled = false;
                }
            using (RegistryKey eaddirkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Electronic Arts\EA Desktop"))
                if (eaddirkey != null) {
                    eaddir = (string)eaddirkey.GetValue("DesktopAppPath");
                }
                else {
                    btn_restartEA.IsEnabled = false;
                }
            CloseButton_AfterPatch.Click += (s, e) => Close();
        }

        private async void btn_restartEA_Click(object sender, RoutedEventArgs e) {
            //Kill all EA processes
            foreach (var process in Process.GetProcessesByName("EADesktop")) {
                process.Kill();
            }
            foreach (var process in Process.GetProcessesByName("Origin")) {
                process.Kill();
            }
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            await Task.Delay(8000);
            Mouse.OverrideCursor = null;
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.Verb = "runas";
            p.StartInfo.WorkingDirectory = Path.GetDirectoryName(eaddir);
            p.StartInfo.FileName = Path.GetDirectoryName(eaddir) + "\\EALauncher.exe";
            p.Start();
            this.Close();
        }

        private async void btn_restartOrigin_Click(object sender, RoutedEventArgs e) {
            //Kill all EA processes
            foreach (var process in Process.GetProcessesByName("EADesktop")) {
                process.Kill();
            }
            foreach (var process in Process.GetProcessesByName("Origin")) {
                process.Kill();
            }
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            await Task.Delay(8000);
            Mouse.OverrideCursor = null;
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.WorkingDirectory = origindir;
            p.StartInfo.FileName = origindir + "\\Origin.exe";
            p.Start();
            this.Close();
        }

        private async void btn_restartPC_Click(object sender, RoutedEventArgs e) {
            string message = "Do you want to restart this PC?";
            string title = "Restart PC";
            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result = MessageBox.Show(message, title, buttons, icon);
            switch (result) {
                case MessageBoxResult.Yes:
                    await Task.Delay(2000);
                    System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0");
                    break;
            }
        }
    }
}
