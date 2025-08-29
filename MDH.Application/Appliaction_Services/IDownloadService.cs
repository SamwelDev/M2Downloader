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
        Task<string> DownloadVideoAsync(string url, bool audioOnly = false);
       
    }
}
