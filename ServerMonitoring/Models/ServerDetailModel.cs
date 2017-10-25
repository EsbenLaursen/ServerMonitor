using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerMonitoring.Entities
{
    public class ServerDetailModel
    {
        public DateTime Created { get; set; }
        public int Utilization { get; set; }
        public int Thread { get; set; }
        public int Processes { get; set; }
        public int Handles { get; set; }
        public int Uptime { get; set; }
        public int RAMUtilization { get; set; }
        public int RAMAvailable { get; set; }
        public int RAMTotal { get; set; }
        public int NetworkSpeed { get; set; }
        public string ServerName { get; set; }
        public int ServerId { get; set; }
        public int NetworkUtilization { get; set; }
        public int BytesSent { get; set; }
        public int BytesReceived { get; set; }
    }
}