using System;
using System.Reflection;
using MMCV_Common;
using MMCV_Model.DocumentSign;
using MMCV_Model.Common;
using System.IO;
using System.Configuration;
using System.Net.Mail;

namespace MMCV_BLL.Email
{
    public static class EmailSender
    {
        private static string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        public static void DocumentSendMail(DocumentSignBO documentSignBO, EmailBO emailBO)
        { 
            try
            {
                var body = emailBO.Content;
                body = body.Replace("$SENDER_NAME$", documentSignBO.Sender);
                body = body.Replace("$DOCUMENT_NAME$", documentSignBO.Title);
                body = body.Replace("$LINK_TO_SIGN$", $"{baseUrl}Home/PdfPage?docId={documentSignBO.DocumentID}&email={documentSignBO.Email}&signIndex={documentSignBO.SignIndex}");
                emailBO.Content = body;

                SendEmail(emailBO);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteLog(ex.ToString(), ex, MethodBase.GetCurrentMethod().Name, "EmailSender");
            }
        }

        public static string ReadEmailTemplate(string path)
        {
            string body = string.Empty;
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    body = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {

            }
            return body;
        }

        public static bool SendEmail(EmailBO emailBO)
        {
            try
            {
                var from = "system@mmcv.mektec.com";
                var delimiter = "------------------------------------------------------------";
                var body = emailBO.Content;

                body += $@"<br><br><br>{delimiter}
    <br><a href='{baseUrl}'>MMCV E-Sign</a>
    <br><strong>MMCV IT Application Center</strong>";

                var message = new MailMessage();

                var toMails = emailBO.MailTo.Split(';');
                if (emailBO.MailTo.Contains(","))
                {
                    toMails = emailBO.MailTo.Split(',');
                }

                foreach (var toMail in toMails)
                {
                    if (toMail != "")
                    {
                        message.To.Add(new MailAddress(toMail)); // replace with valid value 
                    }
                }

                var toCCs = emailBO.CC.Split(';');
                if (emailBO.CC.Contains(","))
                {
                    toCCs = emailBO.CC.Split(',');
                }
                foreach (var tocc in toCCs)
                {
                    if (tocc != "")
                    {
                        message.CC.Add(new MailAddress(tocc)); // replace with valid value 
                    }
                }

                message.Subject = emailBO.Subject;
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
