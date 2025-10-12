using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Api.Infrastructure.Email
{
    public class EmailService
    {
        private EmailStrategy emailStrategy;
        public void SetEmailProvider(EmailStrategy emailStrategy)
        {
            this.emailStrategy = emailStrategy;
        }
        public void Send(string toEmail, string subject, string htmlContent, ref string errorMessage)
        {
            emailStrategy.Send(toEmail, subject, htmlContent, ref errorMessage);
        }
    }
}
