using Microsoft.Extensions.Configuration;
using QRSharingApp.ActivationWebApp.ConfigurationOptions;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace QRSharingApp.ActivationWebApp.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfigurationOption _emailConfigurationOption;

        public EmailSender(IConfiguration configuration)
        {
            _emailConfigurationOption = configuration
                .GetSection(EmailConfigurationOption.SectionName)
                .Get<EmailConfigurationOption>();
        }

        public async Task SendAsync(MailAddress toAddress, string subject, string htmlMessage)
        {
            var fromAddress = new MailAddress(_emailConfigurationOption.Email);
            var smtp = new SmtpClient
            {
                Host = _emailConfigurationOption.Host,
                Port = _emailConfigurationOption.Port,
                EnableSsl = _emailConfigurationOption.EnableSSL,
                Credentials = new NetworkCredential(_emailConfigurationOption.Email, _emailConfigurationOption.Password)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(message);
        }
    }
}
