using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDH.Domain.Domain_Models
{
    public  class MediaDownload
    {
        public int Id { get; set; } // optional
        public string SourceUrl { get; set; } = string.Empty;
        public List<MediaFormat> Formats { get; set; } = new();
    }
}

