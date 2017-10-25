﻿using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using ServerMonitoring.Models;

namespace ServerMonitoring.Logic
{
    public class ReportLogic
    {
        protected static readonly DomainFacade Facade = DomainFacade.Instance;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void GenerateViewModel(DateTime from, DateTime to, ReportViewModel rvm)
        {
            try
            {
                //tjek om to date er i dag. i så fald plus en dag så alt data fra i dag kommer med(ellers tager den fra 00:00)
                if (to.Date == DateTime.Today) {
                   to = to.AddDays(1);
                }
                    List<Event> events = Facade.EventDomainController.GetEventByRange(from, to);
                List<EventType> types = events.Select(x => x.EventType).ToList();
                List<ServerDetail> serverdetails = Facade.ServerDetailDomainController.GetServerDetailByRange(from, to, 10);//10notus
                List<RequestSummary> requestSummaries = Facade.RequestSummaryDomainController.GetRequestSummaryByRange(from, to, 10);//10notus

                //Users
                List<UserSummary> userSummaries = Facade.UserSummaryDomainController.GetUserSummariesByRange(from, to);
                if (userSummaries != null)
                {
                    int distinctUsers = userSummaries.GroupBy(p => p.UserId).Select(g => g.First()).ToList().Count;
                    rvm.UniqueUsers = distinctUsers;

                    //----  Requeststuff
                    int webRequests = userSummaries.Where(x => x.Device == (int)MyEnums.DeviceTypes.Web).Sum(xx => xx.Request);
                    int mobileRequests = userSummaries.Where(x => x.Device == (int)MyEnums.DeviceTypes.Mobile).Sum(xx => xx.Request);

                    int webUsers = userSummaries.Where(x => x.Device == (int)MyEnums.DeviceTypes.Web).ToList().Count == 0 ?
                        1 : userSummaries.Where(x => x.Device == (int)MyEnums.DeviceTypes.Web).ToList().Count;
                    int mobileUsers = userSummaries.Where(x => x.Device == (int)MyEnums.DeviceTypes.Mobile).ToList().Count == 0 ?
                        1 : userSummaries.Where(x => x.Device == (int)MyEnums.DeviceTypes.Mobile).ToList().Count;
                    int totalUsers = userSummaries.ToList().Count == 0 ? 1 : userSummaries.ToList().Count;

                    rvm.AverageWebRequests = webRequests / webUsers;
                    rvm.AverageMobilRequests = mobileRequests / mobileUsers;
                    rvm.TotalAverageRequests = (webRequests + mobileRequests) / totalUsers;

                    int totalRequests = userSummaries == null ? 0 : userSummaries.Sum(x => x.Request);
                    rvm.TotalRequests = totalRequests;

                    //total web/mobile request last interval
                    int webRequestsLastInterval = userSummaries.Where(x => x.Device == (int)MyEnums.DeviceTypes.Web).Sum(xx => xx.Request);
                    int mobileRequestsLastInterval = userSummaries.Where(x => x.Device == (int)MyEnums.DeviceTypes.Mobile).Sum(xx => xx.Request);
                    rvm.TotalMobileRequests = mobileRequestsLastInterval;
                    rvm.TotalWebRequests = webRequestsLastInterval;
                    //----
                }


                var responseTimeEvents = events.Where(x => x.EventType.Name == StaticStrings.ResponseTimeHigh).ToList();

                double avgResponseTime = responseTimeEvents == null || responseTimeEvents.Count <= 0 ? 0 : responseTimeEvents.Average(c => c.Value);

                var serverDownEvents = events.Where(x => x.EventType.Name == StaticStrings.ServerDown).ToList();
                int timeDown = serverDownEvents.Sum(c => c.Value); //minutes registered

                var serverUpEvents = events.Where(x => x.EventType.Name == StaticStrings.ServerUp).ToList();
                int timeUp = serverUpEvents.Sum(c => c.Value); //minutes registered

                //serverdetail stuff
                double avgUtilization = 0;
                long bytesReceived = 0;
                long bytesSent = 0;
                long peakBytesReceived = 0;
                long peakBytesSent = 0;
                int networkUtilization = 0;
                int peakNetworkUtilization = 0;
                if (serverdetails != null && serverdetails.Count > 0)
                {
                    avgUtilization = serverdetails.Average(x => x.Utilization);
                    bytesReceived = serverdetails.Sum(x => x.BytesReceived);
                    bytesSent = serverdetails.Sum(x => x.BytesSent);
                    networkUtilization = Convert.ToInt32(serverdetails.Average(x => x.NetworkUtilization));
                    peakNetworkUtilization = Convert.ToInt32(serverdetails.Max(x => x.NetworkUtilization));
                    peakBytesReceived = Convert.ToInt32(serverdetails.Max(x => x.BytesReceived));
                    peakBytesSent = Convert.ToInt32(serverdetails.Max(x => x.BytesSent));
                }
                rvm.NetworkViewModel.PeakNetworkUtilization = peakNetworkUtilization;
                rvm.NetworkViewModel.NetworkUtilization = networkUtilization;
                rvm.NetworkViewModel.PeakDownload = peakBytesReceived;
                rvm.NetworkViewModel.PeakUpload = peakBytesSent;
                rvm.NetworkViewModel.Upload = bytesSent;
                rvm.NetworkViewModel.Download = bytesReceived;
                rvm.ServerName = "Test";
                rvm.Types = types;
                rvm.to = to;
                rvm.from = from;
                rvm.Events = events;
                rvm.AverageCpuUtilization = avgUtilization;
                rvm.MinutesDown = MinutesToDateString(timeDown);
                rvm.MinutesUp = MinutesToDateString(timeUp);
                rvm.AverageResponseTime = avgResponseTime;
            }
            catch (Exception e)
            {
                log.Error("GetReport: " + e);
            }


        }
        public static string MinutesToDateString(int minutes)
        {
            long minute = minutes % 60;
            long hour = (minutes / 60) % 24;
            long days = (minutes / 60 / 24) % 24;

            string uptimeString = days + " days " + hour + " hours " + minute + " minutes";

            return uptimeString;
        }
    }
}