using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServerMonitoring.Models;
using Domain;
using Model;
using ServerMonitoring.Entities;
using System.Reflection;

namespace ServerMonitoring.Logic
{

    public class ServerLogic
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected static readonly DomainFacade Facade = DomainFacade.Instance;
        static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public ServerDataViewModel GetServerData(int serverId)
        {
            ServerDataViewModel sdvm = new ServerDataViewModel();
            try
            {

                sdvm.NetworkViewModel = new NetworkViewModel();
                //Graph data
                sdvm.CPUUtilizationGraphData = GenerateGraphData(serverId);

                //Servers available (for dropdown)
                sdvm.Servers = GetServers(); // USER ID LATER

                sdvm.Created = DateTime.Now;




                //SHOW SUMMARY - USE THIS CODE
                //int serverInterval = Facade.SettingDomainController.GetValueByName(StaticStrings.ServerDetailInterval);
                //ServerDetail latestServerDetails = Facade.ServerDetailDomainController.GetLatestServerDetailByInterval(serverInterval);

                ServerDetailCurrent latestServerDetails = Facade.ServerDetailCurrentDomainController.GetLatest();

                if (latestServerDetails != null)
                {
                    sdvm.CurrentStatus = true;
                    sdvm.Handles = latestServerDetails.Handles;
                    sdvm.Threads = latestServerDetails.Thread;
                    // sdvm.Uptime = CalculateUpTime(latestServerDetails.UpTime); The computer uptime,
                    sdvm.RAMAvailable = Convert.ToInt32(latestServerDetails.RAMAvailable);
                    sdvm.Processes = latestServerDetails.Processes;
                    sdvm.RAMTotal = Convert.ToInt32(latestServerDetails.RAMTotal);
                    sdvm.NetworkViewModel.NetworkUtilization = latestServerDetails.NetworkUtilization;
                }

                //Should refactors
                int interval = Facade.SettingDomainController.GetValueByName(StaticStrings.ServerDetailInterval);

                List<ServerDetail> serverDetail = Facade.ServerDetailDomainController.GetOneDayServerDetail(serverId);
                if (serverDetail != null && serverDetail.Count > 0)
                {
                    sdvm.NetworkViewModel.Download = serverDetail.Sum(x => x.BytesReceived);
                    sdvm.NetworkViewModel.Upload = serverDetail.Sum(x => x.BytesSent);

                    int totalReceived = serverDetail.Max(x => x.BytesReceived);
                    int totalSent = serverDetail.Max(x => x.BytesSent);
                    string totalReceivedFormatted = SizeSuffix(totalReceived, 1) + "/S";
                    string totalSentFormatted = SizeSuffix(totalSent, 1) + "/S";

                    sdvm.NetworkViewModel.PeakDownloadString = totalReceivedFormatted;
                    sdvm.NetworkViewModel.PeakUploadString = totalSentFormatted;
                    sdvm.NetworkViewModel.NetworkUtilization = Convert.ToInt32(serverDetail.Average(x => x.NetworkUtilization));
                }


                bool dataLastMinut = Facade.ServerDetailCurrentDomainController.HasAchievedDataLastFifteenSeconds();
                //get last up/down event and assign ServerStatus to this.
                var lastDown = Facade.EventDomainController.GetLatestServerDownEvent();
                var lastUp = Facade.EventDomainController.GetLatestServerUpEvent();
                bool ServerStatus = dataLastMinut;
                if (lastUp != null && lastDown != null)
                {
                    if (lastDown.Created > lastUp.Created)
                    {
                        ServerStatus = false;
                    }
                    else
                    {
                        ServerStatus = true;
                    }
                }


                if (!dataLastMinut) //Server considered down
                {
                    if (ServerStatus) // If serverstatus was up, then it just went down and should create event
                    {
                        ServerStatus = false;
                        CreateServerDownEvent(sdvm); //Create event, and finish last server up event
                        Model.Event lastUpEvent = Facade.EventDomainController.GetLatestServerUpEvent();
                        if (lastUpEvent != null)
                        {
                            TimeSpan ts = DateTime.Now.Subtract(lastUpEvent.Created);
                            lastUpEvent.Value = Convert.ToInt32(ts.TotalMinutes);
                            Facade.EventDomainController.UpdateEvent(lastUpEvent);
                        }
                        else
                        {
                            CreateServerUpEvent(sdvm);
                        }

                    }
                    else  //Server down and last serverstatus was false, we just update the last downevents value, and the "downtime"
                    {
                        Model.Event lastDownEvent = Facade.EventDomainController.GetLatestServerDownEvent();
                        if (lastDownEvent != null)
                        {
                            UpdateValueAndUptime(sdvm, lastDownEvent);

                        }
                        else
                        {
                            CreateServerDownEvent(sdvm);
                        }
                    }
                }
                else //Server is up
                {

                    if (!ServerStatus) // If serverstatus was down, then it just went up and should create event
                    {
                        ServerStatus = true;
                        CreateServerUpEvent(sdvm); //Create event, and finish last server up event
                        Model.Event lastDownEvent = Facade.EventDomainController.GetLatestServerDownEvent();
                        if (lastDownEvent != null)
                        {
                            TimeSpan ts = DateTime.Now.Subtract(lastDownEvent.Created);
                            lastDownEvent.Value = Convert.ToInt32(ts.TotalMinutes);
                            Facade.EventDomainController.UpdateEvent(lastDownEvent);
                        }
                        else
                        {
                            CreateServerDownEvent(sdvm);
                        }

                    }
                    else
                    {  //just update the value
                        Model.Event lastUpEvent = Facade.EventDomainController.GetLatestServerUpEvent();
                        if (lastUpEvent != null)
                        {
                            UpdateValueAndUptime(sdvm, lastUpEvent);
                        }
                        else
                        {
                            CreateServerUpEvent(sdvm);
                        }
                    }
                }
                sdvm.CurrentStatus = ServerStatus;
            }
            catch (Exception e)
            {
                log.Error("ServerLogic - GetServerData: " + e);
            }


            return sdvm;

        }

