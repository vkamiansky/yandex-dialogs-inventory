using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic
{
    public interface IConfigurationService
    {
        Task<string> GetMailingAccountLogin();
        Task<string> GetMailingAccountPassword();
        Task<string> GetMailingSmtpHost();
        Task<int> GetMailingSmtpPort();
        Task<string> GetIsConfigured();
    }

}
