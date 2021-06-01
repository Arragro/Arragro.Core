namespace Arragro.Core.Web.Auth.Hmac.Models
{
    public struct AuthorizationProviderResult
    {
        public AuthorizationProviderResult(string appId, bool found, string validationKey)
        {
            AppId = appId;
            Found = found;
            ValidationKey = validationKey;
        }

        /// <summary>
        /// The HMAC App Id.
        /// </summary>
        public string AppId { get; }

        /// <summary>
        /// <see langword="True"/>, if the specified HMAC app has been found based on the <see cref="AppId"/>.
        /// </summary>
        public bool Found { get; }

        /// <summary>
        /// The Validation Key for the specified <see cref="AppId"/>.
        /// </summary>
        public string ValidationKey { get; }
    }
}
