using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Serialization;

namespace ServerMonitoring.Helpers
{
    public static class JsonHelpers
    {
        public static HtmlString JsonInsert(this HtmlHelper html, object o)
        {
            var json = JsonConvert.SerializeObject(o, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return new HtmlString(json);
        }

    }
}