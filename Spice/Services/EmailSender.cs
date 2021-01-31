using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Services
{
    public class EmailSender : IEmailSender
    {

        public EmailOptions EmailOptions { get; set; }

        public EmailSender(IOptions<EmailOptions> options)
        {
            EmailOptions = options.Value;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(EmailOptions.ApiKey,subject,message,email) ;
        }

        private Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage() 
            {
                From = new EmailAddress(SD.CompanyInformations.adminEmail, SD.CompanyInformations.Name),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };

            //address we want to send to
            msg.AddTo(new EmailAddress(email));
            //send email
            try
            {
                return client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return null;
        }
    }
}
