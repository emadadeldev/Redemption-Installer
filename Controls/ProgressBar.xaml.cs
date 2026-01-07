// =======================================================
// Developer: Emad Adel
// Source Code https://github.com/emadadeldev/Redemption
// =======================================================

using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EmadAdel.Redemption_Team.Controls
{
    public partial class ProgressBar : UserControl
    {
        private bool isDownloading = false;
        private string zipFilePath;

        public ProgressBar()
        {
            InitializeComponent();
            InitializeProgressBar();
        }

        private void InitializeProgressBar()
        {
            prog.Minimum = 0;
            prog.Maximum = 100;
            prog.Value = 0;
        }

        public async void StartDownload(string savePath)
        {
            if (isDownloading) return;

            string url = "https://github.com/emadadeldev/RDR2AR/archive/refs/heads/main.zip";
            string fileName = System.IO.Path.GetFileName(new Uri(url).LocalPath);
            zipFilePath = System.IO.Path.Combine(savePath, fileName);

            try
            {
                isDownloading = true;
                prog.Value = 0;
                loadingText.Text = "..جاري تنزيل أحدث نسخة من التعريب";

                await DownloadFileAsync(url, zipFilePath);

                loadingText.Text = "...جاري تثبيت التعريب";

                string extractPath = System.IO.Path.GetDirectoryName(zipFilePath);
                ExtractZipWithOverride(zipFilePath, extractPath);

                if (File.Exists(zipFilePath))
                    File.Delete(zipFilePath);

                Dispatcher.Invoke(() => prog.Value = 100);
                loadingText.Text = ".تم التثبيت بنجاح";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                isDownloading = false;
            }
        }

        private async Task DownloadFileAsync(string url, string destination)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "RDR2AR-Updater/1.0 (+https://github.com/emadadeldev)"
                );

                using (HttpResponseMessage response =
                    await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    long totalBytes = response.Content.Headers.ContentLength ?? -1L;
                    long totalRead = 0L;

                    byte[] buffer = new byte[81920];

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                    using (FileStream fileStream = new FileStream(
                        destination,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None))
                    {
                        int bytesRead;
                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalRead += bytesRead;

                            if (totalBytes > 0)
                            {
                                double progress = ((double)totalRead / totalBytes) * 100.0;
                                if (progress > 100) progress = 100;
                                int displayProgress = (int)Math.Round(progress);
                                Dispatcher.Invoke(() => prog.Value = displayProgress);
                            }
                        }
                    }
                }
            }
        }

        private void ExtractZipWithOverride(string zipPath, string extractPath)
        {
            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string relativePath = entry.FullName;
                    int index = relativePath.IndexOf('/');
                    if (index >= 0)
                        relativePath = relativePath.Substring(index + 1);

                    if (string.IsNullOrEmpty(relativePath))
                        continue;

                    string fullPath = System.IO.Path.Combine(extractPath, relativePath);

                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        Directory.CreateDirectory(fullPath);
                        continue;
                    }

                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath));

                    if (File.Exists(fullPath))
                        File.SetAttributes(fullPath, FileAttributes.Normal);

                    entry.ExtractToFile(fullPath, true);
                }
            }
        }

        public void StopDownload()
        {
            isDownloading = false;
        }

        public void GoToValue(double value)
        {
            if (value >= 0 && value <= 100)
                prog.Value = value;
        }

        private void prog_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // debug
        }
    }
}
