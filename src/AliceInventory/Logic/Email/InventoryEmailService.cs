using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;

namespace AliceInventory.Logic.Email
{
    public class InventoryEmailService : EmailService, IInventoryEmailService
    {
        public InventoryEmailService(IConfigurationService config)
            : base(new EmailHost(config.MailingSmtpHost, int.Parse(config.MailingSmtpPort)),
                config.MailingAccountLogin, config.MailingAccountPassword) { }

        public async void SendListAsync(string email, Logic.Entry[] entries)
        {
            var message = CreateListMessage(email, entries);
            await SendEmailAsync(message);
        }

        private MimeMessage CreateListMessage(string email, Logic.Entry[] entries)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Навык Алисы - Рюкзак", _login));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = $"Ваш отчёт от {DateTime.Now.Date:dd.MM.yyyy}";
            emailMessage.Body = CreateHtmlBodyFromList(entries);
            return emailMessage;
        }

        private MimeEntity CreateHtmlBodyFromList(Logic.Entry[] entries)
        {
            var bodyBuilder = new BodyBuilder {HtmlBody = entries.ToTextList().Replace("\n", "<br>")};
            return bodyBuilder.ToMessageBody();
        }
    }
}
