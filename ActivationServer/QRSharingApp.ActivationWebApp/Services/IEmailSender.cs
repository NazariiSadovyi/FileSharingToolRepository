using System.Net.Mail;
using System.Threading.Tasks;

namespace QRSharingApp.ActivationWebApp.Services
{
    public interface IEmailSender
    {
        Task SendAsync(MailAddress toAddress, string subject, string htmlMessage);
    }
}
