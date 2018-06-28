using Arragro.Core.Common.Enums;
using Arragro.Core.Common.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Arragro.Core.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDataProtection(
            this IServiceCollection services,
            BaseSettings baseSettings)
        {
            var dataProtection = services.AddDataProtection();

            if (baseSettings.DataProtectionSettings.DataProtectionStorage == DataProtectionStorage.Redis)
            {
                if (string.IsNullOrEmpty(baseSettings.DataProtectionSettings.RedisConnection))
                    throw new ArgumentNullException("BaseSettings.DataProtectionSettings.RedisConnection", "You need to supply a connection string for redis");

                var redis = ConnectionMultiplexer.Connect(baseSettings.DataProtectionSettings.RedisConnection);

                dataProtection.PersistKeysToRedis(redis, "DataProtection-Keys");
            }

            if (baseSettings.DataProtectionSettings.DataProtectionStorage == DataProtectionStorage.FileSystem &&
                !string.IsNullOrEmpty(baseSettings.DataProtectionSettings.DataProtectionStoragePath))
            {
                dataProtection.PersistKeysToFileSystem(new DirectoryInfo(baseSettings.DataProtectionSettings.DataProtectionStoragePath));
            }

            if (baseSettings.DataProtectionSettings.UseX509 &&
                !string.IsNullOrWhiteSpace(baseSettings.DataProtectionSettings.CertBase64) &&
                !string.IsNullOrWhiteSpace(baseSettings.DataProtectionSettings.Password))
            {
                /*
                 * https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/?view=aspnetcore-2.1
                 * https://github.com/aspnet/DataProtection
                 * https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/compatibility/replacing-machinekey?view=aspnetcore-2.1
                 * http://www.paraesthesia.com/archive/2016/06/15/set-up-asp-net-dataprotection-in-a-farm/
                 * Create crt and key in ubuntu bash:
                 * openssl req -x509 -sha256 -nodes -days 365 -newkey rsa:4096 -keyout privateKey.key -out certificate.crt
                 * 
                 * Create PFX from crt and key
                 * openssl pkcs12 -export -out certificate.pfx -inkey privateKey.key -in certificate.crt
                 * 
                 * Add the cert to the certmgr local machine - you can get it at: C:\Users\{UserName}\AppData\Local\lxss\root
                 * 
                 */

                var bytes = Convert.FromBase64String(baseSettings.DataProtectionSettings.CertBase64);
                X509Certificate2 x509Cert = new X509Certificate2(bytes, baseSettings.DataProtectionSettings.Password);

                dataProtection.ProtectKeysWithCertificate(x509Cert);
            }

            return services;
        }
    }
}
