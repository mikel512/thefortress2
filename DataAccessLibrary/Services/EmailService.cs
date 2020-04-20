using System;
using Microsoft.Extensions.Configuration;
using RestSharp;
using RestSharp.Authenticators;

namespace DataAccessLibrary.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IRestResponse SendMail(string from, string sender, string to, string subject, string body)
        {
           RestClient client = new RestClient ();
                   client.BaseUrl = new Uri ("https://api.mailgun.net/v3");
                   client.Authenticator =
                       new HttpBasicAuthenticator ("api",
                                                   _configuration["MailgunKey"]);
                   RestRequest request = new RestRequest ();
                   request.AddParameter ("domain", "thefortress.me", ParameterType.UrlSegment);
                   request.Resource = "{domain}/messages";
                   request.AddParameter ("from", $"{sender} <{from}>");
                   request.AddParameter ("to", to);
                   request.AddParameter ("subject", subject);
                   request.AddParameter ("text", body);
                   request.Method = Method.POST;
                   return client.Execute (request);
        }
    }
}