using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerMonitoring.Models
{
    public class ActiveUsersModel
    {
        public int WebUsers { get; set; }
        public int MobilUsers { get; set; }
    }
}