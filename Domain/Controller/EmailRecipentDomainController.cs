using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Controller
{
    public class EmailRecipentDomainController : CrudDomainController<EmailRecipent>
    {
        public List<EmailRecipent> GetAllEmailRecipents()
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    return ctx.EmailRecipents.ToList();
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetAllEmailRecipents: " + e);
                throw;
            }
        }

        public void DeleteEmailRecipent(EmailRecipent r)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var recipent = ctx.EmailRecipents.FirstOrDefault(x=>x.Id==r.Id);
                    if(recipent != null)
                    { 
                        ctx.EmailRecipents.DeleteOnSubmit(recipent);
                    }
                    ctx.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetAllEmailRecipents: " + e);
                throw;
            }
        }

    }
}
