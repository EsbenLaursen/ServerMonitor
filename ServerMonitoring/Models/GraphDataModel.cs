using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerMonitoring.Models
{
    public class GraphDataModel
    {
        public List<GraphData> WebUsersData { get; set; }

        public List<GraphData> MobileUsersData { get; set; }


    }
}