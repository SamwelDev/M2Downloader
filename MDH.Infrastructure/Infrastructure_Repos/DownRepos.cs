using MDH.Application.Appliaction_Services;
using MDH.Domain.Domain_Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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

        public async Task<List<MediaFormat>> GetAvailableFormatsAsync(string url)
        {
            var formats = new List<MediaFormat>();
            var psi = new ProcessStartInfo
            {
                FileName = _ytDlpPath,
                Arguments = $"--list-formats \"{url}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi)!;
            string output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            var lines = output.Split("\n");
            foreach (var line in lines)
            {
                var parts = Regex.Split(line.Trim(), @"\s+");
                if (parts.Length >= 3 && int.TryParse(parts[0], out _))
                {
                    formats.Add(new MediaFormat
                    {
                        FormatId = parts[0],
                        Extension = parts[1],
                        Resolution = parts[2],
                        Note = string.Join(' ', parts.Skip(3))
                    });
                }
            }

            return formats;
        }
        public async Task<string?> GetThumbnailAsync(string url)
{
    var psi = new ProcessStartInfo
    {
        FileName = _ytDlpPath,
        Arguments = $"--get-thumbnail \"{url}\"",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using var process = Process.Start(psi)!;
    string output = await process.StandardOutput.ReadToEndAsync();
    await process.WaitForExitAsync();

    var thumbnail = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
    return string.IsNullOrWhiteSpace(thumbnail) ? null : thumbnail;
}

        public async Task<string> DownloadVideoAsync(
            string url,
            bool audioOnly = false,
            string? formatId = null,
            Action<int, string>? progressCallback = null)
        {
            string outputFile = Path.Combine(_tempFolder, "%(title)s.%(ext)s");
            string format = formatId ?? (audioOnly ? "bestaudio[ext=m4a]/bestaudio" : "bestvideo[ext=mp4]+bestaudio[ext=m4a]/best");

            var psi = new ProcessStartInfo
            {
                FileName = _ytDlpPath,
                Arguments = $"-f {format} --merge-output-format mp4 --no-check-certificate --rm-cache-dir -o \"{outputFile}\" \"{url}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

            process.OutputDataReceived += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(e.Data)) return;
                if (e.Data.Contains("[download]"))
                {
                    var match = Regex.Match(e.Data, @"(\d{1,3}\.\d)%");
                    if (match.Success)
                    {
                        int percent = (int)Math.Round(double.Parse(match.Groups[1].Value));
                        progressCallback?.Invoke(percent, e.Data);
                    }
                }
            };

            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Console.WriteLine("YT-DLP Error: " + e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            return Directory.GetFiles(_tempFolder)
                            .OrderByDescending(f => File.GetCreationTime(f))
                            .FirstOrDefault() ?? string.Empty;
        }
    }







}


