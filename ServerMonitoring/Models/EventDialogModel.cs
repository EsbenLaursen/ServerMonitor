using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerMonitoring.Models
{
    public class EventDialogModel
    {
        public string Uptime { get; set; }
        public Event Event { get; set; }
        public string EventDuration { get; set; }
    }
}