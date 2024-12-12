﻿using MailKit.Security;

namespace YourCare_WebApi.Services.EmailSender
{
    public class MailKitEmailSenderOptions
    {
        public MailKitEmailSenderOptions()
        {
            Host_SecureSocketOptions = SecureSocketOptions.Auto;
        }

        public SecureSocketOptions Host_SecureSocketOptions { get; set; }

        public string Host_Address { get; set; }

        public int Host_Port { get; set; }

        public string Host_Username { get; set; }

        public string Host_Password { get; set; }

        public string Sender_Email { get; set; }

        public string Sender_Name { get; set; }
    }
}
