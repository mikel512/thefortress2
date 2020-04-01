using System.Threading.Tasks;
using RestSharp;

namespace DataAccessLibrary.Security
{
    public interface IEmailService
    {
        IRestResponse SendMail(string from, string sender, string to, string subject, string body);
    }
}