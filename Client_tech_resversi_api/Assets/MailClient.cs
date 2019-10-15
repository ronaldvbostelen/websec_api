using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Client_tech_resversi_api.Assets
{
    public class MailClient : SmtpClient, IMailTransferAgent
    {
        private const string smtpUsername = "nrmailman";
        private const string smtpPassword = "0y/3M(-FPB{YXZh0}AY@}(=xi";

        private readonly MailboxAddress mailAddress;
        private readonly string host;
        private readonly int port;
        private readonly SecureSocketOptions secureSocketOptions;

        private MimeMessage emailMessage;

        public MailClient()
        {
//            host = "mail.drysolidkiss.nl";
            host = "127.0.0.1";
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
            Authenticate(smtpUsername, smtpPassword);
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
