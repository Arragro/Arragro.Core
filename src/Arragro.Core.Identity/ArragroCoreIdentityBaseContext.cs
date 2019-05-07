using Arragro.Core.EntityFrameworkCore.Extensions;
using Arragro.Core.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.RegularExpressions;

namespace Arragro.Core.Identity
{
    public static class StringExtensions
    {
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }
    }

    public class ArragroCoreIdentityBaseContext : IdentityDbContext<User, Role, Guid>
    {
        public ArragroCoreIdentityBaseContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("identity");

            modelBuilder.SnakeCaseTablesAndProperties();
        }
    }

    public class ArragroCoreIdentityContext : ArragroCoreIdentityBaseContext
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

    public class ArragroCoreIdentityPGContext : ArragroCoreIdentityBaseContext
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
