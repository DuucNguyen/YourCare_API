﻿using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace YourCare_WebApi.Services.EmailSender
{
    public class MailKitEmailSender : IEmailSender
    {
        public MailKitEmailSenderOptions Options { get; set; }

        public MailKitEmailSender(IOptions<MailKitEmailSenderOptions> options)
        {
            this.Options = options.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(email, subject, message);
        }

        public Task Execute(string to, string subject, string message)
        {
            // create message
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(Options.Sender_Email);

            if (!string.IsNullOrEmpty(Options.Sender_Name))
                email.Sender.Name = Options.Sender_Name;

            email.From.Add(email.Sender);
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = message };

            // send email
            using (var smtp = new SmtpClient())
            {
                smtp.Connect(Options.Host_Address, Options.Host_Port, SecureSocketOptions.StartTls);

                smtp.Authenticate(Options.Host_Username, Options.Host_Password);

                smtp.Send(email);

                smtp.Disconnect(true);
            }

            return Task.FromResult(true);
        }
    }
}
