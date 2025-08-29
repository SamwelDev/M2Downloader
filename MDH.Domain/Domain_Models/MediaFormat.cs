using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDH.Domain.Domain_Models
{
    public class MediaFormat
    {
        public string FormatCode { get; set; } = string.Empty; 
        public string Extension { get; set; } = string.Empty;
        public string Quality { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}
