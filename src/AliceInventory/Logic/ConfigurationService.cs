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
        public async Task<string> GetMailingAccountLogin()
        {
            try
            {
                Secret<SecretData> secret = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("email_login");
                return secret.Data.Data["CURRENT"] as string;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<string> GetMailingAccountPassword()
        {
            try
            {
                Secret<SecretData> secret = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("email_password");
                return secret.Data.Data["CURRENT"] as string;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<string> GetMailingSmtpHost()
        {
            try
            {
                Secret<SecretData> secret = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("smtp_address");
                return secret.Data.Data["CURRENT"] as string;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<int> GetMailingSmtpPort()
        {
            try
            {
                Secret<SecretData> secret = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("smtp_port");
                var stringPort = secret.Data.Data["CURRENT"] as string;
                return int.Parse(stringPort);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

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
                _VaultClientError = e;
            }
        }
        private IVaultClient _VaultClient;
        private Exception _VaultClientError;

        public async Task<string> GetIsConfigured()
        {
            try
            {
                if (_VaultClient == null)
                    return _VaultClientError.StackTrace;

                Secret<SecretData> smtpAddress = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("smtp_address");
                Secret<SecretData> smtpPort = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("smtp_port");
                Secret<SecretData> emailLogin = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("email_login");
                Secret<SecretData> emailPassword = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("email_password");
                var configValues = new[] { smtpAddress, smtpPort, emailLogin, emailPassword };
                var result = configValues.Any(x => !x.Data.Data.ContainsKey("CURRENT"));
                return result ? "Vault value empty" : string.Empty;
            }
            catch (Exception e)
            {
                return new String(e.StackTrace + "\n" + _VaultClientError?.StackTrace ?? string.Empty);
            }
        }
    }
}
