using Domain;
using Model;
using ServerMonitoring.Logic;
using ServerMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace ServerMonitoring.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        public static int EVENT_SHOWN = 5;
        protected static readonly DomainFacade Facade = DomainFacade.Instance;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        public ActionResult Event()
        {
            EventLogic el = new EventLogic();
            EventViewModel rvm = el.GetEventData();
            //Notifications should be seen

            return View(rvm);
        }

        [HttpGet]
        public ActionResult GetNewEventDetails()
        {
            EventViewModel evm = new EventViewModel();
            evm.Events = new EventLogic().GetLastEvents(EVENT_SHOWN);
            evm.NotificationCount = Facade.NotificationDomainController.GetAll().Where(x => x.Seen == false).ToList().Count;
            return PartialView("~/Views/Server/_EventDetails.cshtml", evm);
        }


        [HttpGet]
        public int GetNotificationCount()
        {
            int count = Facade.NotificationDomainController.GetAll().Where(x => x.Seen == false).ToList().Count;
            return count;
        }

        [HttpGet]
        public ActionResult GetNotificationDetails(int id)
        {
            EventDialogModel edm = new EventDialogModel();
            edm.Event = Facade.EventDomainController.GetEventWithRelationsById(id);
            return PartialView("~/Views/Event/_EventInfoDialog.cshtml", edm);
        }


        [HttpPost]
        public HttpResponseMessage UpdateNotification(int id)
        {
            Notification notification = Facade.NotificationDomainController.GetNotification(id);
            if (notification != null)
            {
                notification.Seen = true;
                Facade.NotificationDomainController.UpdateToSeen(notification);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}