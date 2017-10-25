using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerMonitoring.Entities
{
    public class Event
    {
        public DateTime Created { get; set; }
        public int EventTypeId { get; set; }
        public int Value { get; set; }
        public int RequestSummaryId { get; set; }
    }
}