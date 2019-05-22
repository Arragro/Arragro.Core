namespace Arragro.Core.Email.Razor.Models
{
    public class ResetPasswordViewModel
    {
        public ResetPasswordViewModel(
            string resetPasswordUrl)
        {
            ResetPasswordUrl = resetPasswordUrl;
        }

        public string ResetPasswordUrl { get; set; }
    }
}
