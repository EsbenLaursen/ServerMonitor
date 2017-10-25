using Model;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Controller
{
    public class EventTypeDomainController : CrudDomainController<EventType>
    {
        public EventType GetTypeByName(string highNumberOfUsers)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    return ctx.EventTypes.FirstOrDefault(x => x.Name == highNumberOfUsers);
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetTypeByName: " + e);
                throw;
            }
        }

        public EventType GetTypeById(int id)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    return ctx.EventTypes.FirstOrDefault(x => x.Id == id);
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetTypeById: " + e);
                throw;
            }
        }

        public void UpdateEventType(EventType eventType)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    EventType e = ctx.EventTypes.FirstOrDefault(x => x.Id == eventType.Id);
                    if (e != null)
                    {
                        foreach (PropertyInfo prop in typeof(EventType).GetProperties())
                        {
                            if (UpdateAllowed(prop))
                            {
                                prop.SetValue(e, prop.GetValue(eventType, null));
                            }

                        }
                        ctx.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Database error - UpdateEventType: " + ex);
                throw;
            }
        }
    }
}
