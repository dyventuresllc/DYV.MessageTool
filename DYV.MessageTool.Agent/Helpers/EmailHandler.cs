using SendGrid.Helpers.Mail;
using SendGrid;

namespace DYV.MessageTool.Agent.Helpers
{
    internal static class EmailHandler
    {
        internal static void SendEmail(string apiKey, string msgFrom, string msgTo, string msgSubject, string msgBody)
        {
            SendGridClient client = new SendGridClient(apiKey);
            SendGridMessage msg = new SendGridMessage();
            EmailAddress fromAddress = new EmailAddress($"{msgFrom}");
            
            msg.From = fromAddress;
            msg.AddTo(msgTo);
            msg.Subject = msgSubject;
            msg.HtmlContent = msgBody;
            msg.AddReplyTo("ltaswestcoast@quinnemanuel.com");
            msg.AddReplyTo("ltaseastcoast@quinnemanuel.com");

            client.SendEmailAsync(msg);
        }
    }
}
