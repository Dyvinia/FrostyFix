using System;
using System.Collections.Generic;
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
    public enum DialogSound {
        None,
        Notify,
        Error
    }

    /// <summary>
    /// Interaction logic for MessageBoxDialog.xaml
    /// </summary>
    public partial class MessageBoxDialog : Window {

        public MessageBoxResult result = MessageBoxResult.Cancel;

        public MessageBoxDialog(string message, string title, MessageBoxButton buttons, DialogSound sound) {
            InitializeComponent();
            SetVisibilityOfButtons(buttons);
            PlaySound(sound);

            Title = title;
            MessageText.Text = message;
            Window mainWindow = Application.Current.MainWindow;
            Icon = mainWindow.Icon;

            OKButton.Click += OnClose;
            YesButton.Click += OnClose;
            NoButton.Click += OnClose;
            CancelButton.Click += OnClose;
        }

        public static MessageBoxResult Show(string message, string title, MessageBoxButton buttons, DialogSound sound = DialogSound.None) {
            MessageBoxResult msgBoxResult = MessageBoxResult.None;
            Application.Current.Dispatcher.Invoke(() => {
                MessageBoxDialog window = new MessageBoxDialog(message, title, buttons, sound);
                window.ShowDialog();
                msgBoxResult = window.result;
            });
            return msgBoxResult;
        }

        private void OnClose(object sender, RoutedEventArgs e) {
            if (sender == OKButton)
                result = MessageBoxResult.OK;
            else if (sender == YesButton)
                result = MessageBoxResult.Yes;
            else if (sender == NoButton)
                result = MessageBoxResult.No;
            else if (sender == CancelButton)
                result = MessageBoxResult.Cancel;
            else
                result = MessageBoxResult.None;
            Close();
        }

        private void SetVisibilityOfButtons(MessageBoxButton button) {
            switch (button) {
                case MessageBoxButton.OK:
                    CancelButton.Visibility = Visibility.Collapsed;
                    NoButton.Visibility = Visibility.Collapsed;
                    YesButton.Visibility = Visibility.Collapsed;
                    OKButton.Focus();
                    break;
                case MessageBoxButton.OKCancel:
                    NoButton.Visibility = Visibility.Collapsed;
                    YesButton.Visibility = Visibility.Collapsed;
                    CancelButton.Focus();
                    break;
                case MessageBoxButton.YesNo:
                    OKButton.Visibility = Visibility.Collapsed;
                    CancelButton.Visibility = Visibility.Collapsed;
                    NoButton.Focus();
                    break;
                case MessageBoxButton.YesNoCancel:
                    OKButton.Visibility = Visibility.Collapsed;
                    CancelButton.Focus();
                    break;
                default:
                    break;
            }
        }

        private void PlaySound(DialogSound sound) {
            switch (sound) {
                case DialogSound.None:
                    break;
                case DialogSound.Notify:
                    SystemSounds.Exclamation.Play();
                    break;
                case DialogSound.Error:
                    SystemSounds.Hand.Play();
                    break;
            }
        }
    }
}
