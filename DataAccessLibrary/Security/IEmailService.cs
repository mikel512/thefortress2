namespace DataAccessLibrary.Security
{
    public interface IEmailService
    {
        string SendEmail(string sender, string recipient, string subject, string body);
    }
}