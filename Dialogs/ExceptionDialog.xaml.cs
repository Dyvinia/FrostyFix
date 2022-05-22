using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FrostyFix4.Dialogs {
    /// <summary>
    /// Interaction logic for ExceptionWindow.xaml
    /// </summary>
    public partial class ExceptionDialog : Window {
        public ExceptionDialog(Exception ex, string title, bool isCrash, string messagePrefix) {
            InitializeComponent();

            Title = title;
            Window mainWindow = Application.Current.MainWindow;
            Icon = mainWindow.Icon;

            // Calculate Window Height
            int headerHeight = 30;
            int height = 220;
            if (!isCrash) {
                headerHeight = 5;
                height -= 25;
                HeaderText.Visibility = Visibility.Collapsed;
            }
            Header.Height = new GridLength(headerHeight);
            Height = height + headerHeight;

            // Create exception message
            string message = ex.Message;
            if (messagePrefix != null) 
                message = messagePrefix + Environment.NewLine + message;
            if (ex.InnerException != null)
                message += Environment.NewLine + Environment.NewLine + ex.InnerException;
            ExceptionText.Text = message;

            if (isCrash) CloseButton.Click += (s, e) => Environment.Exit(0);
            else CloseButton.Click += (s, e) => this.Close();
            CopyButton.Click += (s, e) => Clipboard.SetText(message);

            SystemSounds.Hand.Play();
        }

        public static void Show(Exception ex, string title, bool isCrash = false, string messagePrefix = null) {
            Application.Current.Dispatcher.Invoke(() => {
                ExceptionDialog window = new ExceptionDialog(ex, title, isCrash, messagePrefix);
                window.ShowDialog();
            });
        }
    }
}
