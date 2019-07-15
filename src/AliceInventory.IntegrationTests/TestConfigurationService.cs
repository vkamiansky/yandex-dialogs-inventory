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

        public Task<int> GetMailingSmtpPort()
        {
            return Task.FromResult(0);
        }

        public Task<string> GetIsConfigured()
        {
            return Task.FromResult("Vault value empty");
        }
    }
}