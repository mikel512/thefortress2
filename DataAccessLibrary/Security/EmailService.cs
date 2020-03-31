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
        public string SendEmailConfirmation(string email)
        {
            
            return "";
        }
    }
}