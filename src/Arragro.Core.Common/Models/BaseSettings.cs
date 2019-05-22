using Arragro.Core.Common.Enums;
using Arragro.Core.Common.Interfaces;
using System;

namespace Arragro.Core.Common.Models
{
    public class EmailConfiguration
    {
        public string AppLogoUrl { get; set; } = "https://arragro-static-web.azureedge.net/public/images/email/ArragroEmailLogo.png";
        public string AppName { get; set; } = "Arragro";
        public string AppUrl { get; set; } = "https://www.arragro.com";
    }

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
        /// Determines if X509 cert is to be used, required for multi instance apps, replaces machine key
        /// </summary>
        public bool UseX509 { get; set; } = false;
        /// <summary>
        /// Certificate path (cert should be pfx with password)
        /// </summary>
        public string CertificatePath { get; set; } = null;
        /// <summary>
        /// Password to the CertBase64 cert
        /// </summary>
        public string Password { get; set; } = null;
        /// <summary>
        /// Storage provider
        /// </summary>
        public DataProtectionStorage DataProtectionStorage { get; set; } = DataProtectionStorage.FileSystem;
        /// <summary>
        /// Connection string to redis
        /// </summary>
        public string RedisConnection { get; set; } = null;
        /// <summary>
        /// Path to the file storeage is FileSystem is set on DataProtectionStorage
        /// </summary>
        public string DataProtectionStoragePath { get; set; } = null;
    }

    public class BaseSettings
    {
        public EmailConfiguration EmailConfiguration { get; set; } = new EmailConfiguration();
        public WebInfoSettings WebInfoSettings { get; set; } = new WebInfoSettings();
        public DataProtectionSettings DataProtectionSettings { get; set; } = new DataProtectionSettings();
        public IAllDbContextMigrationsApplied AllDbContextMigrationsApplied { get; set; } = null;
    }
}
