using System;
using MimeKit;

namespace AliceInventory.Logic.Email
{
    public class InventoryEmailService : IInventoryEmailService
    {
        private IConfigurationService Configuration { get; set; }

        public InventoryEmailService(IConfigurationService config)
        {
            Configuration = config;
        }

        public async void SendListAsync(string email, Logic.Entry[] entries)
        {
            try
            {
                var host = new EmailHost(await Configuration.GetMailingSmtpHost(), await Configuration.GetMailingSmtpPort());
                var login = await Configuration.GetMailingAccountLogin();
                var password = await Configuration.GetMailingAccountPassword();
                var message = CreateListMessage(email, login, entries);
                await EmailHelper.SendEmailAsync(host, login, password, message);
            }
            catch
            {
                // ignored
            }
        }

        private MimeMessage CreateListMessage(string receiverEmail, string senderEmail, Logic.Entry[] entries)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Навык Алисы - Учёт", senderEmail));
            emailMessage.To.Add(new MailboxAddress("", receiverEmail));
            emailMessage.Subject = $"Ваш список от {DateTime.Now.Date:dd.MM.yyyy}";
            emailMessage.Body = CreateHtmlBodyFromList(entries);
            return emailMessage;
        }

        private MimeEntity CreateHtmlBodyFromList(Logic.Entry[] entries)
        {
            var bodyBuilder = new BodyBuilder {HtmlBody = entries.ToHtml()};
            return bodyBuilder.ToMessageBody();
        }
    }
}
