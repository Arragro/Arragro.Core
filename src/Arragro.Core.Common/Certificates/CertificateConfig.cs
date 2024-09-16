using Arragro.Core.Common.Enums;
using System;
using System.IO;
using System.Text;

namespace Arragro.Core.Common.Certificates
{
    public class CertificateConfig
    {
        /// <summary>
        /// CertificateStore to use, FilePath or KeyVault
        /// </summary>
        public CertificateStore CertificateStore { get; set; } = CertificateStore.FilePath;
        /// <summary>
        /// File Path Certificate Config for Path certificate
        /// </summary>
        public FilePathCertificateConfig FilePathCertificateConfig { get; set; } = null;
        /// <summary>
        /// KeyVault Certificate Config for KeyVault
        /// </summary>
        public KeyVaultCertificateConfig KeyVaultCertificateConfig { get; set; } = null;

        public void Validate()
        {
            var stringBuilder = new StringBuilder();
            if (CertificateStore == CertificateStore.FilePath)
            {
                if (FilePathCertificateConfig == null)
                    stringBuilder.AppendLine("You must supply a FilePathCertificateConfig.");
                else
                {
                    if (FilePathCertificateConfig.PfxCertificatePath == null)
                        stringBuilder.AppendLine("The CertificatePath needs to be configured.");
                    if (FilePathCertificateConfig.PfxCertificatePassword == null)
                        stringBuilder.AppendLine("The Password needs to be configured.");
                }
            }
            else if (CertificateStore == CertificateStore.KeyVault)
            {
                if (KeyVaultCertificateConfig == null)
                    stringBuilder.AppendLine("You must supply a KeyVaultCertificateConfig.");
                else
                {
                    if (KeyVaultCertificateConfig.KeyVaultUri == null)
                        stringBuilder.AppendLine("The KeyVaultUri needs to be configured.");
                    if (KeyVaultCertificateConfig.CertificateName == null)
                        stringBuilder.AppendLine("The CertificateName needs to be configured.");
                }
            }

            if (CertificateStore == CertificateStore.FilePath &&
                !File.Exists(FilePathCertificateConfig.PfxCertificatePath))
            {
                throw new Exception($"Cannot find cert file for JWTSettings. Current Path is: {Directory.GetCurrentDirectory()}");
            }

            if (CertificateStore == CertificateStore.KeyVault &&
                string.IsNullOrEmpty(KeyVaultCertificateConfig.CertificateName))
            {
                throw new Exception("If Certificate Config is for KeyVaule, you must supply a CertificateName");
            }

            if (stringBuilder.Length > 0)
                throw new Exception($"JwtSettings in appsettings.json has issues:\r\n\r\n{stringBuilder}");
        }
    }
}
