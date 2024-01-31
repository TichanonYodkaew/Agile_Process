using Microsoft.Extensions.Options;
using MimeKit;
//using System.Net;
//using System.Net.Mail;
using MailKit.Net.Smtp;
using MimeKit;

namespace AgileRap_Process2
{
    public class EmailSender : IEmailSender
    {

        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public void SendEmail(List<string> toList, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Controller", _emailSettings.UserName));

            foreach (var to in toList)
            {
                message.To.Add(new MailboxAddress("Operator", to));
            }

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect(_emailSettings.Host, _emailSettings.Port, false);
                client.Authenticate(_emailSettings.UserName, _emailSettings.Password);
                client.Send(message);
                client.Disconnect(true);
            }
        }

    }
}
