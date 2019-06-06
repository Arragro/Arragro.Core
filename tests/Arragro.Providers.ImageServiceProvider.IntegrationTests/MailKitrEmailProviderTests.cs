using Arragro.Core.Common.Models;
using Arragro.Providers.MailKitEmailProvider;
using System.Threading.Tasks;
using Xunit;

namespace Arragro.Providers.ImageServiceProvider.IntegrationTests
{
    public class MailKitrEmailProviderTests
    {
        [Fact]
        public async Task test_email_sends()
        {
            var smtpSettings = new EmailSettings();
            var emailProvider = new EmailProvider(smtpSettings);

            var emailMessage = new EmailMessage("test", "test", "<h3>test</h3>", new EmailAddress("support@arragro.com"));
            emailMessage.Headers.Add("test", "123");

            await emailProvider.SendEmailAsync(emailMessage);
        }
    }
}
