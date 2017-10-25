using Domain;
using Model;
using ServerMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ServerMonitoring
{
    public class UserLogic
    {
        protected static readonly DomainFacade Facade = DomainFacade.Instance;
        const int minutesADay = 1440;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DateTime GetStartOfInterval(int minutes, int setupInterval)
        {
            var now = DateTime.Now.AddMinutes(-minutes + setupInterval);
            var hours = now.Hour;
            var day = now.Day;
            var mins = now.Minute / setupInterval;
            mins *= setupInterval;
            DateTime time = new DateTime(now.Year, now.Month, day, hours, mins, 0);
            time = time.AddMinutes(setupInterval / 2);
            return time;
        }

        public DashBoardViewModel GetUsersGraphData()
        {
            DashBoardViewModel dbvm = new DashBoardViewModel();

            try
            {
                int interval = Facade.SettingDomainController.GetValueByName(StaticStrings.UsersOnlineInterval);
                if (interval <= 0) { interval = 5; } //Better check
                int iterations = minutesADay / interval;

                List<RequestSummary> allRequests = Facade.RequestSummaryDomainController.GetOneDayRequestSummaries();

                List<GraphData> graphDataMobile = new List<GraphData>();
                List<GraphData> graphDataWeb = new List<GraphData>();


                GraphData currentWebUser = new GraphData();
                GraphData currentMobileUser = new GraphData();
                currentMobileUser.x = DateTime.Now;
                currentWebUser.x = DateTime.Now;
                //Get current active users and check if older than interval
                ActiveUser a = Facade.ActiveUsersDomainController.GetActiveUsers();
                if (a != null)
                {
                    currentWebUser.y = a.WebUsers;
                    currentMobileUser.y = a.MobilUsers;
                }
                graphDataMobile.Add(currentMobileUser);
                graphDataWeb.Add(currentWebUser);



                //Getting graph data for all the finished intervals
                for (int i = 2; i < iterations + 1; i++)
                {
                    var minutes = i * interval;

                    List<RequestSummary> requests = allRequests.Where(x => x.DateTimeCreated >= DateTime.Now.AddMinutes(-minutes)
                    && x.DateTimeCreated < DateTime.Now.AddMinutes(-minutes + interval)).ToList();

                    GraphData dataMobile = new GraphData();
                    GraphData dataWeb = new GraphData();

                    // Find starten af interval (9:33 -> 9:30)
                    DateTime time = GetStartOfInterval(minutes, interval);

                    //Hvis der ikke findes requests i dette interval
                    if (requests == null || requests.Count < 1)
                    {
                        dataMobile.y = 0;
                        dataWeb.y = 0;
                        dataMobile.x = time;
                        dataWeb.x = time;
                    }
                    else
                    {   // Ellers brug requestets tid. 
                        dataMobile.x = requests.LastOrDefault().DateTimeCreated.AddMinutes(interval / 2); //?;
                        dataWeb.x = requests.LastOrDefault().DateTimeCreated.AddMinutes(interval / 2); //?;

                        dataMobile.y = requests.Last().MobileUsers;
                        dataWeb.y = requests.Last().WebUsers;
                    }
                    graphDataMobile.Add(dataMobile);
                    graphDataWeb.Add(dataWeb);
                }



                dbvm.MobileUsersData = graphDataMobile;
                dbvm.WebUsersData = graphDataWeb;
                
            }
            catch (Exception e)
            {
                log.Error("GetUsersGraphData: " + e);
            }
            return dbvm;
        }
    }
}