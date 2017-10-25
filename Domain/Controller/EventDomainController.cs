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
    public class EventDomainController : CrudDomainController<Event>
    {
        public List<Event> GetAllEventsWithTypes()
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var dlo = new DataLoadOptions();
                    dlo.LoadWith<Event>(k => k.Notification);
                    dlo.LoadWith<Event>(e => e.EventType);
                    ctx.LoadOptions = dlo;
                    return ctx.Events.OrderByDescending(x => x.Created).ToList();
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetAllEventsWithTypes: " + e);
                throw;
            }
        }

        public List<Event> GetEventByRange(DateTime from, DateTime to)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var dlo = new DataLoadOptions();
                    dlo.LoadWith<Event>(d => d.Notification);
                    dlo.LoadWith<Event>(a => a.EventType);
                    dlo.LoadWith<Event>(t => t.RequestSummary);
                    dlo.LoadWith<Event>(x => x.ServerDetail);
                    ctx.LoadOptions = dlo;
                    var l = ctx.Events.ToList();
                    return ctx.Events.Where(x => x.Created > from && x.Created < to).OrderByDescending(x => x.Created).ToList();
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetEventByRange: " + e);
                throw;
            }
        }

        public Event GetEventByRequestSummaryId(int id)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    return ctx.Events.FirstOrDefault(x => x.RequestSummaryId == id);
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetByRequestSummaryId: " + e);
                throw;
            }
        }

        public void UpdateEvent(Event e)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    Event updateEvent = ctx.Events.FirstOrDefault(x => x.Id == e.Id);
                    if (updateEvent != null)
                    {
                        foreach (PropertyInfo prop in typeof(Event).GetProperties())
                        {
                            if (UpdateAllowed(prop))
                            {
                                prop.SetValue(updateEvent, prop.GetValue(e, null));
                            }

                        }
                        ctx.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Database error - UpdateEvent: " + ex);
                throw;
            }
        }
    
        public Event GetLatestServerUpEvent()
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    return ctx.Events.OrderByDescending(x => x.Created)
                        .FirstOrDefault(x => x.EventType.Name == StaticStrings.ServerUp);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Database error - GetLatestServerUpEvent: " + ex);
                throw;
            }
        }

        public Event GetLatestServerDownEvent()
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    return ctx.Events.OrderByDescending(x=>x.Created).FirstOrDefault(x => x.EventType.Name == StaticStrings.ServerDown);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Database error - GetLatestServerDownEvent: " + ex);
                throw;
            }
        }

        public List<Event> GetNumberOfEventsWithTypes(int numberOfEvents)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var dlo = new DataLoadOptions();
                    dlo.LoadWith<Event>(k => k.Notification);
                    dlo.LoadWith<Event>(e => e.EventType);
                    ctx.LoadOptions = dlo;
                    return ctx.Events.OrderByDescending(x => x.Created).Take(numberOfEvents).ToList();
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetNumberOfEventsWithTypes: " + e);
                throw;
            }
        }

        public Event GetEventWithRelationsById(int id)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var dlo = new DataLoadOptions();
                    dlo.LoadWith<Event>(d => d.Notification);
                    dlo.LoadWith<Event>(a => a.EventType);
                    dlo.LoadWith<Event>(t => t.RequestSummary);
                    dlo.LoadWith<Event>(x => x.ServerDetail);
                    ctx.LoadOptions = dlo;
                    return ctx.Events.FirstOrDefault(x => x.Id == id); //null
                }

            }
            catch (Exception ex)
            {
                Log.Error("Database error - GetEventById: " + ex);
                throw;
            }
        }

        public List<Event> GetLastEvents(int min)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    DataLoadOptions dlo = new DataLoadOptions();
                    dlo.LoadWith<Event>(l => l.EventType);
                    ctx.LoadOptions = dlo;
                    return ctx.Events.Where(x => x.Created < DateTime.Now && x.Created > DateTime.Now.AddMinutes(-min)).ToList();
                }

            }
            catch (Exception ex)
            {
                Log.Error("Database error - GetLastEvents: " + ex);
                throw;
            }
        }

        public void UpdateCurrentEvent(Event serverDownEvent)
        {

            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    Event e = ctx.Events.FirstOrDefault(x => x.Id == serverDownEvent.Id);
                    if (e != null)
                    {
                        foreach (PropertyInfo prop in typeof(Event).GetProperties())
                        {
                            if (UpdateAllowed(prop))
                            {
                                prop.SetValue(e, prop.GetValue(serverDownEvent, null));
                            }

                        }
                        ctx.SubmitChanges();
                    }
                }
            }
            catch (Exception e)
            {
                //Log.Error("Database error - UpdateCurrentEvent: " + e);
                throw;
            }

        }
    }
}
