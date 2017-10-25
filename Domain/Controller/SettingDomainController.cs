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
    public class SettingDomainController : CrudDomainController<Setting>
    {
        public List<Setting> GetSettingsWithTypes()
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    DataLoadOptions dlo = new DataLoadOptions();
                    dlo.LoadWith<Setting>(x=>x.SettingsType);
                    ctx.LoadOptions = dlo;
                    var settings = ctx.Settings.ToList();
                    return settings;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetSettings:", e);
                throw;
            }
        }

        public void UpdateSetting(Setting settings)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    Setting r = ctx.Settings.FirstOrDefault(x => x.Id == settings.Id);
                    if (r != null)
                    {
                        foreach (PropertyInfo prop in typeof(Setting).GetProperties())
                        {
                            if (UpdateAllowed(prop))
                            {
                                prop.SetValue(r, prop.GetValue(settings, null));
                            }

                        }
                        ctx.SubmitChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - UpdateSetting: " + e);
                throw;
            }
        }

        public int GetValueByName(string name)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var setting = ctx.Settings.FirstOrDefault(x=>x.Name == name);
                    return setting.Value;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetSettings:", e);
                throw;
            }
        }

        public Setting GetSettingById(int id)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var setting = ctx.Settings.FirstOrDefault(x => x.Id == id);
                    return setting;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetSettingById:", e);
                throw;
            }
        }
    }
}
