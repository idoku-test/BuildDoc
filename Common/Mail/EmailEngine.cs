using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CN.SGS.Expense.Common
{
    /// <summary>
    /// 具体发送邮件类
    /// </summary>
    public class EmailEngine
    {
        public SmtpClient smtp;

        public MailMessage message;

        public bool Send(EmailEntity entity)
        {
            bool result = false;
            try
            {
                if (this.message == null)
                {
                    return false;
                }
                this.message.Subject = entity.Subject;
                this.message.Body = entity.Body;
                this.message.To.Clear();
                this.message.CC.Clear();
                this.message.Bcc.Clear();

                if (entity.To != null)
                {
                    foreach (MailAddress ma in entity.To)
                    {
                        if (!this.message.To.Contains(ma))
                            this.message.To.Add(ma);
                    }
                }
                if (entity.Cc != null)
                {
                    foreach (MailAddress ma in entity.Cc)
                    {
                        if (!this.message.CC.Contains(ma))
                            this.message.CC.Add(ma);
                    }
                }
                if (entity.Bcc != null)
                {
                    foreach (MailAddress ma in entity.Bcc)
                    {
                        if (!this.message.Bcc.Contains(ma))
                            this.message.Bcc.Add(ma);
                    }
                }
                this.smtp.Send(this.message);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                ex.ToString();
                //entity.Remark = ex.Message;
            }
            return result;
        }

        public EmailEngine(string mailSenderServer, int mailTimeOut, string mailSenderName, string mailSenderPassword)
        {
            if (smtp == null)
            {
                smtp = new SmtpClient();
            }
            InitMailSetting(mailSenderServer, mailTimeOut, mailSenderName, mailSenderPassword);
        }

        private void InitMailSetting(string mailSenderServer, int mailTimeOut, string mailSenderName, string mailSenderPassword)
        {
            smtp = new System.Net.Mail.SmtpClient(mailSenderServer);
            smtp.Timeout = mailTimeOut * 1000;
            smtp.Port = 25;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential(mailSenderName, mailSenderPassword);

            this.message = new System.Net.Mail.MailMessage();
            this.message.Sender = new System.Net.Mail.MailAddress(mailSenderName);
            this.message.From = new System.Net.Mail.MailAddress(mailSenderName);
            this.message.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnFailure;
            
            this.message.IsBodyHtml = true;
            this.message.BodyEncoding = System.Text.Encoding.UTF8;
            this.message.SubjectEncoding = System.Text.Encoding.UTF8;
        }
    }
}
