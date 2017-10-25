using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerMonitoring.DAL.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class MonitorRequest
    {
        //Potentiel server id
        
        /// <summary>
        /// the id for the request
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// the url for the request
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// the time the request was made
        /// </summary>
        public DateTime DateCreated { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Device { get; set; }
        public double RequestTimeMiliSeconds { get; set; }

    }
}