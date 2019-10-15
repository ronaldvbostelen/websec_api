using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets.Interfaces;
using Client_tech_resversi_api.Models.Non_DB_models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Client_tech_resversi_api.Assets
{
    public class MailClient : SmtpClient, IMailTransferAgent
    {
        private readonly SmtpUser User;
        private readonly MailboxAddress mailAddress;
        private readonly string host;
        private readonly int port;
        private readonly SecureSocketOptions secureSocketOptions;

        private MimeMessage emailMessage;

        public MailClient(IOptions<SmtpUser> user)
        {
            User = new SmtpUser
            {
                Username = user.Value.Username,
                Password = user.Value.Password,
                Server = user.Value.Server
            };

            host = User.Server;
            port = 587;
            secureSocketOptions = SecureSocketOptions.StartTls;
            mailAddress = new MailboxAddress("DrySolidKiss.nl", "noreply@drysolidkiss.nl");
            ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
        }

        public IMailTransferAgent EmailMessage(MailboxAddress rcpt, string subject, string body)
        {
            throw new NotImplementedException();
        }

        public IMailTransferAgent EmailMessage(MailboxAddress rcpt, string subject, string body, string htmlBody)
        {
            var bodyBuilder = new BodyBuilder
            {
                TextBody = body,
                HtmlBody = htmlBody
            };

            var email = new MimeMessage
            {
                Subject = subject,
                Body = bodyBuilder.ToMessageBody()
            };

            email.From.Add(mailAddress);
            email.To.Add(rcpt);

            emailMessage = email;

            return this;
        }

        public async Task<IMailTransferAgent> SendEmailAsync()
        {
            if (emailMessage == null)
                throw new ArgumentNullException(nameof(emailMessage), "Email was not set");

            Connect(host, port, secureSocketOptions);
            Authenticate(User.Username, User.Password);
            await SendAsync(emailMessage);
            Disconnect(true);
            return this;
        }

        public IMailTransferAgent SendEmail()
        {
            throw new NotImplementedException();
        }
    }
}
