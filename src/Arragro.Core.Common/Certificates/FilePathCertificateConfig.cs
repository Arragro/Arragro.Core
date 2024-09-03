using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Arragro.Core.Common.Certificates
{
	public class FilePathCertificateConfig
	{
		public string PfxCertificatePath { get; set; }
		public string PfxCertificatePassword { get; set; }

		private void Validate()
		{
			if (String.IsNullOrEmpty(PfxCertificatePath))
				throw new ArgumentException("The CertificateStore is set to FilePath but has no PfxCertificatePath.");
			if (!File.Exists(PfxCertificatePath))
				throw new ArgumentException("The CertificateStore is set to FilePath but the PfxCertificatePath supplied doesn't exist.");
		}

		public X509Certificate2 GetX509Certificate()
		{
			Validate();

			return new X509Certificate2(File.ReadAllBytes(PfxCertificatePath), PfxCertificatePassword);
		}
	}
}