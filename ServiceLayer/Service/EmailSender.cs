using DomainLayer.Models;
using MailKit.Net.Smtp;
using MimeKit;
using ServiceLayer.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Service
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfiguraion;

        public EmailSender(EmailConfiguration emailConfiguration)
        {
            _emailConfiguraion = emailConfiguration; 
        }


        public void SendEmail(Message message) {
            var emailMessage = CreateEmailMessage(message);

            Send(emailMessage); 
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfiguraion.From, _emailConfiguraion.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

            return emailMessage; 
        }

        private void Send(MimeMessage mailMessage)
        {
            using(var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfiguraion.SmtpServer, _emailConfiguraion.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfiguraion.UserName, _emailConfiguraion.Password);

                    client.Send(mailMessage); 
                }
                catch
                {
                    throw; 
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose(); 
                }
            }
        }
    }
}
