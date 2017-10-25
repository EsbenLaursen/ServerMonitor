using Model;
using ServerMonitoring.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerMonitoring.Models
{
    public class ServerDataViewModel
    {
        public List<GraphData> CPUUtilizationGraphData { get; set; }
        public int Threads { get; set; }
        public int Processes { get; set; }
        public int Handles { get; set; }
        public string Uptime { get; set; }
        public int RAMUtilization { get; set; }
        public int RAMAvailable { get; set; }
        public int RAMTotal { get; set; }
        public int ServerId { get; set; }
        public bool CurrentStatus { get; set; }
        public DateTime Created { get; set; }
        public List<ServerModel> Servers { get; set; }

        public NetworkViewModel NetworkViewModel { get; set; }

    }
}