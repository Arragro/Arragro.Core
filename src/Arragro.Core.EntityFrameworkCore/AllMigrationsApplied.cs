using Arragro.Core.Common.Interfaces;
using Arragro.Core.Common.Models;
using Arragro.Core.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Arragro.Core.EntityFrameworkCore
{
    public class AllDbContextMigrationsApplied : IAllDbContextMigrationsApplied
    {
        private readonly IEnumerable<DbContext> _dbContexts;

        public AllDbContextMigrationsApplied(IEnumerable<DbContext> dbContexts)
        {
            _dbContexts = dbContexts;
        }

        public IEnumerable<MigrationTestResult> TestDbContextsMigrated()
        {
            var migrationTestResults = new List<MigrationTestResult>();
            foreach (var dbContext in _dbContexts)
            {
                var type = dbContext.GetType();
                var applied = dbContext.AllMigrationsApplied();
                migrationTestResults.Add(new MigrationTestResult
                {
                    DbContextType = type,
                    Migrated = applied
                });
            }
            return migrationTestResults;
        }
    }
}
