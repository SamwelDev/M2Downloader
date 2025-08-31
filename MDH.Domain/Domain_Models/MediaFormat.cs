using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDH.Domain.Domain_Models
{
    public class MediaFormat
    {
        public string FormatId { get; set; } = "";
        public string Extension { get; set; } = "";
        public string Resolution { get; set; } = ""; 
        public string Note { get; set; } = ""; 
        public long? FileSize { get; set; } 

        public string FileSizeReadable => FileSize.HasValue 
            ? $"{(FileSize.Value / 1024.0 / 1024.0):0.##} MB" 
            : "Unknown";
        }
}
