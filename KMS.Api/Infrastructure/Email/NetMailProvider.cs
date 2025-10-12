using System.Net.Mail;
using System.Net;

namespace KMS.Api.Infrastructure.Email
{
    public class NetMailProvider : EmailStrategy
    {
        private readonly MailMessage message;
        private readonly SmtpClient smtp;
        private readonly string host;
        private readonly int port;
        private string fromMail;
        private string password;
        public NetMailProvider(string host, int port)
        {
            message = new MailMessage();
            smtp = new SmtpClient();
            this.host = host;
            this.port = port;
        }
        public override void SetSender(string fromMail, string password)
        {
            this.fromMail = fromMail;
            this.password = password;
        }
        public override void Send(string toEmail, string subject, string htmlContent, ref string errorMessage)
        {
            try
            {

                var sendMailThread = new Thread(() =>
                {
                    message.From = new MailAddress(fromMail);
                    message.To.Add(new MailAddress(toEmail));
                    message.Subject = subject;
                    message.IsBodyHtml = true;
                    message.Body = htmlContent;
                    smtp.Port = port;
                    smtp.Host = host;
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(fromMail, password);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    object userState = message;
                    smtp.Send(message);
                });
                sendMailThread.Start();
            }
            catch (Exception ex) { errorMessage = ex.Message; }
        }
    }
}
