using Arragro.Core.Common.Certificates;
using Arragro.Core.Common.Enums;
using Arragro.Core.Common.Interfaces;
using System;

namespace Arragro.Core.Common.Models
{
    public class WebInfoSettings
    {
        public bool IsWebInfoEnabled { get; set; } = false;
        public Guid Secret { get; set; } = Guid.Empty;
    }

    /// <summary>
    /// DataProtectionSettings is used to configure DataProtection.  Useful for multiple instances
    /// </summary>
    public class DataProtectionSettings
    {
        /// <summary>
        /// Determines if DataProtection is enabled
        /// </summary>
        public bool UseDataProtection { get; set; } = false;
        /// <summary>
        /// The application name used by dataprotection
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// CertificateStore to use, FilePath or KeyVault
        /// </summary>
        public CertificateStore CertificateStore { get; set; } = CertificateStore.None;
        /// <summary>
        /// File Path Certificate Config for Path certificate
        /// </summary>
        public FilePathCertificateConfig FilePathCertificateConfig { get; set; } = null;
        /// <summary>
        /// KeyVault Certificate Config for KeyVault
        /// </summary>
        public KeyVaultCertificateConfig KeyVaultCertificateConfig { get; set; } = null;
        /// <summary>
        /// Storage provider
        /// </summary>
        public DataProtectionStorage DataProtectionStorage { get; set; } = DataProtectionStorage.FileSystem;
        /// <summary>
        /// Connection string to redis
        /// </summary>
        public string RedisConnection { get; set; } = null;
        /// <summary>
        /// The key Redis uses for dataprotection
        /// </summary>
        public string RedisKey { get; set; } = "DataProtection-Keys";
        /// <summary>
        /// Path to the file storeage is FileSystem is set on DataProtectionStorage
        /// </summary>
        public string DataProtectionStoragePath { get; set; } = null;
    }

    public class BaseSettings
    {
        public WebInfoSettings WebInfoSettings { get; set; } = new WebInfoSettings();
        public DataProtectionSettings DataProtectionSettings { get; set; } = new DataProtectionSettings();
        public IAllDbContextMigrationsApplied AllDbContextMigrationsApplied { get; set; } = null;
    }
}
