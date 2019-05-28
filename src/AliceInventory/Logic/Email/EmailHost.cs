using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic.Email
{
    public class EmailHost
    {
        public static EmailHost Yandex = new EmailHost("Yandex", "smtp.yandex.ru", 25);

        public string Name { get; set; }
        public string Url { get; set; }

        public int Port { get; set; }

        public EmailHost(string name, string url, int port)
        {
            Name = name;
            Url = url;
            Port = port;
        }
    }
}
