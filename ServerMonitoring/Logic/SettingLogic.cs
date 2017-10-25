using Domain;
using Model;
using ServerMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerMonitoring.Logic
{
    public class SettingLogic
    {
        protected static readonly DomainFacade Facade = DomainFacade.Instance;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SettingsModel GetSettings()
        {
            SettingsModel sm = new SettingsModel();
            try
            {
                List<Setting> settings = Facade.SettingDomainController.GetSettingsWithTypes();

                List<SettingData> graphSettings = new List<SettingData>();
                List<SettingData> serverSettings = new List<SettingData>();
                List<SettingData> eventSettings = new List<SettingData>();
                List<SettingData> allSettings = new List<SettingData>();

                foreach (var setting in settings)
                {
                    ConvertData(setting, allSettings);
                    switch (setting.SettingsType.Name)
                    {
                        case "Graph":
                            ConvertData(setting, graphSettings);
                            break;
                        case "Server":
                            ConvertData(setting, serverSettings);
                            break;
                        case "Event":
                            ConvertData(setting, eventSettings);
                            break;
                    }
                }
                sm.GraphSettings = graphSettings;
                sm.ServerSettings = serverSettings;
                sm.EventSettings = eventSettings;
                sm.Settings = allSettings;

                sm.EventTypes = Facade.EventTypeDomainController.GetAll();

                sm.EmailRecipents = Facade.EmailRecipentDomainController.GetAllEmailRecipents();

            }
            catch (Exception e)
            {
                log.Error("SettingLogic - GetSettings: " + e);
            }

            return sm;

        }

        private static void ConvertData(Setting setting, List<SettingData> settings)
        {
            try
            {
                SettingData data = new SettingData();
                data.Id = setting.Id;
                data.Name = setting.Name;
                data.Value = setting.Value;
                data.Description = setting.Description;
                settings.Add(data);
            }
            catch (Exception e)
            {
                log.Error("SettingLogic - ConvertData: " + e);
            }
        }
    }
}