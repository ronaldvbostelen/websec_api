using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;

namespace Client_tech_resversi_api.Assets.Interfaces
{
    public interface IEmailer
    {
        Task SendActivationMailAsync(MailboxAddress rcpt, string username, string activationKey);
        Task SendRecoverMailAsync(MailboxAddress rcpt, string username, string resetKey);
    }
}
