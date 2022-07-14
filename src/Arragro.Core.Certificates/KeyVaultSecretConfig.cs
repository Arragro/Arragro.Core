using System;

namespace Arragro.Core.Certificates
{
	internal class KeyVaultSecretConfig
	{
		public Uri KeyVaultUri { get; set; }
		public string TenantId { get; set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
	}
}