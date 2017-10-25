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
    public class SettingController : Controller
    {
        protected static readonly DomainFacade Facade = DomainFacade.Instance;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Settings
        [HttpPost]
        public ActionResult UpdateSetting(SettingsModel model)
        {
            if (ModelState.IsValid)
            {
                //Refactor this
                if (model.EventSettings != null)
                {
                    foreach (var setting in model.EventSettings)
                    {
                        Setting s = Facade.SettingDomainController.GetSettingById(setting.Id);
                        s.Value = setting.Value;
                        Facade.SettingDomainController.UpdateSetting(s);
                    }
                }
                if (model.ServerSettings != null)
                {
                    foreach (var setting in model.ServerSettings)
                    {
                        Setting s = Facade.SettingDomainController.GetSettingById(setting.Id);
                        s.Value = setting.Value;
                        Facade.SettingDomainController.UpdateSetting(s);
                    }
                }
                if (model.GraphSettings != null)
                {
                    foreach (var setting in model.GraphSettings)
                    {
                        Setting s = Facade.SettingDomainController.GetSettingById(setting.Id);
                        s.Value = setting.Value;
                        Facade.SettingDomainController.UpdateSetting(s);

                    }
                }
                if(model.EventTypes != null)
                {
                    foreach (var type in model.EventTypes)
                    {
                        EventType t = Facade.EventTypeDomainController.GetTypeById(type.Id);
                        t.PeakValue = type.PeakValue;
                        t.Notify = type.Notify;
                        t.Risk = type.Risk;
                        Facade.EventTypeDomainController.UpdateEventType(t);
                    }
                }

            }
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public ActionResult GetSettings()
        {

            SettingsModel model = new SettingLogic().GetSettings();
            return PartialView("~/Views/Settings/_SettingsDialog.cshtml", model);
        }
    }
}