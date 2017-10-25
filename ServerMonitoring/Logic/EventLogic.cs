using Domain;
using Model;
using ServerMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServerMonitoring.Entities;

namespace ServerMonitoring.Logic
{

    public class EventLogic
    {
        protected static readonly DomainFacade Facade = DomainFacade.Instance;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EventViewModel GetEventData()
        {
            EventViewModel evm = new EventViewModel();

            try
            {
                var types = Facade.EventTypeDomainController.GetAll();
                var events = Facade.EventDomainController.GetAllEventsWithTypes();

                evm.EventTypes = types;
                evm.Events = events;

                var notifications = Facade.NotificationDomainController.GetAllNotSeenNotifications();
                foreach (var n in notifications)
                {
                    n.Seen = true;
                    Facade.NotificationDomainController.UpdateToSeen(n);
                }
            }
            catch (Exception e)
            {
                log.Error("EventLogic - GetEventData: " + e);
            }

            return evm;
        }

        public List<Model.Event> GetLastEvents(int numberOfEvents)
        {
            return Facade.EventDomainController.GetNumberOfEventsWithTypes(numberOfEvents);
        }

      
    }
}