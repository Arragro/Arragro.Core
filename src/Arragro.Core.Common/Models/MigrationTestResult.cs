using System;

namespace Arragro.Core.Common.Models
{
    public class MigrationTestResult
    {
        public Type DbContextType { get; set; }
        public bool Migrated { get; set; }
    }
}
