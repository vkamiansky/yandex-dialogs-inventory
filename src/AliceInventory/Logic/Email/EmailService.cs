using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace AliceInventory.Logic.Email
{
    public static class EmailHelper
    {

        public static async Task SendEmailAsync(EmailHost host, string login, string password, MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync(host.Url, host.Port, false);
                await client.AuthenticateAsync(login, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
