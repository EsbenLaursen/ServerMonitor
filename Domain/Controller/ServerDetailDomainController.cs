using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Controller
{
    public class ServerDetailDomainController : CrudDomainController<ServerDetail>
    {
        public ServerDetail CreateServerSummary(ServerDetail serverDetails)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    serverDetails.Created = DateTime.Now;
                    ctx.ServerDetails.InsertOnSubmit(serverDetails);
                    ctx.SubmitChanges();
                    return serverDetails;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - CreateServerSummary:", e);
                throw;
            }
        }

        public ServerDetail GetLatestServerDetail(int serverId)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var serverDetails = ctx.ServerDetails.OrderByDescending(x => x.Created).FirstOrDefault(xx => xx.ServerId == serverId);
                    return serverDetails;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetLatestServerDetail:", e);
                throw;
            }
        }

        public List<ServerDetail> GetServerDetailByRange(DateTime from, DateTime to, int serverId) //Serverid to be implemented
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var serverDetails = ctx.ServerDetails.Where(x => x.Created > from
                    && x.Created < to).ToList(); 
                    return serverDetails;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetLatestServerDetailWithInterval:", e);
                throw;
            }
        }

        public ServerDetail GetLatestServerDetailByInterval(int interval)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var serverDetails = ctx.ServerDetails.Where(x => x.Created < DateTime.Now 
                    && x.Created > DateTime.Now.AddMinutes(-interval)).FirstOrDefault();
                    return serverDetails;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetLatestServerDetailWithInterval:", e);
                throw;
            }
        }

        public List<ServerDetail> GetOneDayServerDetail(int serverId)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var serverDetails = ctx.ServerDetails.Where(x => x.Created > DateTime.Now.AddDays(-1) && x.ServerId == serverId).ToList();
                    return serverDetails;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetOneDayServerDetail:", e);
                throw;
            }
        }
        public ServerDetail GetServerDetailByName(int serverId)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var details = ctx.ServerDetails.OrderByDescending(x=>x.Created).FirstOrDefault(x => x.ServerId == serverId);
                    return details;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetOneDayServerDetail:", e);
                throw;
            }
        }
    }
}
