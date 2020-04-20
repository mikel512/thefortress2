using RestSharp;

namespace DataAccessLibrary.Services
{
    public interface IEmailService
    {
        IRestResponse SendMail(string from, string sender, string to, string subject, string body);
    }
}