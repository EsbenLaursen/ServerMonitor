using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Controller
{
    public class RequestDomainController : CrudDomainController<Request>
    {
     
        public void DeleteAllOlderThanInterval(int interval)
        {
            try {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var requestsToDelete = ctx.Requests.Where(x=>x.DateCreated < DateTime.Now.AddMinutes(-interval)).ToList();
                    foreach (var r in requestsToDelete)
                    {
                        ctx.Requests.DeleteOnSubmit(r);
                    }
                    ctx.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetRequests:", e);
                throw;
            }
        }
    }
}
