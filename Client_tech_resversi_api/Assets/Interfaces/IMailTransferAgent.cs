using System.Threading.Tasks;
using MimeKit;

namespace Client_tech_resversi_api.Assets.Interfaces
{
    public interface IMailTransferAgent
    {
        IMailTransferAgent EmailMessage(MailboxAddress rcpt, string subject, string body);
        IMailTransferAgent EmailMessage(MailboxAddress rcpt, string subject, string body, string htmlBody);
        IMailTransferAgent SendEmail();
        Task<IMailTransferAgent> SendEmailAsync();
    }
}