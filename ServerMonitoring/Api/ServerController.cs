using Domain;
using Model;
using ServerMonitoring.Entities;
using ServerMonitoring.Logic;
using ServerMonitoring.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServerMonitoring.Api
{
    public class ServerController : ApiController
    {
        protected static readonly DomainFacade Facade = DomainFacade.Instance;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Authorize]
        [HttpGet]
        [Route("api/serverDetailsData/{serverId}")]
        public ServerDataViewModel ServerData(int serverId)
        {
            ServerLogic sl = new ServerLogic();
            ServerDataViewModel sdvm = sl.GetServerData(serverId);
            return sdvm;
        }

        [Authorize]
        [HttpPost]
        [Route("api/serverDetailsCurrent")]
        public HttpResponseMessage PostServerDetailCurrent(ServerDetailModel serverDetails)
        {

            try
            {
                if (serverDetails == null || serverDetails.ServerName == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
                //Server regonized?
                Server s = Facade.ServerDomainController.GetServerByName(serverDetails.ServerName);
                if (s == null)
                {
                    s = new Server();
                    s.Name = serverDetails.ServerName;
                    s = Facade.ServerDomainController.CreateServer(s);
                }
                //Create everytime!
                ServerDetailCurrent currentServerDetail = new ServerDetailCurrent();
                currentServerDetail.Handles = serverDetails.Handles;
                currentServerDetail.Utilization = serverDetails.Utilization;
                currentServerDetail.Thread = serverDetails.Thread;
                currentServerDetail.Processes = serverDetails.Processes;
                currentServerDetail.UpTime = serverDetails.Uptime;
                currentServerDetail.RAMUtilization = serverDetails.RAMUtilization;
                currentServerDetail.RAMAvailable = serverDetails.RAMAvailable;
                currentServerDetail.RAMTotal = serverDetails.RAMTotal;
                currentServerDetail.BytesReceived = serverDetails.BytesReceived;
                currentServerDetail.BytesSent = serverDetails.BytesSent;
                currentServerDetail.NetworkUtilization = serverDetails.NetworkUtilization;
                currentServerDetail.ServerId = s.Id;
                Facade.ServerDetailCurrentDomainController.CreateServerDetailCurrent(currentServerDetail);
                //Styrer den sidste del af grafen (Bliver slettet løbende)



                //ServerSummary hvert interval!

                //der gemmes et summary hvert interval (10 MIN). DVS 13:35, består af data fra 13:30-13:40

                //ved 13:30, kan vi gemme (summary 13:45)
                //ved 13:40, kan vi gemme (summary 13:35) OSV

                //KL 13:50:01, tjekker vi 15minutter tilbage (1,5 interval)

                //Get the interval
                int serverInterval = Facade.SettingDomainController.GetValueByName(StaticStrings.ServerDetailInterval);
                //If interval = 10min -> check 15min back
                ServerDetail latestServerDetailSummary = Facade.ServerDetailDomainController.GetLatestServerDetailByInterval(serverInterval + (serverInterval / 2));
                //If null, create new
                if (latestServerDetailSummary == null)
                {
                    //get all servercurrent
                    List<ServerDetailCurrent> currentServerDetailsList = Facade.ServerDetailCurrentDomainController.GetAll();
                    //create new serverdetailSummary with the averages from all servercurrent
                    ServerDetail serverDetailSummary = new ServerDetail();
                    serverDetailSummary.Handles = Convert.ToInt32(currentServerDetailsList.Average(x => x.Handles));
                    serverDetailSummary.RAMAvailable = Convert.ToInt32(currentServerDetailsList.Average(x => x.RAMAvailable));
                    serverDetailSummary.RAMTotal = Convert.ToInt32(currentServerDetailsList.Average(x => x.RAMTotal));
                    serverDetailSummary.Thread = Convert.ToInt32(currentServerDetailsList.Average(x => x.Thread));
                    serverDetailSummary.Processes = Convert.ToInt32(currentServerDetailsList.Average(x => x.Processes));
                    serverDetailSummary.Utilization = Convert.ToInt32(currentServerDetailsList.Average(x => x.Utilization));
                    serverDetailSummary.ServerId = currentServerDetailsList.FirstOrDefault().ServerId;
                    serverDetailSummary.UpTime = currentServerDetailsList.LastOrDefault().UpTime;
                    serverDetailSummary.Created = GetStartOfInterval(DateTime.Now, serverInterval);
                    serverDetailSummary.BytesSent = currentServerDetailsList.Sum(x => x.BytesSent);
                    serverDetailSummary.BytesReceived = currentServerDetailsList.Sum(x => x.BytesReceived);
                    serverDetailSummary.NetworkUtilization = Convert.ToInt32(currentServerDetailsList.Average(x => x.NetworkUtilization));

                    latestServerDetailSummary = Facade.ServerDetailDomainController.Create(serverDetailSummary);
                }

                //Events - Could add more


                RequestSummary requestSummary = Facade.RequestSummaryDomainController.GetRequestSummaryLastMinutes(serverInterval);

                EventLogic ev = new EventLogic(); //Move most logic to this. Maybe make generic method for creating events?
                Model.EventType cpuEventType = Facade.EventTypeDomainController.GetTypeByName(StaticStrings.CpuUsageHigh);
                if (currentServerDetail.Utilization > cpuEventType.PeakValue)
                {
                    //Notification
                    Notification n = Facade.NotificationDomainController.Create(new Notification());

                    //Event
                    Model.Event e = new Model.Event();
                    //ServerDetail -> tage nuværende snapshot, eller tage summary eller begge?
                    e.ServerDetailId = latestServerDetailSummary.Id; //Server summary
                                                                     // requestSummary i dette tidsrum
                    e.RequestSummaryId = requestSummary == null ? 0 : requestSummary.Id;
                    e.Created = DateTime.Now;
                    e.EventTypeId = cpuEventType.Id;
                    e.NotificationId = n.Id;
                    e.Value = serverDetails.Utilization;
                    Model.Event createdEvent = Facade.EventDomainController.Create(e);

                    //Notify by email?
                    if (cpuEventType.Notify)
                    {
                        EmailManager emailManager = new EmailManager();
                    }

                }
                Model.EventType ramEventType = Facade.EventTypeDomainController.GetTypeByName(StaticStrings.LowMemory);
                if (currentServerDetail.RAMAvailable < ramEventType.PeakValue)
                {
                    //Notification
                    Notification n = Facade.NotificationDomainController.Create(new Notification());

                    //Event
                    Model.Event e = new Model.Event();
                    e.ServerDetailId = latestServerDetailSummary.Id;
                    e.RequestSummaryId = requestSummary == null ? 0 : requestSummary.Id;
                    e.Created = DateTime.Now;
                    e.EventTypeId = ramEventType.Id;
                    e.NotificationId = n.Id;
                    e.Value = serverDetails.RAMAvailable;
                    Model.Event createdEvent = Facade.EventDomainController.Create(e);

                    //Notify by email?
                    if (ramEventType.Notify)
                    {
                        EmailManager emailManager = new EmailManager();
                    }
                }
            }
            catch (Exception e)
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.Content = new StringContent("Error: " + e);
                return response;
            }


            //Slet Dem her?
            int setupInterval = Facade.SettingDomainController.GetValueByName(StaticStrings.ServerDetailInterval);
            Facade.ServerDetailCurrentDomainController.DeleteAllOlderThanInterval(setupInterval);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        
        public DateTime GetStartOfInterval(DateTime now, int interval)
        {
            var mins = now.Minute / interval;
            mins *= interval;
            DateTime time = new DateTime(now.Year, now.Month, now.Day, now.Hour, mins, 0);
            int inTheMiddleOfInterval = (interval / 2);
            time = time.AddMinutes(-inTheMiddleOfInterval);
            return time;
        }
    }
}