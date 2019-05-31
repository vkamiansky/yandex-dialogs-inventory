using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.SecretsEngines;
using VaultSharp.V1.Commons;

namespace AliceInventory.Logic
{
    public class ConfigurationService : IConfigurationService
    {
        public string MailingAccountLogin { get; }
        public string MailingAccountPassword { get; }
        public string MailingSmtpHost { get; }
        public string MailingSmtpPort { get; }

        public ConfigurationService()
        {
            try
            {
                string secretToken = Environment.GetEnvironmentVariable("SECRET_TOKEN");
                string secretIp = Environment.GetEnvironmentVariable("SECRET_IP");
                string secretPort = Environment.GetEnvironmentVariable("SECRET_PORT");

                IAuthMethodInfo authMethod = new TokenAuthMethodInfo(secretToken);
                var vaultClientSettings = new VaultClientSettings($"http://{secretIp}:{secretPort}", authMethod);
                _VaultClient = new VaultClient(vaultClientSettings);
            }
            catch (Exception e)
            {

            }
        }
        private IVaultClient _VaultClient;

        public async Task<bool> GetIsConfigured()
        {
            if (_VaultClient == null)
                return false;
            try
            {
                Secret<SecretData> smtpAddress = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("smtp_address");
                Secret<SecretData> smtpPort = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("smtp_port");
                Secret<SecretData> emailLogin = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("email_login");
                Secret<SecretData> emailPassword = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("email_password");
                
                var configValues = new[] { smtpAddress, smtpPort, emailLogin, emailPassword };
                var result = configValues.Any(x => !x.Data.Data.ContainsKey("CURRENT"));
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
