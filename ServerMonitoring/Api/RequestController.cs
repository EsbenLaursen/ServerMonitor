using Domain;
using Model;
using ServerMonitoring.DAL.Entities;
using ServerMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServerMonitoring.Api
{
    [Authorize]
    public class RequestController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected static readonly DomainFacade Facade = DomainFacade.Instance;

        [HttpGet]
        [Route("api/graphData")] //For the user graph
        public GraphDataModel GetModel()
        {
            //return DashBoardViewModel instead?
            UserLogic gl = new UserLogic();
            Models.DashBoardViewModel dbvm = gl.GetUsersGraphData();
            GraphDataModel gd = new GraphDataModel()
            {
                MobileUsersData = dbvm.MobileUsersData,
                WebUsersData = dbvm.WebUsersData
            };
            return gd;
        }

        [HttpGet]
        [Route("api/requestData")] //For the dropdown and active users
        public RequestDataViewModel RequestData()
        {
            RequestLogic rl = new RequestLogic();
            RequestDataViewModel rdvm = rl.GetRequestData();
            return rdvm;
        }

        [HttpGet]
        [Route("api/requestGraphData")] //For the responsetime graph
        public RequestDataViewModel RequestsGraphData()
        {
            RequestLogic rl = new RequestLogic();
            RequestDataViewModel rdvm = rl.GetRequestResponseTimeGraphData();
            return rdvm;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("api/activeUsers")]
        public void ActiveUsers(ActiveUsersModel userModel)
        {
            //Kan regne med at den rammer "PostRequest" når ActiveUsers ændres?
            //Ellers skal activeUsers bruges i stedet ? - opdater Usersummary her
            log.Info("(api/activeUsers)  -  Mobil: " + userModel.MobilUsers + " Web users: " + userModel.WebUsers);

            try
            { //Kalder 10.1.10.11/settings/getSettings(a) fra client, den skal gennem flere routere?
                if (userModel == null)
                {
                    return;
                }

                ActiveUser activeUser = Facade.ActiveUsersDomainController.GetActiveUsers(); // first
                if (activeUser == null)
                {
                    ActiveUser activeUsers = new ActiveUser();
                    activeUsers.MobilUsers = userModel.MobilUsers;
                    activeUsers.WebUsers = userModel.WebUsers;
                    Facade.ActiveUsersDomainController.CreateNewActiveUsers(activeUsers);
                }
                else
                {
                    activeUser.MobilUsers = userModel.MobilUsers;
                    activeUser.WebUsers = userModel.WebUsers;
                    activeUser.Created = DateTime.Now;
                    Facade.ActiveUsersDomainController.UpdateActiveUsers(activeUser);
                }
            }
            catch (Exception e)
            {
                log.Error("Error in requestController - ActiveUsers: " + e.Message);
            }

        }


        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage PostRequest(MonitorRequest request)
        {
            if (request == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            int interval = Facade.SettingDomainController.GetValueByName(StaticStrings.ResponseTimeInterval);
            RequestSummary requestSummary = Facade.RequestSummaryDomainController.GetRequestSummaryLastMinutes(interval);

            if (requestSummary == null)
            {
                requestSummary = Facade.RequestSummaryDomainController.CreateRequestSummary(requestSummary, interval);
            }
            //System.Configuration.ConfigurationManager.AppSettings["RequestTimeWatchUrl"];
            if (request.Url.Contains(Model.Properties.Settings.Default["RequestTimeWatchUrl"].ToString()))
            {
                //total requests
                var totalRequests = requestSummary.Requests;
                //Current average time
                var averageTime = requestSummary.ResponseTime;
                //total request time 
                var totalRequestsTime = averageTime * totalRequests;
                totalRequests++;
                //the new average = total time + the requests time split by totalrequest + 1
                var newAverageTime = (totalRequestsTime + (int)request.RequestTimeMiliSeconds) / totalRequests;
                requestSummary.ResponseTime = newAverageTime;

                requestSummary.Requests = totalRequests;

                CheckForHighResponseTime(request, requestSummary);

                Facade.RequestSummaryDomainController.UpdateRequestSummary(requestSummary);
            }


            UserSummary userSummary = Facade.UserSummaryDomainController.GetUserSummeryByUserAndRequestSummary(request.UserId, request.Device, requestSummary.Id);
            if (userSummary != null)
            {
                userSummary.Request++;
                Facade.UserSummaryDomainController.UpdateUserSummary(userSummary);
            }
            else
            {
                UserSummary newUserSummary = new UserSummary();
                newUserSummary.UserId = request.UserId;
                newUserSummary.DateTimeCreated = DateTime.Now;
                newUserSummary.Device = request.Device;
                newUserSummary.RequestSummaryId = requestSummary.Id;
                newUserSummary.Request = 1;
                Facade.UserSummaryDomainController.CreateUserSummary(newUserSummary);
                if (request.Device == (int)MyEnums.DeviceTypes.Mobile) { requestSummary.MobileUsers++; }
                if (request.Device == (int)MyEnums.DeviceTypes.Web) { requestSummary.WebUsers++; }
                Facade.RequestSummaryDomainController.UpdateRequestSummary(requestSummary);
            }

            try
            {
                Request r = new Request();
                r.DateCreated = DateTime.Now;
                r.RequestTimeMiliSeconds = Convert.ToInt32(request.RequestTimeMiliSeconds);
                r.Url = request.Url;
                r.UserId = request.UserId;
                r.Device = request.Device;
                Facade.RequestDomainController.Create(r);
            }
            catch (Exception e)
            {
                log.Error("Error in request api controller. Request Part: " + e);
            }
            return new HttpResponseMessage(HttpStatusCode.OK);

        }


        private void CheckForHighResponseTime(MonitorRequest request, RequestSummary requestSummary)
        {
            EventType type = Facade.EventTypeDomainController.GetTypeByName(StaticStrings.ResponseTimeHigh);
            if (request.RequestTimeMiliSeconds > type.PeakValue)
            {
                //Lav notification
                Notification notification = new Notification();
                notification.Seen = false;
                notification.Created = DateTime.Now;
                notification = Facade.NotificationDomainController.Create(notification);
                //request should have serverId SOMEDAY
                // Facade.ServerDetailDomainController.GetLatestServerDetail(request.ServerId); 

                int serverInterval = Facade.SettingDomainController.GetValueByName(StaticStrings.ServerDetailInterval);
                //If interval = 10min -> check 15min back
                ServerDetail latestServerDetailSummary = Facade.ServerDetailDomainController.GetLatestServerDetailByInterval(serverInterval + (serverInterval / 2));
   
                Event e = new Event();
                e.Created = DateTime.Now;
                e.Value = Convert.ToInt32(request.RequestTimeMiliSeconds);
                e.EventTypeId = type.Id;
                e.NotificationId = notification.Id;
                e.RequestSummaryId = requestSummary == null ? 0 : requestSummary.Id;
                e.ServerDetailId = latestServerDetailSummary == null ? 0 : latestServerDetailSummary.Id;

                Facade.EventDomainController.Create(e);
            }
        }
    }



}
