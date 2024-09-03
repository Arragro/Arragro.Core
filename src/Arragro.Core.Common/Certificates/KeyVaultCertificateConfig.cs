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
        /// <summary>
        /// The uri of the KeyVault service
        /// </summary>
		public Uri KeyVaultUri { get; set; }
        /// <summary>
        /// KeyVault certificate name
        /// </summary>
        public string CertificateName { get; set; }
        public string TenantId { get; set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }

        private void Validate()
        {
            if (KeyVaultUri == null)
                throw new ArgumentException("The KeyVaultUri must be supplied.");
			if (!string.IsNullOrEmpty(CertificateName))
                throw new ArgumentException("The KeyVaultCertificateName must be supplied.");
        }

        private CertificateClient GetCertificateClient()
		{
			var sb = new StringBuilder();
			if (KeyVaultUri == null)
				sb.AppendLine("You must supply a KeyVaultUri.");

			if (sb.Length > 0) throw new ArgumentException($"There are issues with the KeyVaultCertificateConfig:\r\n\r\n{sb}");

			CertificateClient client;
			if (string.IsNullOrEmpty(TenantId) || string.IsNullOrEmpty(ClientId) || string.IsNullOrEmpty(ClientSecret))
			{
				client = new CertificateClient(KeyVaultUri, new DefaultAzureCredential());
			}
			else
			{
				client = new CertificateClient(KeyVaultUri, new ClientSecretCredential(TenantId, ClientId, ClientSecret));
			}
			return client;
		}

        public async Task<Response<X509Certificate2>> GetX509CertificateAsync(string version = null, CancellationToken cancellationToken = default)
        {
            Validate();

			var client = GetCertificateClient();
			var certificate = await client.DownloadCertificateAsync(CertificateName, version, cancellationToken);
			return certificate;
		}

		public Response<X509Certificate2> GetX509Certificate(string version = null)
        {
            Validate();

			var client = GetCertificateClient();
			var certificate = client.DownloadCertificate(CertificateName, version);
			return certificate;
		}
	}
}