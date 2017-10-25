using Domain;
using Model;
using ServerMonitoring.Logic;
using ServerMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace ServerMonitoring.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        protected static readonly DomainFacade Facade = DomainFacade.Instance;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        
        public ActionResult Index()
        {
            UserLogic gl = new UserLogic();
            RequestLogic rl = new RequestLogic();
            ServerLogic sl = new ServerLogic();

            //Different models
            RequestDataViewModel rdvm = rl.GetRequestResponseTimeGraphData();
            ServerDataViewModel sdvm = sl.GetServerData(10);
            SettingLogic s = new SettingLogic();
            //events
            EventViewModel evm = new EventViewModel();
            evm.Events = new EventLogic().GetLastEvents(5);
            evm.NotificationCount = Facade.NotificationDomainController.GetAll().Where(x => x.Seen == false).ToList().Count;
            
            //MasterViewModel
            DashBoardViewModel dbvm = gl.GetUsersGraphData();
            dbvm.RequestDataViewModel = rdvm;
            dbvm.ServerDataViewModel = sdvm;
            dbvm.SettingsModel = s.GetSettings();
            dbvm.EventViewModel = evm;
            //   DashBoardViewModel dbvm = gl.GetGraphDataOneHourInterval();
            return View(dbvm);
        } 

        [HttpGet]
        public ActionResult GetUserGraph() {
            UserLogic gl = new UserLogic();
            DashBoardViewModel dbvm = gl.GetUsersGraphData();
            return PartialView("~/Views/Graph/_UserActivityGraphDialog.cshtml", dbvm);
        }


    }
}