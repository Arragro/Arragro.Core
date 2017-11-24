using Arragro.TestBase;
using Microsoft.EntityFrameworkCore;

namespace Arragro.Core.EntityFrameworkCore.IntegrationTests
{
    public class FooContext : BaseContext
    {
        public FooContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ModelFoo> ModelFoos { get; set; }
        public DbSet<CompositeFoo> CompositeFoo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CompositeFoo>()
                .HasKey(x => new { x.Id, x.SecondId });
        }
    }
}
