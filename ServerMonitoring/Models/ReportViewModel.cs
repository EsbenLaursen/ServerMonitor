using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerMonitoring.Models
{
    public class ReportViewModel
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public string ServerName { get; set; }

        public List<Event> Events { get; set; }
        public List<EventType> Types { get; set; }
        public double AverageResponseTime { get; set; }
        public double AverageCpuUtilization { get; set; }
        public string MinutesDown { get; set; }
        public string MinutesUp { get; set; }
        public int UniqueUsers { get; set; }
        public NetworkViewModel NetworkViewModel { get; set; }
        //requeststuff
        public int TotalRequests { get; set; }
        public int TotalWebRequests { get; set; }

        public int TotalMobileRequests { get; set; }

        public int AverageWebRequests { get; internal set; }
        public int AverageMobilRequests { get; internal set; }
        public int TotalAverageRequests { get; internal set; }
    }
}