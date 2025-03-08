using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace Agazaty.Data.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailsettings;
        public EmailService(IOptions<EmailSettings> options)
        {
            _emailsettings = options.Value;
        }
        public async Task SendEmail(EmailRequest emailRequest)
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_emailsettings.Email),
                Subject = emailRequest.Subject
            };
            email.To.Add(MailboxAddress.Parse(emailRequest.Email));
            var builder = new BodyBuilder();
            builder.HtmlBody = emailRequest.Body;
            email.Body = builder.ToMessageBody();
            email.From.Add(new MailboxAddress(_emailsettings.DisplayName, _emailsettings.Email));
            using var smtp = new SmtpClient();
            smtp.Connect(_emailsettings.Host, _emailsettings.Port, SecureSocketOptions.StartTls);

            smtp.Authenticate(_emailsettings.Email, _emailsettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
