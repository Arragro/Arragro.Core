using Arragro.Core.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Arragro.Core.Common.CacheProvider;
using System.Text;
using System.Data.SqlClient;

namespace Arragro.Core.Common
{
    public class ApplicationProperties
    {
        private static ILog _log = LogManager.GetLogger(typeof(ApplicationProperties));
        public string ApplicationPath { get; set; }
        private IDictionary<string, bool> _dbsAttached { get; set; }
        private static ApplicationProperties _applicationProperties;

        private static TimeSpan[] Retries = new[]
        {
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(300),
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromMilliseconds(800),
            TimeSpan.FromMilliseconds(1000),
            TimeSpan.FromMilliseconds(3000)
        };

        public ApplicationProperties(IConfigurationRoot configuration)
        {
            _dbsAttached = new Dictionary<string, bool>();

            var connectionStrings = configuration.GetChildren()
                                        .Single(x => x.Key == "connectionStrings")
                                        .GetChildren();

            foreach (var connectionString in connectionStrings)
            {
                _dbsAttached.Add(connectionString.Value, true);
            }

            _applicationProperties = this;
        }

        private static void ResetApplicationProperties(ApplicationProperties applicationProperties, bool hasDatabaseAccess)
        {
            foreach (var keyPairValue in applicationProperties._dbsAttached)
            {
                applicationProperties._dbsAttached[keyPairValue.Key] = hasDatabaseAccess;
                _log.Info(string.Format("ResetApplicationProperties reset {0} to {1}", keyPairValue.Key, hasDatabaseAccess.ToString()));
            }
            _applicationProperties = applicationProperties;
        }

        private static bool TestSqlServerConnectivity(string connectionString)
        {
            var newConnectionString = Cache.Get(connectionString, () =>
            {
                var splitConnections = connectionString.Split(';');
                var shortConnection = new StringBuilder();
                foreach (var splitConnection in splitConnections)
                {
                    if (!string.IsNullOrEmpty(splitConnection) && !splitConnection.Contains("Connection Timeout"))
                        shortConnection.Append(splitConnection).Append(";");
                    else if (!string.IsNullOrEmpty(splitConnection) && splitConnection.Contains("Connection Timeout"))
                        shortConnection.Append("Connection Timeout=5");
                }
                return shortConnection.ToString();
            });

            using (var conn = new SqlConnection(newConnectionString))
            {
                try
                {
                    conn.Open();
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error("TestSqlServerConnectivity threw an exception", ex);
                }
                return false;
            }
        }

        public static bool TestSqlServerConnection()
        {
            foreach (var dbAttached in _applicationProperties._dbsAttached)
            {
                ResetApplicationProperties(_applicationProperties, TestSqlServerConnectivity(dbAttached.Key));
            }
            return !_applicationProperties._dbsAttached.Any(x => !x.Value);
        }
    }
}
