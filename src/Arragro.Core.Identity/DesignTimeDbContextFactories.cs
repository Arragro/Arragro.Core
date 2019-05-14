using Arragro.Core.EntityFrameworkCore;
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
}
