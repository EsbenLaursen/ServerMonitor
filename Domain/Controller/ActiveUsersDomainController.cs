using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Controller
{
    public class ActiveUsersDomainController : CrudDomainController<ActiveUser>
    {
        public ActiveUser GetActiveUsers(int interval)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    return ctx.ActiveUsers.Where(x => x.Created > DateTime.Now.AddMinutes(-interval)).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetActiveUsers:", e);
                throw;
            }
        }

        public ActiveUser CreateNewActiveUsers(ActiveUser activeUser)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    activeUser.Created = DateTime.Now;
                    ctx.ActiveUsers.InsertOnSubmit(activeUser);
                    ctx.SubmitChanges();
                    return activeUser;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - CreateNewActiveUsers:", e);
                throw;
            }
        }


        public void UpdateActiveUsers(ActiveUser activeUsers)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    ActiveUser a = ctx.ActiveUsers.FirstOrDefault();
                    if (a != null)
                    {
                        foreach (PropertyInfo prop in typeof(ActiveUser).GetProperties())
                        {
                            //if (UpdateAllowed(prop))
                            //{
                                prop.SetValue(a, prop.GetValue(activeUsers, null));
                            //}
                        }
                        ctx.SubmitChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - UpdateActiveUsers: " + e);
                throw;
            }
        }

        public ActiveUser GetActiveUsers()
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    return ctx.ActiveUsers.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetActiveUsers:", e);
                throw;
            }
        }
    }
}
