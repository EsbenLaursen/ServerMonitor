using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ServerMonitoring.Models
{
    public class SettingsModel
    {
        public List<SettingData> Settings { get; set; }
        public List<SettingData> GraphSettings { get; set; }
        public List<SettingData> ServerSettings { get; set; }
        public List<SettingData> EventSettings { get; set; }

        //These are also kind of settings - because it should be possible to change the risk, notify and peakvalue
        public List<EventType> EventTypes { get; set; }
        public List<EmailRecipent> EmailRecipents { get; set; }
    }

    public class SettingData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Count must be a natural number")]
        public int Value { get; set; }
        public string Description { get; set; }
    }
}