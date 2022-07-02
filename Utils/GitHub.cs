using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using Newtonsoft.Json;
using DyviniaUtils.Dialogs;


namespace DyviniaUtils {
    class GitHub {
        public async static void CheckVersion(string repoAuthor, string repoName) {
            try {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Add("User-Agent", "request");

                dynamic json = JsonConvert.DeserializeObject<dynamic>(await client.GetStringAsync($"https://api.github.com/repos/{repoAuthor}/{repoName}/releases/latest"));
                Version latest = new(((string)json.tag_name)[1..]);
                Version local = Assembly.GetExecutingAssembly().GetName().Version;

                if (local.CompareTo(latest) < 0) {
                    string message = $"You are using {repoName} v{local.ToString()[..5]}. \nWould you like to download the latest version? (v{latest})";
                    MessageBoxResult Result = MessageBoxDialog.Show(message, repoName, MessageBoxButton.YesNo, DialogSound.Notify);
                    if (Result == MessageBoxResult.Yes) {
                        Process.Start(new ProcessStartInfo($"https://github.com/{repoAuthor}/{repoName}/releases/latest") { UseShellExecute = true });
                    }
                }
            }
            catch (Exception e) {
                ExceptionDialog.Show(e, repoName, false, "Unable to check for updates:");
            }
        }
    }
}
