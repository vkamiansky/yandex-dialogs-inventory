using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AliceInventory.Logic;

namespace AliceInventory.IntegrationTests
{
    public class TestConfigurationService : IConfigurationService
    {
        public Task<string> GetMailingAccountLogin()
        {
            return null;
        }

        public Task<string> GetMailingAccountPassword()
        {
            return null;
        }

        public Task<string> GetMailingSmtpHost()
        {
            return null;
        }

        public Task<string> GetDbConnectionString()
        {
            return null;
        }

        public Task<string> GetDbName()
        {
            return null;
        }

        public Task<int> GetMailingSmtpPort()
        {
            return Task.FromResult(0);
        }

        public Task<string> GetIsConfigured()
        {
            return Task.FromResult("Vault value empty");
        }

        public Task<string> GetTracingHost()
        {
            return null;
        }

        public Task<int> GetTracingPort()
        {
            return Task.FromResult(0);
        }
    }
}
