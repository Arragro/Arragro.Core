namespace Arragro.Core.Common.Interfaces.Providers
{
    public interface IEmailProvider
    {
        void SendEmail(string subject, string text, string html, string from, string to);
        void SendEmail(string subject, string text, string html, string to);
    }
}
