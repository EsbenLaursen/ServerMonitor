using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
namespace Domain.Controller
{
    public class ServerDomainController : CrudDomainController<Server>
    {

        public Server GetServerById(int serverId)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    return ctx.Servers.FirstOrDefault(x=>x.Id == serverId);
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetServerById:", e);
                throw;
            }
        }
        public List<Server> GetAllServers()
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var servers = ctx.Servers.Distinct().ToList();
                    return servers;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetOneDayServerDetail:", e);
                throw;
            }
        }
        public Server CreateServer(Server server)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    ctx.Servers.InsertOnSubmit(server);
                    ctx.SubmitChanges();
                    return server;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - CreateServer:", e);
                throw;
            }
        }

        public Server GetServerByName(string serverName)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    return ctx.Servers.FirstOrDefault(x => x.Name == serverName);  
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - CreateServer:", e);
                throw;
            }
        }
    }
}
