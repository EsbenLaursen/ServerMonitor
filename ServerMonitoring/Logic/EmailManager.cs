using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model;
using System.Net.Mail;
using ServerMonitoring.Models;

namespace ServerMonitoring.Logic
{
    public class EmailManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public String SMTP { get; set; }
        public String SmtpUsername { get; set; }
        public String Smtppassw { get; set; }
        public String Fromadress { get; set; }
        public String Fromname { get; set; }

        public EmailManager(string smtp, string smtpUsername, string smtppassw, string fromadress, string fromname)
        {
            SMTP = smtp;
            SmtpUsername = smtpUsername;
            Smtppassw = smtppassw;
            Fromadress = fromadress;
            Fromname = fromname;
        }

        public EmailManager() {

        }

        internal void SendMail(ReportViewModel rvm)
        {
            SmtpClient smtpClient = new SmtpClient("smtp-mail.outlook.com", 25);

            smtpClient.Credentials = new System.Net.NetworkCredential("info@MyWebsiteDomainName.com", "myIDPassword");
            smtpClient.UseDefaultCredentials = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            MailMessage mail = new MailMessage();

            //Setting From , To and CC
            mail.From = new MailAddress("info@MyWebsiteDomainName", "MyWeb Site");
            mail.To.Add(new MailAddress("info@MyWebsiteDomainName"));
            mail.CC.Add(new MailAddress("MyEmailID@gmail.com"));

            smtpClient.Send(mail);
        }
    }
}