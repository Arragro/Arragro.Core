using Arragro.Core.Identity.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Arragro.Core.Identity.Domains
{
    public class Users<TUser> 
        where TUser :  User
    {
        private readonly UserManager<TUser> _userManager;

        public Users(UserManager<TUser> userManager)
        {
            _userManager = userManager;
        }

        public IEnumerable<TUser> Get()
        {
            return _userManager.Users.ToList();
        }

        public IEnumerable<TUser> GetActiveOnly()
        {
            return _userManager.Users.Where(x => x.IsEnabled).ToList();
        }

        public async Task<TUser> GetAsync(Guid id)
        {
            return await _userManager.FindByIdAsync(id.ToString()).ConfigureAwait(continueOnCapturedContext: true);
        }

        public async Task<IEnumerable<string>> GetRolesAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var claims = await _userManager.GetClaimsAsync(user);
            var roles = claims.Where(x => x.Type == ClaimTypes.Role).ToList();
            return roles.Select(x => x.Value);
        }

        public async Task<TUser> GetAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> AddAsync(TUser user, Guid userId, string password = null)
        {
            user.ModifiedBy = userId;
            user.ModifiedDate = DateTime.UtcNow;
            if (password != null)
                return await _userManager.CreateAsync(user, password);
            return await _userManager.CreateAsync(user);
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, Guid userId)
        {
            user.ModifiedBy = userId;
            user.ModifiedDate = DateTime.UtcNow;
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> DeleteAsync(TUser user)
        {
            var createResult = await _userManager.DeleteAsync(user);

            return createResult;
        }

        protected async Task<IdentityResult> ClearRoleClaimsAsync(TUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);

            return await _userManager.RemoveClaimsAsync(user, claims.Where(x => x.Type == ClaimTypes.Role));
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(TUser user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(TUser user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsyncAsync(TUser user, string code)
        {
            return await _userManager.ConfirmEmailAsync(user, code);
        }

        public async Task<IdentityResult> ResetPasswordAsync(TUser user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<bool> IsEmailConfirmedAsyncAsync(TUser user)
        {
            return await _userManager.IsEmailConfirmedAsync(user);
        }

        public bool ValidateLastChanged(TUser user, string modifiedDate)
        {
            if (long.TryParse(modifiedDate, out long ticks))
            {
                var date = new DateTime(ticks);
                return date == user.ModifiedDate;
            }
            return false;
        }

        public async Task<ClaimsPrincipal> GetClaimsPrincipal(TUser user)
        {
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("User-Identifier", user.Id.ToString()),
                new Claim("ModifiedDate", user.ModifiedDate.Ticks.ToString())
            };

            if (user.IsEnabled)
            {
                var claims = await GetRolesAsync(user.Id);

                userClaims.AddRange(claims.Select(x => new Claim(ClaimTypes.Role, x)).ToList());
            }

            var claimsIdentity = new ClaimsIdentity(
                userClaims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}
