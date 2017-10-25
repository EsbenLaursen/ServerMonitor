using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public static class StaticStrings
    {
        public static String ResponseTimeInterval { get { return "ResponseTimeInterval"; } }
        public static String UsersOnlineInterval { get { return "UsersOnlineInterval"; } }
        public static String ServerDetailInterval { get { return "ServerDetailInterval"; } }
        

        //Event types
        public static String ResponseTimeHigh { get { return "Response time high"; } }
        public static String CpuUsageHigh { get { return "Cpu usage high"; } }
        public static String LowMemory { get { return "Low memory"; } }
        public static String HighNumberOfUsers { get { return "High number of users"; } }
        public static String ServerDown { get { return "Server down"; } }
        public static String ServerUp { get { return "Server up"; } }




    }
}
