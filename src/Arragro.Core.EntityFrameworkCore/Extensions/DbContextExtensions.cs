using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Arragro.Core.EntityFrameworkCore.Extensions
{
    public static class DbContextExtensions
    {
        public static bool AllMigrationsApplied(this DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }

        public static bool Exists(this DbContext context)
        {
            return (context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();
        }

        public static DbTransaction GetDbTransaction(this DbContext context)
        {
            if (context.Database.CurrentTransaction == null)
                return null;
            return context.Database.CurrentTransaction.GetDbTransaction();
        }

        public static async Task<TResult> ExecuteDbCommandCommandAsync<TResult>(this DbContext context, Func<IDbConnection, Task<TResult>> func)
        {
            var conn = context.Database.GetDbConnection();
            var connClose = false;
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
                connClose = true;
            }

            var result = await func(conn);

            if (connClose)
                conn.Close();

            return result;
        }

        public static TResult ExecuteDbCommandCommand<TResult>(this DbContext context, Func<IDbConnection, TResult> func)
        {
            var conn = context.Database.GetDbConnection();
            var connClose = false;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
                connClose = true;
            }

            var result = func(conn);

            if (connClose)
                conn.Close();

            return result;
        }
    }
}
