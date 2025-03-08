namespace Agazaty.Data.Email
{
    public interface IEmailService
    {
        Task SendEmail(EmailRequest emailRequest);
    }
}
