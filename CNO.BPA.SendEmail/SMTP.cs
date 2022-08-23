using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using log4net;

namespace CNO.BPA.SendEmail
{
    class SMTP
    {
        //Logging log = new Logging();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void SendMail(string From, string To,
                     string Subject, string Body)
        {
            try
            {
                //Config config = new Config();

                //MailMessage represents the e-mail being sent
                using (MailMessage message = new MailMessage(From,
                       To, Subject, Body))
                {
                    message.IsBodyHtml = true;
                    message.Body = Body;
                    message.CC.Add("IA_EMAIL_CONFIRMATION_CC@cnoinc.com");
                    SmtpClient mailClient = new SmtpClient("smtp.cnoinc.com");
                    mailClient.UseDefaultCredentials = true;
                    mailClient.Send(message);
                    log.Debug("SMTP Sent");
                } 
            }
            catch (SmtpException ex)
            {
                log.Error("CNO.BPA.SendEmail Error in SendMail: " + ex.Message);
                throw new SmtpException
                   ("SmtpException has occured: " + ex.Message);
            }
            catch (Exception ex)
            {
                log.Error("CNO.BPA.SendEmail Error in SendMail: UNHANDLED EXCEPTION: " + ex.Message);
                throw new Exception("UNHANDLED ERROR: " + ex.Message);
            }
        }
    }
}
