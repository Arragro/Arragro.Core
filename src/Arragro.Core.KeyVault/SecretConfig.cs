using Arragro.Core.KeyVault.Enums;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.KeyVault
{
	public class SecretConfig
	{
		public CertificateStore CertificateStore { get; set; }
		public KeyVaultSecretConfig KeyVaultSecretConfig { get; set; }

		private SecretClient GetSecretClient()
		{
			var sb = new StringBuilder();
			if (KeyVaultSecretConfig.KeyVaultUri == null)
				sb.AppendLine("You must supply a KeyVaultUri.");

			if (sb.Length > 0) throw new ArgumentException($"There are issues with the KeyVaultSecretConfig:\r\n\r\n{sb}");

			SecretClient client;
			if (!string.IsNullOrEmpty(KeyVaultSecretConfig.TenantId) && !string.IsNullOrEmpty(KeyVaultSecretConfig.ClientId) & !string.IsNullOrEmpty(KeyVaultSecretConfig.ClientSecret))
			{
				client = new SecretClient(KeyVaultSecretConfig.KeyVaultUri, new DefaultAzureCredential());
			}
			else
			{
				client = new SecretClient(KeyVaultSecretConfig.KeyVaultUri, new ClientSecretCredential(KeyVaultSecretConfig.TenantId, KeyVaultSecretConfig.ClientId, KeyVaultSecretConfig.ClientSecret));
			}
			return client;
		}

		public async Task<Response<KeyVaultSecret>> GetSecretAsync(string key, string version = null, CancellationToken cancellationToken = default)
		{
			var client = GetSecretClient();
			var value = await client.GetSecretAsync(key, version, cancellationToken);
			return value;
		}

		public Response<KeyVaultSecret> GetSecret(string key)
		{
			var client = GetSecretClient();
			var value = client.GetSecret(key);
			return value;
		}
	}
}