using MDH.Domain.Domain_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDH.Application.Appliaction_Services
{
    public  interface IDownloadService
    {
        Task<string> DownloadVideoAsync(
           string url,
           bool audioOnly = false,
           string? formatId = null,
           Action<int, string>? progressCallback = null);
        Task<List<MediaFormat>> GetAvailableFormatsAsync(string url);
        Task<string?> GetThumbnailAsync(string url);

    }
}
