namespace Arragro.Core.Email.Razor.Models
{
    public class ConfirmAccountEmailViewModel
    {
        public ConfirmAccountEmailViewModel(
            bool usesExternalProviders,
            string confirmEmailUrl)
        {
            UsesExternalProviders = usesExternalProviders;
            ConfirmEmailUrl = confirmEmailUrl;
        }

        public bool UsesExternalProviders { get; set; }
        public string ConfirmEmailUrl { get; set; }
    }
}
