namespace Arragro.Core.Web.Auth.Hmac.Models
{
    internal class ValidationResult
    {
        public bool Valid { get; set; }

        /// <summary>
        /// Only valid if <see cref="Valid"/> is true.
        /// </summary>
        public string Username { get; set; }
    }
}
