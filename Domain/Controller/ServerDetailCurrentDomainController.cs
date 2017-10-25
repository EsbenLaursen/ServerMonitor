using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Controller
{
    public class ServerDetailCurrentDomainController : CrudDomainController<ServerDetailCurrent>
    {
        public ServerDetailCurrent CreateServerDetailCurrent(ServerDetailCurrent serverDetailCurrent)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    serverDetailCurrent.Created = DateTime.Now;
                    ctx.ServerDetailCurrents.InsertOnSubmit(serverDetailCurrent);
                    ctx.SubmitChanges();
                    return serverDetailCurrent;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - CreateServerDetailCurrent:", e);
                throw;
            }
        }

        public bool HasAchievedDataLastFifteenSeconds()
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    bool dataAchieved = ctx.ServerDetailCurrents.Where(x => x.Created > DateTime.Now.AddSeconds(-15))
                        .ToList().Count == 0 ? false : true;
                    return dataAchieved;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - HasAchievedDataLastMinut " + e);
                throw;
            }
        }

        public ServerDetailCurrent GetLatest()
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    return ctx.ServerDetailCurrents.OrderByDescending(x=>x.Created).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetNewest " + e);
                throw;
            }
        }

        public void DeleteAllOlderThanInterval(int interval)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var serverDetailsToDelete = ctx.ServerDetailCurrents.Where(x => x.Created < DateTime.Now.AddMinutes(-interval)).ToList();
                    foreach (var s in serverDetailsToDelete)
                    {
                        ctx.ServerDetailCurrents.DeleteOnSubmit(s);
                    }
                    ctx.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - Serverdetailcurrent - DeleteAllOlderThanInterval: " + e);
                throw;
            }
        }

       
    }
}
