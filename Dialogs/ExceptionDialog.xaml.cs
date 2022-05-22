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
    public enum Sound {
        None,
        Asterik,
        Beep,
        Exclamation,
        Hand,
        Question
    }

    /// <summary>
    /// Interaction logic for ExceptionWindow.xaml
    /// </summary>
    public partial class ExceptionDialog : Window {
        public ExceptionDialog(Exception ex, string title, bool isCrash, string messagePrefix, Sound sound = Sound.Hand) {
            InitializeComponent();
            PlaySound(sound);

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
            else CloseButton.Click += (s, e) => Close();
            CopyButton.Click += (s, e) => Clipboard.SetText(message);
        }

        public static void Show(Exception ex, string title, bool isCrash = false, string messagePrefix = null, Sound sound = Sound.Hand) {
            Application.Current.Dispatcher.Invoke(() => {
                ExceptionDialog window = new ExceptionDialog(ex, title, isCrash, messagePrefix, sound);
                window.ShowDialog();
            });
        }

        private void PlaySound(Sound sound) {
            switch (sound) {
                case Sound.None:
                    break;
                case Sound.Asterik:
                    SystemSounds.Exclamation.Play();
                    break;
                case Sound.Beep:
                    SystemSounds.Beep.Play();
                    break;
                case Sound.Exclamation:
                    SystemSounds.Exclamation.Play();
                    break;
                case Sound.Hand:
                    SystemSounds.Hand.Play();
                    break;
                case Sound.Question:
                    SystemSounds.Question.Play();
                    break;
            }
        }
    }
}
