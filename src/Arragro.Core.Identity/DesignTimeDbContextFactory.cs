using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Arragro.Core.Identity
{
    public class ArragroCoreIdentityContextFactory : DesignTimeDbContextFactory<ArragroCoreIdentityContext>
    {
    }

    public class ArragroCoreIdentityPGContextFactory : DesignTimeDbContextFactory<ArragroCoreIdentityPGContext>
    {
    }


    public class DesignTimeDbContextFactory<T> : IDesignTimeDbContextFactory<T>
        where T : DbContext
    {
        public T CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<T>();

            var dbContext = (T)Activator.CreateInstance(
                typeof(T),
                builder.Options);

            return dbContext;
        }
    }
}
