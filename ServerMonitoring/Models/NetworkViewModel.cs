using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerMonitoring.Models
{
    public class NetworkViewModel
    {
        public long Upload { get; set; }
        public long Download { get; set; }
        public long PeakUpload { get; set; }
        public long PeakDownload { get; set; }
        public int NetworkUtilization { get; set; }
        public int PeakNetworkUtilization { get; set; }

        public string PeakUploadString { get; set; }
        public string PeakDownloadString { get; set; }
    }
}