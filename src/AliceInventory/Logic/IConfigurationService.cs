using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic
{
    public interface IConfigurationService
    {
        string MailingAccountLogin { get; }
        string MailingAccountPassword { get; }
        string MailingSmtpHost { get; }
        string MailingSmtpPort { get; }
    }
}
