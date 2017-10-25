using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerMonitoring.Models
{
    public class RequestDataViewModel
    {
        public int TotalRequests { get; set; }
        public int TotalAverageRequests { get; set; }
        public int MobilRequests { get; set; }
        public int AverageMobilRequests { get; set; }
        public int WebRequests { get; set; }
        public int AverageWebRequests { get; set; }
        public int Interval { get; set; }
        public decimal AverageResponsTime { get; set; }
        public bool DataLastInterval { get; set; }
        public ActiveUsersModel ActiveUserModel { get; set; }

        public List<GraphData> AverageResponseTimeGraphData { get; set; }

    }
}