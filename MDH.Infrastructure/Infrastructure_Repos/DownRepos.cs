using MDH.Application.Appliaction_Services;
using MDH.Domain.Domain_Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MDH.Infrastructure.Infrastructure_Repos
{
    public class DownRepos : IDownloadService
    {
        private readonly string _tempFolder;
        private readonly string _ytDlpPath;
      
       


        public DownRepos()
        {
            _tempFolder = Path.Combine(Path.GetTempPath(), "YTDownloads");
            _ytDlpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tools", "yt-dlp.exe");

            if (!Directory.Exists(_tempFolder))
                Directory.CreateDirectory(_tempFolder);

            if (!File.Exists(_ytDlpPath))
                throw new FileNotFoundException("yt-dlp executable not found.", _ytDlpPath);
        }

        public async Task<string> DownloadVideoAsync(string url, bool audioOnly = false)
        {
            string outputFile = Path.Combine(_tempFolder, "%(title)s.%(ext)s");

            string format = audioOnly ? "bestaudio" : "bestvideo+bestaudio";

            var psi = new ProcessStartInfo
            {
                FileName = _ytDlpPath,
                Arguments = $"-f {format} -o \"{outputFile}\" \"{url}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("YT-DLP Error: " + error);
            }

            var downloadedFile = Directory.GetFiles(_tempFolder)
                .OrderByDescending(f => File.GetCreationTime(f))
                .FirstOrDefault();

            return downloadedFile ?? string.Empty;
        }
    }
}