        private void UpdateValueAndUptime(ServerDataViewModel sdvm, Model.Event lastUpOrDownEvent)
        {
            try
            {
                if (lastUpOrDownEvent != null)
                {
                    TimeSpan ts = DateTime.Now.Subtract(lastUpOrDownEvent.Created);//the time between the event was created and now.
                    decimal time = Convert.ToDecimal(ts.TotalSeconds);//convert to totalminutes
                    lastUpOrDownEvent.Value = Convert.ToInt32(ts.TotalMinutes);//update current value
                    sdvm.Uptime = CalculateUpTime(time);//update uptime
                    Facade.EventDomainController.UpdateEvent(lastUpOrDownEvent);
                }
            }
            catch (Exception e)
            {
                log.Error("ServerLogic - UpdateValueAndUptime: " + e);
            }
        }

        private static List<ServerModel> GetServers()
        {
            List<ServerModel> servers = new List<ServerModel>();
            try
            {
                foreach (var server in Facade.ServerDomainController.GetAllServers())
                {
                    servers.Add(new ServerModel() { Name = server.Name, Id = server.Id });
                }
            }
            catch (Exception e)
            {
                log.Error("ServerLogic - GetServers: " + e);
            }

            return servers;
        }

        private List<GraphData> GenerateGraphData(int serverId)
        {

            List<GraphData> graphDataCPUUtilization = new List<GraphData>();
            try
            {

                //Data past 24 hours from serverdetails
                List<ServerDetail> allServerDetails = Facade.ServerDetailDomainController.GetOneDayServerDetail(serverId).OrderByDescending(x => x.Created).ToList();
                //This is for the current average
                //Delete all requests that is older than interval 
                              //int setupInterval = Facade.SettingDomainController.GetValueByName(StaticStrings.ServerDetailInterval);
                              //Facade.ServerDetailCurrentDomainController.DeleteAllOlderThanInterval(setupInterval);
                //Now get them all, to see the current average
                List<ServerDetailCurrent> serverDetailCurrent = Facade.ServerDetailCurrentDomainController.GetAll();
                int averageRightNow = CalculateAverage(serverDetailCurrent);
                GraphData currentAverageData = new GraphData();
                currentAverageData.x = DateTime.Now;
                currentAverageData.y = averageRightNow;


                if (averageRightNow != 0) //if no data (server down), dont plot 0% cpu in 
                {
                    graphDataCPUUtilization.Add(currentAverageData);
                }
                foreach (var serverDetail in allServerDetails)
                {
                    GraphData serverData = new GraphData();
                    serverData.x = serverDetail.Created;
                    serverData.y = serverDetail.Utilization;
                    graphDataCPUUtilization.Add(serverData);
                }
            }
            catch (Exception e)
            {
                log.Error("ServerLogic - GenerateGraphData: " + e);
            }
            return graphDataCPUUtilization;
        }

