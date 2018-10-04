using Arragro.Core.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Arragro.Core.Identity
{
    public class ArragroCoreIdentityBaseContext : IdentityDbContext<User, Role, Guid>
    {
        public ArragroCoreIdentityBaseContext(DbContextOptions options) : base(options) { }
    }

    public class ArragroCoreIdentityContext : ArragroCoreIdentityBaseContext<User, Role>
    {
        public ArragroCoreIdentityContext(DbContextOptions<ArragroCoreIdentityContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=ArragroCMS;Trusted_Connection=True;");
            }
        }
    }

    public class ArragroCoreIdentityPGContext : ArragroCoreIdentityBaseContext<User, Role>
    {
        public ArragroCoreIdentityPGContext(DbContextOptions<ArragroCoreIdentityPGContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(@"host=localhost;port=5432;database=arragro-cms;user id=postgres;password=Password1");
            }
        }

    }
}
