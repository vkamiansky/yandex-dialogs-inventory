using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;

namespace AliceInventory.Logic.Email
{
    public class AliceEmailService : EmailService, IAliceEmailService
    {
        public AliceEmailService(string login, string password) : base(EmailHost.Yandex, login, password) { }

        public async void SendListAsync(string email, Data.Entry[] entries)
        {
            var message = CreateListMessage(email, entries);
            await SendEmailAsync(message);
        }

        private MimeMessage CreateListMessage(string email, Data.Entry[] entries)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Навык Алисы - Рюкзак", _login));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = $"Ваш отчёт от {DateTime.Now.Date:dd.MM.yyyy}";
            emailMessage.Body = CreateHtmlBodyFromList(entries);
            return emailMessage;
        }

        private MimeEntity CreateHtmlBodyFromList(Data.Entry[] entries)
        {
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = CreateHtmlFromList(entries);
            return bodyBuilder.ToMessageBody();
        }

        private string CreateHtmlFromList(Data.Entry[] entries)
        {
            var stringBuilder = new StringBuilder();
            foreach (var entry in entries)
            {
                stringBuilder.Append($"{entry.Name}: {entry.Count} {entry.Unit}<br>");
            }

            return stringBuilder.ToString();
        }
    }
}
