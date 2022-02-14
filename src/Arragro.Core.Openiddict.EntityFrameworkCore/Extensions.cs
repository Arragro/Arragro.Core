using Arragro.Core.Common.Enums;
using Arragro.Core.EntityFrameworkCore.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Arragro.Core.Openiddict.EntityFrameworkCore
{
	public static class Extensions
	{
        private const string SqliteInMemoryConnectionString = "DataSource=:memory:";

        public static void ConfigureOpenIddictDatabase(this IServiceCollection services, DatabaseType databaseType, string contentRootPath, string connectionString)
		{

            switch (databaseType)
            {
                case DatabaseType.Postgres:
                    services.AddDbContext<OpenIddictPGContext>(
                        options => options.UseNpgsql(connectionString, sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(3);
                            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        })
                    );
                    services.AddScoped<OpenIddictBaseContext, OpenIddictPGContext>();
                    break;
                case DatabaseType.SqlServer:
                    services.AddDbContext<OpenIddictContext>(
                        options => options.UseSqlServer(connectionString, sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(3);
                            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        })
                    );
                    services.AddScoped<OpenIddictBaseContext, OpenIddictContext>();
                    break;
                case DatabaseType.Sqlite:
                    var sqliteCconnectionString = connectionString;
                    if (sqliteCconnectionString != SqliteInMemoryConnectionString)
                    {
                        var sqliteConnectionStringBuilder = new SqliteConnectionStringBuilder(sqliteCconnectionString);
                        sqliteConnectionStringBuilder.DataSource = Path.GetFullPath(Path.Combine(contentRootPath, sqliteConnectionStringBuilder.DataSource));
                        Directory.CreateDirectory(Path.GetDirectoryName(sqliteConnectionStringBuilder.DataSource));
                        sqliteCconnectionString = sqliteConnectionStringBuilder.ToString();
                    }

                    services.AddDbContext<OpenIddictSqliteContext>(options =>
                    {
                        options.UseSqlite(sqliteCconnectionString, sqlOptions =>
                        {
                            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        });
                    });

                    services.AddScoped<OpenIddictBaseContext, OpenIddictSqliteContext>();
                    break;
                default:
                    services.AddDbContext<OpenIddictContext>(options => options.UseInMemoryDatabase("InMemory"));
                    break;
            }
        }

        public static void MigrateOpenIddictDatabase(this IServiceProvider services, bool migrateDatabaseOnStartup)
        {
            using (var scope = services.CreateScope())
            {
                var openIddictContext = scope.ServiceProvider.GetRequiredService<OpenIddictBaseContext>();

                if (!openIddictContext.Exists() || (!openIddictContext.AllMigrationsApplied()))
                    openIddictContext.Database.Migrate();
            }
        }
    }
}
