using CateringManagement.ViewModels;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace CateringManagement.Utilities
{
    public class MyEmailSender : IMyEmailSender
    {
        private readonly IEmailConfiguration _emailConfiguration;

        public MyEmailSender(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        /// <summary>
        /// Asynchronously builds and sends a message to a single recipient
        /// </summary>        
        /// <param name="name">Optional - Uses the email if not supplied</param>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="htmlMessage"></param>
        /// <returns></returns>
        public async Task SendOneAsync(string name, string email, string subject, string htmlMessage)
        {
            //only sends emails to this email for testing purposes
            if (email != "nhilu1@ncstudents.niagaracollege.ca" || email != "jkaluba@niagaracollege.ca" || email != "dstovell@niagaracollege.ca")
                return;

            if (String.IsNullOrEmpty(name))
            {
                name = email;
            }

            var message = new MimeMessage();
            message.To.Add(new MailboxAddress(name, email));
            message.From.Add(new MailboxAddress(_emailConfiguration.SmtpFromName, _emailConfiguration.SmtpUsername));

            message.Subject = subject;
            //We will say we are sending HTML. But there are options for plaintext etc. 
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = htmlMessage
            };

            //Be careful that the SmtpClient class is the one from Mailkit not the framework!
            using var emailClient = new SmtpClient();
            //The last parameter here is to use SSL (Which you should!)
            emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, false);

            //Remove any OAuth functionality as we won't be using it. 
            emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

            emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

            await emailClient.SendAsync(message);

            emailClient.Disconnect(true);
        }

        /// <summary>
        /// Asynchronously sends a message to a List of email addresses
        /// </summary>
        /// <param name="emailMessage"></param>
        /// <returns></returns>
        public async Task SendToManyAsync(EmailMessage emailMessage)
        {
            //only sends emails to this email for testing purposes
            emailMessage.ToAddresses = emailMessage.ToAddresses.Where(a => a.Address == "nhilu1@ncstudents.niagaracollege.ca").Select(a => a).ToList();
            if (emailMessage.ToAddresses.Count() == 0)
                return;


            var message = new MimeMessage();
            message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

            message.From.Add(new MailboxAddress(_emailConfiguration.SmtpFromName, _emailConfiguration.SmtpUsername));

            message.Subject = emailMessage.Subject;
            //We will say we are sending HTML. But there are options for plaintext etc. 
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = emailMessage.Content
            };

            //Be careful that the SmtpClient class is the one from Mailkit not the framework!
            using var emailClient = new SmtpClient();
            //The last parameter here is to use SSL (Which you should!)
            emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, false);

            //Remove any OAuth functionality as we won't be using it. 
            emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

            emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

            await emailClient.SendAsync(message);

            emailClient.Disconnect(true);
        }
    }
}
