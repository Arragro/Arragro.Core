using Arragro.Core.EntityFrameworkCore;

namespace Arragro.Core.Openiddict.EntityFrameworkCore
{
    public class OpenIddictContextFactory : DesignTimeDbContextFactory<OpenIddictContext>
    {
    }

    public class OpenIddictPGContextFactory : DesignTimeDbContextFactory<OpenIddictPGContext>
    {
    }

    public class OpenIddictSqliteContextFactory : DesignTimeDbContextFactory<OpenIddictSqliteContext>
    {
    }
}
