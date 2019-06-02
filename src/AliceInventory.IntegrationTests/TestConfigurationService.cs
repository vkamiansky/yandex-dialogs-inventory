using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AliceInventory.Logic;

namespace AliceInventory.IntegrationTests
{
    public class TestConfigurationService : IConfigurationService
    {
        public string MailingAccountLogin { get; } = "login";
        public string MailingAccountPassword { get; } = "password";
        public string MailingSmtpHost { get; } = "host";
        public string MailingSmtpPort { get; } = "0";
        public Task<string> GetIsConfigured()
        {
            return Task.FromResult("Vault value empty");
        }
    }
}
