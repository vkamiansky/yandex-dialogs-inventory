using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic.Email
{
    public class EmailHost
    {
        public static EmailHost Yandex = new EmailHost("smtp.yandex.ru", 25);
        
        public string Url { get; set; }

        public int Port { get; set; }

        public EmailHost(string url, int port)
        {
            Url = url;
            Port = port;
        }
    }
}
