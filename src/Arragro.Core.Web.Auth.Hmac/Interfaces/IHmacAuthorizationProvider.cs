using Arragro.Core.Web.Auth.Hmac.Models;
using System.Threading.Tasks;

namespace Arragro.Core.Web.Auth.Hmac.Interfaces
{
    public interface IHmacAuthorizationProvider
    {
        Task<AuthorizationProviderResult> TryGetApiKeyAsync(string appId);
    }
}
