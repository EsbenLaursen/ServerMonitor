using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Controller
{

    public class NotificationDomainController : CrudDomainController<Notification>
    {
        public Notification Create(Notification n)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    n.Seen = false;
                    n.Created = DateTime.Now;
                    ctx.Notifications.InsertOnSubmit(n);
                    ctx.SubmitChanges();
                    return n;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - NotificationDomainController Create: " + e);
                throw;
            }

        }

        public List<Notification> GetAllNotSeenNotifications()
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    return ctx.Notifications.Where(x => x.Seen == false).ToList();
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetAllNotSeenNotifications: " + e);
                throw;
            }
        }

        public void UpdateToSeen(Notification nofitication)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    Notification n = ctx.Notifications.FirstOrDefault(x => x.Id == nofitication.Id);
                    if (n != null)
                    {
                        foreach (PropertyInfo prop in typeof(Notification).GetProperties())
                        {
                            if (UpdateAllowed(prop))
                            {
                                prop.SetValue(n, prop.GetValue(nofitication, null));
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

        public Notification GetNotification(int id)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    return ctx.Notifications.FirstOrDefault(x => x.Id == id);
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetNotification: " + e);
                throw;
            }
        }
    }
}
