using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace AliceInventory.Logic.Email
{
    public class EmailService
    {
        protected EmailHost _host;
        protected string _login;
        protected string _password;

        public EmailService(EmailHost host, string login, string password)
        {
            _login = login;
            _password = password;
            _host = host;
        }

        public async Task SendEmailAsync(MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_host.Url, 25, false);
                await client.AuthenticateAsync(_login, _password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
