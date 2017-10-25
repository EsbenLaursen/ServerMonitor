using log4net;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Controller
{
    public class SystemDomainController
    {
        protected ILog Log
        {
            get { return LogManager.GetLogger(GetType()); }
        }
        public string GetValueByTag(string tag)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    return ctx.SystemSetups.FirstOrDefault(x=>x.TagId==tag).Value;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - SystemSetup: " + e);
                throw;
            }
        }
        
    }
}
