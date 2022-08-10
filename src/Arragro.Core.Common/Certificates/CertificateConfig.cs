using Arragro.Core.Common.Enums;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Arragro.Core.Common.Certificates
{
	public class CertificateConfig
	{
		public CertificateStore CertificateStore { get; set; }
		public string PfxCertificatePath { get; set; }
		public string PfxCertificatePassword { get; set; }
		public KeyVaultCertificateConfig KeyVaultCertificateConfig { get; set; }

		private void Validate()
		{
			if (CertificateStore == CertificateStore.FilePath)
			{
				if (String.IsNullOrEmpty(PfxCertificatePath))
					throw new ArgumentException("The CertificateStore is set to FilePath but has no PfxCertificatePath.");
				if (!File.Exists(PfxCertificatePath))
					throw new ArgumentException("The CertificateStore is set to FilePath but the PfxCertificatePath supplied doesn't exist.");
			}
			else if (CertificateStore == CertificateStore.KeyVault)
			{
				if (KeyVaultCertificateConfig == null)
					throw new ArgumentException("The CertificateStore is set to KeyVault but has no KeyVaultCertificateConfig.");
			}
		}

		public X509Certificate2 GetX509Certificate(string certificateName = null)
		{
			Validate();
			if (CertificateStore == CertificateStore.KeyVault)
			{
				var cert = KeyVaultCertificateConfig.GetCertificate(certificateName);
				if (cert.GetRawResponse().Status != (int)HttpStatusCode.OK)
					throw new ArgumentException("KeyVault didn't return a 200.");
				return cert.Value;
			}
			else
			{
				return new X509Certificate2(File.ReadAllBytes(PfxCertificatePath), PfxCertificatePassword);
			}
		}
	}
}