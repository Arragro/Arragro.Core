using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Arragro.Core.Openiddict.EntityFrameworkCore
{
    public class OpenIddictBaseContext : DbContext
    {
        public OpenIddictBaseContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("openiddict");

            builder.UseOpenIddict();

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }

    public class OpenIddictBaseContext<T> : OpenIddictBaseContext where T : DbContext
    {

        public OpenIddictBaseContext() : base(new DbContextOptions<T>())
        {
        }

        public OpenIddictBaseContext(DbContextOptions<T> options)
            : base(options)
        {
        }
    }

    public class OpenIddictContext : OpenIddictBaseContext<OpenIddictContext>
    {
        public OpenIddictContext()
        {
        }

        public OpenIddictContext(DbContextOptions<OpenIddictContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=ArragroCMS;Trusted_Connection=True;");
            }
        }
    }

    public class OpenIddictPGContext : OpenIddictBaseContext<OpenIddictPGContext>
    {
        public OpenIddictPGContext()
        {
        }

        public OpenIddictPGContext(DbContextOptions<OpenIddictPGContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(@"host=192.168.69.254;port=5469;database=arragro-cms-examples;user id=postgres;password=password1;");
            }
        }
    }

    public class OpenIddictSqliteContext : OpenIddictBaseContext<OpenIddictSqliteContext>
    {
        public OpenIddictSqliteContext()
        {
        }

        public OpenIddictSqliteContext(DbContextOptions<OpenIddictSqliteContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(@"DataSource=:memory:");
            }
        }
    }
}
