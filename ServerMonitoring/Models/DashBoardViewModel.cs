using Model;
using ServerMonitoring.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerMonitoring.Models
{
    public class DashBoardViewModel
    {
        public List<GraphData> WebUsersData { get; set; }

        public List<GraphData> MobileUsersData { get; set; }
        public SettingsModel SettingsModel { get; set; }
        public RequestDataViewModel RequestDataViewModel { get; set; }
        public ServerDataViewModel ServerDataViewModel { get; set; }
        public EventViewModel EventViewModel { get; set; }
    }
    public class GraphData
    {
        public DateTime x { get; set; }
        public int y { get; set; }
    }

}