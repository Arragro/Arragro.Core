using Arragro.Core.Common.Models;
using Arragro.Core.Docker;
using Arragro.Core.MailhogClient;
using Arragro.Providers.MailKitEmailProvider;
using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Arragro.Providers.ImageServiceProvider.IntegrationTests
{
    public class MailKitEmailProviderTests : IDisposable
    {
        public MailKitEmailProviderTests()
        {
            DockerExtentions.StartDockerServicesAsync(new List<Func<DockerClient, Task<ContainerListResponse>>>
            {
                Mailhog.StartMailhog
            }).Wait();
        }

        public void Dispose()
        {
            DockerExtentions.RemoveDockerServicesAsync().Wait();
        }

        [Fact]
        public async Task test_email_sends()
        {
            var smtpSettings = new EmailSettings();
            var emailProvider = new EmailProvider(smtpSettings);

            var emailMessage = new EmailMessage("test", "test", "<h3>test</h3>", new EmailAddress("support@arragro.com", "tester mctest"));
            emailMessage.Headers.Add("test", "123");
            emailMessage.Ccs.Add(new EmailAddress("test@test.com"));
            emailMessage.Bccs.Add(new EmailAddress("test@test.com"));

            await emailProvider.SendEmailAsync(emailMessage);

            var httpClient = new HttpClient();
            var mailhogClient = new MailhogClient(httpClient);
            var messages = await mailhogClient.GetMessagesAsync();
            foreach (var item in messages.Items)
            {
                var message = await mailhogClient.GetMessageAsync(item.Id);
                var consumable = message.ToConsumableMessage();
                var result = await mailhogClient.DeleteMessageAsync(item.Id);
            }
        }
    }
}
