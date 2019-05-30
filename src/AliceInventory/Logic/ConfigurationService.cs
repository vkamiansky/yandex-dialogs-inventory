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
            var vars = Environment.GetEnvironmentVariables();
            KeyValuePair<string, string> vaultProperties = new KeyValuePair<string, string>("","");
            foreach (var key in vars.Keys.Cast<string>())
            {
               vaultProperties = KeyValuePair.Create(key, Environment.GetEnvironmentVariable(key));
            }

            this.ConfigureVault("secret", "http://172.18.0.2:8200", "mail");
            if(vaultProperties.Key == "VAULT_VAR" && vaultProperties.Value == "VAULT_CONF")
            {
                Configured = true;
            }
        } 

        public bool IsApplicationConfigured 
        { 
            get
            {
                return this.Configured;  
            }
            private set
            {
                this.Configured = value;
            }
        }
        private bool Configured = false;

        private async void ConfigureVault(string token, string ipAddrPort, string path)
        {
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(token);
            var vaultClientSettings = new VaultClientSettings(ipAddrPort, authMethod);
            IVaultClient vaultClient = new VaultClient(vaultClientSettings);
            Secret<SecretData> kv2Secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path);
        }
    }
}
