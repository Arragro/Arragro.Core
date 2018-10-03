
using Arragro.Core.DistributedCache;
using Arragro.Core.Identity.Domains;
using Arragro.Core.Identity.Models;
using AutoMapper;
using System;

namespace Arragro.Core.Identity.Resolvers
{
    public class UserNameResolver : IMemberValueResolver<object, object, Guid, string>
    {
        private readonly Users<User> _users;
        private readonly DistributedCacheManager _distributedCacheManager;

        public UserNameResolver(
            Users<User> users,
            DistributedCacheManager distributedCacheManager)
        {
            _users = users;
            _distributedCacheManager = distributedCacheManager;
        }

        public string Resolve(object source, object destination, Guid sourceMember, string destMember, ResolutionContext context)
        {
            return _distributedCacheManager.Get($"UserNameResolver:{sourceMember}", () =>
            {
                var user = _users.GetAsync(sourceMember).Result;
                if (user == null)
                    return "Something is wrong with this users data";
                return user.UserName;
            });
        }
    }
}
