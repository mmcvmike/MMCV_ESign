using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;

namespace MMCV_Common.Helper
{
    public static class MailHelper
    {
        private static string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        public static bool SendEmail(string subject, string from, string to, string cc, string body)
        {
            try
            {
                var delimiter = "------------------------------------------------------------";
                body += $@"<br><br><br>{delimiter}
                         <br><a href='{baseUrl}'>MMCV E-Sign</a>
                         <br><strong>MMCV IT Innovation Center</strong>";

                var message = new MailMessage();

                var toMails = to.Split(';');
                if (to.Contains(","))
                {
                    toMails = to.Split(',');
                }
                
                foreach (var toMail in toMails)
                {
                    if (toMail != "")
                    {
                        message.To.Add(new MailAddress(toMail)); // replace with valid value 
                    }
                }

                var toCCs = cc.Split(';');
                if (cc.Contains(","))
                {
                    toCCs = cc.Split(',');
                }
                foreach (var tocc in toCCs)
                {
                    if (tocc != "")
                    {
                        message.CC.Add(new MailAddress(tocc)); // replace with valid value 
                    }
                }

                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    message.From = new MailAddress(from); // replace with valid value
                    smtp.Port = 25;
                    smtp.Host = "souras01.mekjpn.ngnet";
                    smtp.Send(message);
                }
            }
            catch (Exception)
            {

            }

            return true;
        }
    }
}
