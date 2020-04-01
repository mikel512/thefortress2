using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace DataAccessLibrary.Security
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string SendEmail(string sender, string recipient, string subject, string body)
        {
            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(sender);
                mail.To.Add(new MailAddress(recipient));
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (var smtpClient = new SmtpClient())
                {
                    var cred = new NetworkCredential
                    {
                        UserName = "postmaster@mg.thefortress.me",
                        Password = _configuration["MailSmtpKey"]
                    };
                    smtpClient.Credentials = cred;
                    smtpClient.Host = "smtp.mailgun.org";
                    smtpClient.Port = 25;
                    smtpClient.EnableSsl = false;
                    smtpClient.Send(mail);
                }
            } 
            return "";
        }
    }
}