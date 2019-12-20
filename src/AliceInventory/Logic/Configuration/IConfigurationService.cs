using System.Threading.Tasks;

namespace AliceInventory.Logic
{
    public interface IConfigurationService
    {
        Task<string> GetMailingAccountLogin();
        Task<string> GetMailingAccountPassword();
        Task<string> GetMailingSmtpHost();
        Task<int> GetMailingSmtpPort();
        Task<string> GetDbConnectionString();
        Task<string> GetDbName();
        Task<string> GetIsConfigured();
        Task<string> GetTracingHost();
        Task<int> GetTracingPort();
    }
}
