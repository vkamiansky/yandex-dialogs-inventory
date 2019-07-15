using System;
using System.Linq;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;

namespace AliceInventory.Logic
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IVaultClient _VaultClient;
        private readonly Exception _VaultClientError;

        public ConfigurationService()
        {
            try
            {
                var secretToken = Environment.GetEnvironmentVariable("SECRET_TOKEN");
                var secretIp = Environment.GetEnvironmentVariable("SECRET_IP");
                var secretPort = Environment.GetEnvironmentVariable("SECRET_PORT");

                IAuthMethodInfo authMethod = new TokenAuthMethodInfo(secretToken);
                var vaultClientSettings = new VaultClientSettings($"http://{secretIp}:{secretPort}", authMethod);
                _VaultClient = new VaultClient(vaultClientSettings);
            }
            catch (Exception e)
            {
                _VaultClientError = e;
            }
        }

        public async Task<string> GetMailingAccountLogin()
        {
            try
            {
                var secret = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("email_login");
                return secret.Data.Data["CURRENT"] as string;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetMailingAccountPassword()
        {
            try
            {
                var secret = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("email_password");
                return secret.Data.Data["CURRENT"] as string;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetMailingSmtpHost()
        {
            try
            {
                var secret = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("smtp_address");
                return secret.Data.Data["CURRENT"] as string;
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> GetMailingSmtpPort()
        {
            try
            {
                var secret = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("smtp_port");
                var stringPort = secret.Data.Data["CURRENT"] as string;
                return int.Parse(stringPort);
            }
            catch
            {
                return 0;
            }
        }

        public async Task<string> GetIsConfigured()
        {
            try
            {
                if (_VaultClient == null)
                    return _VaultClientError.StackTrace;

                var smtpAddress = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("smtp_address");
                var smtpPort = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("smtp_port");
                var emailLogin = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("email_login");
                var emailPassword = await _VaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("email_password");
                var configValues = new[] {smtpAddress, smtpPort, emailLogin, emailPassword};
                var result = configValues.Any(x => !x.Data.Data.ContainsKey("CURRENT"));
                return result ? "Vault value empty" : string.Empty;
            }
            catch (Exception e)
            {
                return new string(e.StackTrace + "\n" + _VaultClientError?.StackTrace ?? string.Empty);
            }
        }
    }
}