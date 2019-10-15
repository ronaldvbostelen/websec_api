using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;

namespace Client_tech_resversi_api.Assets
{
    public class Emailer
    {
        public async Task SendActivationMailAsync(MailboxAddress rcpt, string username, string activationKey)
        {
            using (var mailClient = new MailClient())
            {
                var body = $"Greetings {username}\r\n Your activation code: {activationKey}\r\n Please activate your account www.localhost.nl/activate";
                var htmlBody = $"Greetings {username} <br/> Your activation code: {activationKey}<br/> Please activate your account <a href=\"drysolidkiss.nl/activate\">HERE</a>"; 
                try
                {
                    await mailClient.EmailMessage(rcpt, "Activation email", body, htmlBody).SendEmailAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public async Task SendRecoverMailAsync(MailboxAddress rcpt, string username, string resetKey)
        {
            using (var mailClient = new MailClient())
            {
                var body = $"Greetings {username}\r\n Your reset code: {resetKey}\r\n Please reset your password here: www.localhost.nl/reset";
                var htmlBody = $"Greetings {username} <br/> Your reset code: {resetKey}<br/> Please reset your password <a href=\"drysolidkiss.nl/reset\">HERE</a>";
                try
                {
                    await mailClient.EmailMessage(rcpt, "Reset code for your account", body, htmlBody).SendEmailAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}
