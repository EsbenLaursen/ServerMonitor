using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerMonitoring.Entities
{
    public class EventType
    {
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public int PeakValue { get; set; }
        public bool Notify { get; set; }
        public int Risk { get; set; }
    }
}