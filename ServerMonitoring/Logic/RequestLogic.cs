using Domain;
using log4net;
using Model;
using ServerMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerMonitoring
{

    public class RequestLogic
    {
        protected static readonly DomainFacade Facade = DomainFacade.Instance;
        const int minutesADay = 1440;
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public RequestDataViewModel GetRequestData()
        {
            RequestDataViewModel rdvm = new RequestDataViewModel();
            try
            {
                int interval = Facade.SettingDomainController.GetValueByName(StaticStrings.ResponseTimeInterval);

                rdvm.Interval = interval;

                //this is for getting the total/total mobil/total web requests made by users, in the last given interval
                RequestSummary requestSummary = Facade.RequestSummaryDomainController.GetLatestFinishedRequestSummary(interval);
                if (requestSummary != null)
                {
                    rdvm.DataLastInterval = true;
                    List<UserSummary> LatestUserSummary = Facade.UserSummaryDomainController.GetUserSummariesByRequestSummary(requestSummary);

                    //total request last interval
                    int totalRequests = LatestUserSummary.Sum(x => x.Request);
                    rdvm.TotalRequests = totalRequests;

                    //total web/mobile request last interval
                    int webRequestsLastInterval = LatestUserSummary.Where(x => x.Device == (int)MyEnums.DeviceTypes.Web).Sum(xx => xx.Request);
                    int mobileRequestsLastInterval = LatestUserSummary.Where(x => x.Device == (int)MyEnums.DeviceTypes.Mobile).Sum(xx => xx.Request);
                    rdvm.MobilRequests = mobileRequestsLastInterval;
                    rdvm.WebRequests = webRequestsLastInterval;






                }
                else
                {
                    rdvm.DataLastInterval = false;
                }
                //For average calculations
                List<UserSummary> UsersSummaries = Facade.UserSummaryDomainController.GetAllUserSummeriesLastMinutes(minutesADay).ToList();
                int webRequests = UsersSummaries.Where(x => x.Device == (int)MyEnums.DeviceTypes.Web).Sum(xx => xx.Request);
                int mobileRequests = UsersSummaries.Where(x => x.Device == (int)MyEnums.DeviceTypes.Mobile).Sum(xx => xx.Request);

                int webUsers = UsersSummaries.Where(x => x.Device == (int)MyEnums.DeviceTypes.Web).ToList().Count == 0 ?
                    1 : UsersSummaries.Where(x => x.Device == (int)MyEnums.DeviceTypes.Web).ToList().Count;
                int mobileUsers = UsersSummaries.Where(x => x.Device == (int)MyEnums.DeviceTypes.Mobile).ToList().Count == 0 ?
                    1 : UsersSummaries.Where(x => x.Device == (int)MyEnums.DeviceTypes.Mobile).ToList().Count;
                int totalUsers = UsersSummaries.ToList().Count == 0 ? 1 : UsersSummaries.ToList().Count;

                rdvm.AverageWebRequests = webRequests / webUsers;
                rdvm.AverageMobilRequests = mobileRequests / mobileUsers;
                rdvm.TotalAverageRequests = (webRequests + mobileRequests) / totalUsers;


                //Active users model
                ActiveUser activeUsers = Facade.ActiveUsersDomainController.GetActiveUsers();
                if (activeUsers == null)
                {
                    activeUsers = new ActiveUser()
                    {
                        MobilUsers = 0,
                        WebUsers = 0
                    };
                }
                rdvm.ActiveUserModel = new ActiveUsersModel();
                rdvm.ActiveUserModel.WebUsers = activeUsers.WebUsers;
                rdvm.ActiveUserModel.MobilUsers = activeUsers.MobilUsers;
            }
            catch (Exception e)
            {
                Log.Error("RequestLogic - GetRequestData: ", e);
            }
            return rdvm;
        }

        private int CalculateAverage(List<Request> requests)
        {
            int average = 0;
            try
            {
                if (requests.Count == 0)
                {
                    return 0;
                }

                foreach (var r in requests)
                {
                    average += r.RequestTimeMiliSeconds;
                }


            }
            catch (Exception e)
            {
                Log.Error("RequestLogic - GetRequestData: ", e);
            }
            return average / requests.Count;
        }


        public RequestDataViewModel GetRequestResponseTimeGraphData()
        {
            RequestDataViewModel rdvm = new RequestDataViewModel();
            try
            {
                // int setupInterval = GetSetupVal();
                int setupInterval = Facade.SettingDomainController.GetValueByName(StaticStrings.ResponseTimeInterval);
                if (setupInterval <= 0) { setupInterval = 5; } //Better check
                int iterations = minutesADay / setupInterval;


                //This is for the current average
                //Delete all requests that is older than interval
                Facade.RequestDomainController.DeleteAllOlderThanInterval(setupInterval);
                //Now get them all, to see the current average
                List<Request> requestsLastInterval = Facade.RequestDomainController.GetAll();
                int averageRightNow = CalculateAverage(requestsLastInterval);
                GraphData currentAverageData = new GraphData();
                currentAverageData.x = DateTime.Now;
                currentAverageData.y = averageRightNow;


                //Last 24 hours request summaries.
                List<RequestSummary> allRequests = Facade.RequestSummaryDomainController.GetOneDayRequestSummaries();
                List<GraphData> graphDataResponseTime = new List<GraphData>();

                graphDataResponseTime.Add(currentAverageData);
                for (int i = 2; i < iterations + 1; i++)
                {
                    var minutes = i * setupInterval; //0, 60, 120

                    List<RequestSummary> requests = allRequests.Where(x => x.DateTimeCreated >= DateTime.Now.AddMinutes(-minutes)
                    //efter 12:13
                    && x.DateTimeCreated < DateTime.Now.AddMinutes(-minutes + setupInterval)).ToList(); //før 13:23
                    //finder vi 13:20


                    GraphData dataResponse = new GraphData();

                    
                    DateTime time = GetStartOfInterval(minutes, setupInterval);

                    //Hvis der ikke findes requests i dette interval, brug "time"
                    //tag gennemsnittet fra forrige ?
                    if (requests == null || requests.Count < 1)
                    {
                        dataResponse.y = 0;
                        dataResponse.x = time;
                    }
                    else
                    {   // Ellers brug requestets tid. 
                        dataResponse.x = requests.LastOrDefault().DateTimeCreated.AddMinutes(setupInterval / 2); //?
                        dataResponse.y = requests.Last().ResponseTime;
                    }
                    graphDataResponseTime.Add(dataResponse);
                }


               
                rdvm.AverageResponseTimeGraphData = graphDataResponseTime;
            }
            catch (Exception e)
            {
                Log.Error("RequestLogic - GetRequestResponseTimeGraphData: ", e);
            }
            return rdvm;
        }

        public DateTime GetStartOfInterval(int minutes, int setupInterval)
        {
            var now = DateTime.Now.AddMinutes(-minutes + setupInterval);
            var mins = now.Minute / setupInterval;
            mins *= setupInterval;
            DateTime time = new DateTime(now.Year, now.Month, now.Day, now.Hour, mins, 0);
            time = time.AddMinutes(setupInterval / 2);
            return time;
        }

        protected ILog Log
        {
            get { return LogManager.GetLogger(GetType()); }
        }
    }
}