using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Certificates
{
	public class KeyVaultCertificateConfig
	{
		public Uri KeyVaultUri { get; set; }
		public string TenantId { get; set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }

		//public SecretClient GetSecretClient()
		//{
		//	var sb = new StringBuilder();
		//	if (KeyVaultUri == null)
		//		sb.AppendLine("You must supply a KeyVaultUri.");
		//	if (string.IsNullOrEmpty(CertificateName))
		//		sb.AppendLine("You must supply a certificate name.");

		//	if (!string.IsNullOrEmpty(TenantId) && !string.IsNullOrEmpty(ClientId) & !string.IsNullOrEmpty(ClientSecret))
		//	{
		//		return new SecretClient(KeyVaultUri, new DefaultAzureCredential());
		//	}
		//	else
		//	{
		//		return new SecretClient(KeyVaultUri, new ClientSecretCredential(TenantId, ClientId, ClientSecret));
		//	}
		//}

		private CertificateClient GetCertificateClient()
		{
			var sb = new StringBuilder();
			if (KeyVaultUri == null)
				sb.AppendLine("You must supply a KeyVaultUri.");

			if (sb.Length > 0) throw new ArgumentException($"There are issues with the KeyVaultCertificateConfig:\r\n\r\n{sb}");

			CertificateClient client;
			if (string.IsNullOrEmpty(TenantId) && string.IsNullOrEmpty(ClientId) & string.IsNullOrEmpty(ClientSecret))
			{
				client = new CertificateClient(KeyVaultUri, new DefaultAzureCredential());
			}
			else
			{
				client = new CertificateClient(KeyVaultUri, new ClientSecretCredential(TenantId, ClientId, ClientSecret));
			}
			return client;
		}

		internal async Task<Response<X509Certificate2>> GetCertificateAsync(string certificateName, string version = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(certificateName))
				throw new ArgumentNullException(nameof(certificateName), "You must supply a certificate name.");

			var client = GetCertificateClient();
			var certificate = await client.DownloadCertificateAsync(certificateName, version, cancellationToken);
			return certificate;
		}

		internal Response<X509Certificate2> GetCertificate(string certificateName, string version = null)
		{
			if (string.IsNullOrEmpty(certificateName))
				throw new ArgumentNullException(nameof(certificateName), "You must supply a certificate name.");

			var client = GetCertificateClient();
			var certificate = client.DownloadCertificate(certificateName, version);
			return certificate;
		}
	}
}