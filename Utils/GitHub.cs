using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using DyviniaUtils.Dialogs;

namespace DyviniaUtils {
    class GitHub {

        class Release {
            public string tag_name { get; set; }

            public Asset[] assets { get; set; }

            public class Asset {
                public string name { get; set; }
                public string browser_download_url { get; set; }
            }
        }


        /// <summary>
        /// Checks Github if there is a newer version
        /// </summary>
        public static async Task<bool> CheckVersion(string repoAuthor, string repoName) {
            try {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Add("User-Agent", "request");
                Release json = JsonSerializer.Deserialize<Release>(await client.GetStringAsync($"https://api.github.com/repos/{repoAuthor}/{repoName}/releases/latest"));

                Version latest = new(json.tag_name[1..]);
                Version local = Assembly.GetExecutingAssembly().GetName().Version;

                if (local.CompareTo(latest) < 0)
                    return true;
                else 
                    return false;
            }
            catch (Exception e) {
                ExceptionDialog.Show(e, repoName, "Unable to check for updates:");
                return false;
            }
        }

        /// <summary>
        /// Downloads and Installs newest version
        /// </summary>
        public static async Task InstallUpdate(string repoAuthor, string repoName) {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "request");
            Release json = JsonSerializer.Deserialize<Release>(await client.GetStringAsync($"https://api.github.com/repos/{repoAuthor}/{repoName}/releases/latest"));

            string downloadUrl = json.assets.Where(x => x.browser_download_url.Contains(".exe")).FirstOrDefault().browser_download_url;

            string filePath = Environment.ProcessPath;
            string oldPath = filePath.Replace(".exe", ".old.exe");

            File.Move(filePath, oldPath, true);

            await Downloader.DownloadWithWindow(downloadUrl, filePath);

            Process.Start(new ProcessStartInfo { FileName = filePath, UseShellExecute = true });
            Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
        }
    }
}
