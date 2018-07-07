using Arragro.Core.Common.Models;
using System.Collections.Generic;

namespace Arragro.Core.Common.Interfaces
{
    public interface IAllDbContextMigrationsApplied
    {
        IEnumerable<MigrationTestResult> TestDbContextsMigrated();
    }
}
