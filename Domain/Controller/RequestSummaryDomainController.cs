using log4net;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Controller
{
    
    public class RequestSummaryDomainController : CrudDomainController<RequestSummary>
    {
        
        public List<RequestSummary> GetRequestSummaries()
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var reqSummaries = ctx.RequestSummaries.OrderBy(x => x.DateTimeCreated).ToList();
                    return reqSummaries;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetRequestSummaries:", e);
                throw;
            }
        }

        public RequestSummary GetLatestFinishedRequestSummary(int interval)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    // hvis klokken = 11:32, er 11:30 summary igang, så vil vi have 25 min summary
                    // efter 11:22 og før 11:27
                    var reqSummary = ctx.RequestSummaries.Where(x => x.DateTimeCreated > DateTime.Now.AddMinutes(-interval*2)
                    && x.DateTimeCreated < DateTime.Now.AddMinutes(-interval)).FirstOrDefault();
                    
                    return reqSummary;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetRequestSummaries:", e);
                throw;
            }
        }

        public List<RequestSummary> GetRequestSummaryByRange(DateTime from, DateTime to, int serverId) //ServerId
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var reqSummaries = ctx.RequestSummaries.Where(x => x.DateTimeCreated > from
                    && x.DateTimeCreated < to).ToList();
                    return reqSummaries;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetRequestSummaryByRange:", e);
                throw;
            }
        }

        public RequestSummary GetFirst()
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var e = ctx.RequestSummaries.OrderByDescending(x=> x.DateTimeCreated).FirstOrDefault();
                    return e;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetRequestSummaries:", e);
                throw;
            }
        }

        public List<RequestSummary> GetOneDayRequestSummaries()
        {

            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var reqSummaries = ctx.RequestSummaries.Where(x=> x.DateTimeCreated > DateTime.Now.AddDays(-1)).ToList();
                    return reqSummaries;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetRequestSummaries:", e);
                throw;
            }
        }

        public RequestSummary GetRequestSummaryLastMinutes(int interval)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var now = DateTime.Now; 
                    var minutes = now.Minute / interval; 
                    minutes *= interval;
                    var date = new DateTime(now.Year, now.Month, now.Day, now.Hour, minutes, 0);
                    var currentRequestSummary = ctx.RequestSummaries.FirstOrDefault(x => x.DateTimeCreated >= date );
                    return currentRequestSummary;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetCurrentRequestSummary:", e);
                throw;
            }
        }

        public RequestSummary CreateRequestSummary(RequestSummary requestSummary, int interval)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    requestSummary = new RequestSummary();
                    var now = DateTime.Now;
                    var minutes = now.Minute / interval;  
                    minutes *= interval; 
                    requestSummary.DateTimeCreated = new DateTime(now.Year, now.Month, now.Day, now.Hour, minutes, 0); 

                    requestSummary.MobileUsers = 0;
                    requestSummary.WebUsers = 0;

                    ctx.RequestSummaries.InsertOnSubmit(requestSummary);
                    ctx.SubmitChanges();
                    return requestSummary;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - CreateRequestSummary:", e);
                throw;
            }
        }

       


        public List<RequestSummary> GetRequestSummariesLastHour()
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {

                    var result = ctx.RequestSummaries.Where(x => x.DateTimeCreated > DateTime.Now.AddHours(-1) && x.DateTimeCreated < DateTime.Now).ToList();
                    return result;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetRequestSummariesLastHour:", e);
                throw;
            }
        }
   
        public void UpdateRequestSummary(RequestSummary requestSummary)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    RequestSummary r = ctx.RequestSummaries.FirstOrDefault(x => x.Id == requestSummary.Id);
                    if (r != null)
                    {
                        foreach (PropertyInfo prop in typeof(RequestSummary).GetProperties())
                        {
                            if (UpdateAllowed(prop))
                            {
                                prop.SetValue(r, prop.GetValue(requestSummary, null));
                            }

                        }
                        ctx.SubmitChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - UpdateRequestSummary: " + e);
                throw;
            }
        }

        public List<RequestSummary> GetRequestSummariesInterval(int minutes, int setupInterval)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    return ctx.RequestSummaries.Where(x => x.DateTimeCreated >= DateTime.Now.AddMinutes(-minutes)
                && x.DateTimeCreated < DateTime.Now.AddMinutes(-minutes + setupInterval)).ToList();
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetRequestSummariesInterval: " + e);
                throw;
            }
           
        }
    }
}