        private void CreateServerUpEvent(ServerDataViewModel sdvm)
        {
            try
            {
                Model.EventType type = Facade.EventTypeDomainController.GetTypeByName(StaticStrings.ServerUp);
                int serverInterval = Facade.SettingDomainController.GetValueByName(StaticStrings.ServerDetailInterval);
                RequestSummary requestSummary = Facade.RequestSummaryDomainController.GetRequestSummaryLastMinutes(serverInterval);
                ServerDetail latestServerDetailSummary = Facade.ServerDetailDomainController.GetLatestServerDetailByInterval(serverInterval + (serverInterval / 2));

                Notification n = new Notification();
                n.Seen = false;
                n = Facade.NotificationDomainController.Create(n);

                Model.Event e = new Model.Event();
                e.Created = DateTime.Now;
                e.Value = 1;
                e.EventTypeId = type.Id;
                e.RequestSummaryId = requestSummary == null ? 0 : requestSummary.Id;
                e.ServerDetailId = latestServerDetailSummary == null ? 0 : latestServerDetailSummary.Id;
                e.NotificationId = n.Id;
                Facade.EventDomainController.Create(e);
                sdvm.CurrentStatus = true;
            }
            catch (Exception e)
            {
                log.Error("ServerLogic - CreateServerUpEvent: " + e);
            }
        }

        private void CreateServerDownEvent(ServerDataViewModel sdvm)
        {
            try
            {
                Model.EventType type = Facade.EventTypeDomainController.GetTypeByName(StaticStrings.ServerDown);
                int serverInterval = Facade.SettingDomainController.GetValueByName(StaticStrings.ServerDetailInterval);
                RequestSummary requestSummary = Facade.RequestSummaryDomainController.GetRequestSummaryLastMinutes(serverInterval);
                ServerDetail latestServerDetailSummary = Facade.ServerDetailDomainController.GetLatestServerDetailByInterval(serverInterval + (serverInterval / 2));

                Notification n = new Notification();
                n.Seen = false;
                n = Facade.NotificationDomainController.Create(n);

                Model.Event e = new Model.Event();
                e.Created = DateTime.Now;
                e.Value = 1;
                e.EventTypeId = type.Id;
                e.RequestSummaryId = requestSummary == null ? 0 : requestSummary.Id;
                e.ServerDetailId = latestServerDetailSummary == null ? 0 : latestServerDetailSummary.Id;
                e.NotificationId = n.Id;
                Facade.EventDomainController.Create(e);
                sdvm.Uptime = CalculateUpTime(0);
                sdvm.CurrentStatus = false;
            }
            catch (Exception e)
            {
                log.Error("ServerLogic - CreateServerDownEvent: " + e);
            }

        }

        private int CalculateAverage(List<ServerDetailCurrent> serverDetailCurrent)
        {

            int average = 0;
            try
            {
                if (serverDetailCurrent.Count == 0)
                {
                    return 0;
                }

                foreach (var r in serverDetailCurrent)
                {
                    average += r.Utilization;
                }


            }
            catch (Exception e)
            {
                log.Error("ServerLogic - CalculateAverage: ", e);
            }
            return average / serverDetailCurrent.Count;

        }

        public string CalculateUpTime(decimal sec)
        {
            string uptimeString = "";
            try
            {
                long seconds = Convert.ToInt64(sec);
                long minute = (seconds / 60) % 60;
                long hour = (seconds / 60 / 60) % 24;
                long days = (seconds / 60 / 60 / 24);

                uptimeString = days + " days " + hour + " hours " + minute + " minutes";
            }
            catch (Exception e)
            {
                log.Error("ServerLogic - CalculateUpTime: ", e);
            }

            return uptimeString;
        }

        static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
    }
}