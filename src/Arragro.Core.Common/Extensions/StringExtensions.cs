using Arragro.Core.Common.Enums;
using System.Text.RegularExpressions;

namespace Arragro.Core.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }
        
        public static string ProcessSqlString(this string sql, DatabaseType databaseType)
        {
            if (databaseType == DatabaseType.Sqlite)
                return sql.Replace(" cms.", " ")
                            .Replace(" dynamic.", " ")
                            .Replace(" identity.", " ")
                            .Replace("STRING_AGG", "group_concat")
                            .Replace("LEN(", "length(");

            if (databaseType == DatabaseType.Postgres)
                return sql.Replace("LEN(", "length(");

            return sql;
        }
    }
}
