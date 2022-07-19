using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Arragro.Core.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Arragro.Core.Identity.Interfaces
{
    public interface IUsers<TUser> where TUser : User
    {
        Task<IdentityResult> AddAsync(TUser user, Guid userId, string password = null);
        Task<IdentityResult> AddLogin(TUser user, UserLoginInfo userLoginInfo);
        Task<IdentityResult> ConfirmEmailAsync(TUser user, string code);
        Task<IdentityResult> DeleteAsync(TUser user);
        Task<string> GenerateEmailConfirmationTokenAsync(TUser user);
        Task<string> GeneratePasswordResetTokenAsync(TUser user);
        IEnumerable<TUser> Get();
        IEnumerable<TUser> GetActiveOnly();
        Task<TUser> GetAsync(Guid id);
        Task<TUser> GetAsync(string email);
        Task<ClaimsPrincipal> GetClaimsPrincipal(TUser user);
        Task<IEnumerable<string>> GetRolesAsync(Guid id);
        Task<bool> IsEmailConfirmedAsyncAsync(TUser user);
        Task<IdentityResult> ResetPasswordAsync(TUser user, string token, string password);
        Task<IdentityResult> UpdateAsync(TUser user, Guid userId);
        bool ValidateLastChanged(TUser user, string modifiedDate);
    }
}