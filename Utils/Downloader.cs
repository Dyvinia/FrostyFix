using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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
    }
}
