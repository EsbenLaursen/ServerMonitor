using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerMonitoring.Models
{
    public class EventViewModel
    { 
        public List<EventType> EventTypes { get; set; }
        public List<Event> Events { get; set; }
        public int Interval { get; set; }
        public int NotificationCount { get; set; }
    }
}