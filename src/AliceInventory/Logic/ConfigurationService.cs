using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic
{
    public class ConfigurationService : IConfigurationService
    {
        public string MailingAccountLogin { get; }
        public string MailingAccountPassword { get; }
        public string MailingSmtpHost { get; }
        public string MailingSmtpPort { get; }
    }
}
