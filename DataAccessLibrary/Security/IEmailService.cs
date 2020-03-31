namespace DataAccessLibrary.Security
{
    public interface IEmailService
    {
        string SendEmailConfirmation(string email);
    }
}