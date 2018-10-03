using Arragro.Core.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace Arragro.Core.Identity
{
    public class ArragroCoreIdentityBaseContext<TUser, TRole> : IdentityDbContext<TUser, TRole, Guid> 
        where TUser : User
        where TRole : Role
    {
    }
}
