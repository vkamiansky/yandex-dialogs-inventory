using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic.Email
{
    public class EmailHost
    {
        public static EmailHost Yandex = new EmailHost("Yandex", "smtp.yandex.ru");

        public string Name { get; set; }
        public string Url { get; set; }

        public EmailHost(string name, string url)
        {
            Name = name;
            Url = url;
        }
    }
}
