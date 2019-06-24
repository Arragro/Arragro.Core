using Arragro.Core.EntityFrameworkCore;

namespace Arragro.Core.Identity
{
    public class ArragroCoreIdentityContextFactory : DesignTimeDbContextFactory<ArragroCoreIdentityContext>
    {
    }

    public class ArragroCoreIdentityPGContextFactory : DesignTimeDbContextFactory<ArragroCoreIdentityPGContext>
    {
    }

    public class ArragroCoreIdentitySqliteContextFactory : DesignTimeDbContextFactory<ArragroCoreIdentitySqliteContext>
    {
    }
}
