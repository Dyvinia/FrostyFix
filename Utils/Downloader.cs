using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DyviniaUtils {

    class Downloader {
        /// <summary>
        /// Downloads file to destination
        /// </summary>
        public static async Task Download(string downloadUrl, string destinationFilePath, IProgress<double> progress) {
            using HttpClient httpClient = new() { Timeout = TimeSpan.FromMinutes(30) };
            using HttpResponseMessage response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();
            long? totalBytes = response.Content.Headers.ContentLength;

            using Stream contentStream = await response.Content.ReadAsStreamAsync();
            long? totalBytesRead = 0L;
            long readCount = 0L;
            byte[] buffer = new byte[4096];
            bool isMoreToRead = true;

            using var fileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);

            do {
                int bytesRead = await contentStream.ReadAsync(buffer);
                if (bytesRead == 0) {
                    isMoreToRead = false;
                    progress.Report((double)((double)totalBytesRead / totalBytes));
                    continue;
                }

                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));

                totalBytesRead += bytesRead;
                readCount++;

                if (readCount % 100 == 0) {
                    progress.Report((double)((double)totalBytesRead / totalBytes));
                }
            }
            while (isMoreToRead);
        }

        /// <summary>
        /// Shows Progress window while Downloading file to destination
        /// </summary>
        public static async Task DownloadWithWindow(string downloadUrl, string destinationFilePath) {
            DownloadWindow downloadWindow = new("Downloading");
            downloadWindow.Show();
            await Download(downloadUrl, destinationFilePath, downloadWindow.Progress);
            await Task.Delay(100);
            downloadWindow.Close();
        }

        private class DownloadWindow : Window {
            public IProgress<double> Progress;

            public DownloadWindow(string title) {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ResizeMode = ResizeMode.NoResize;
                WindowStyle = WindowStyle.None;
                AllowsTransparency = true;
                Background = Brushes.Transparent;
                Cursor = Cursors.Wait;

                Title = title;
                Height = 100;
                Width = 400;

                Grid innerGrid = new() {
                    Background = new BrushConverter().ConvertFromString("#FF141414") as Brush,
                    Margin = new Thickness(5)
                };

                TextBlock labelText = new() {
                    Text = title,
                    FontWeight = FontWeights.Bold,
                    FontSize = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                ProgressBar progressBar = new() {
                    Height = 5,
                    Maximum = 1,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    BorderThickness = new Thickness(),
                    Background = Brushes.Transparent,
                    Foreground = Brushes.White,
                };
                Progress = new Progress<double>(p => progressBar.Value = p);

                innerGrid.Children.Add(labelText);
                innerGrid.Children.Add(progressBar);

                Grid rootGrid = new() {
                    Background = new BrushConverter().ConvertFromString("#FF2D2D2D") as Brush
                };
                rootGrid.Children.Add(innerGrid);

                Content = rootGrid;
            }
        }
    }
}
