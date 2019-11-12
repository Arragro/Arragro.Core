using Arragro.Core.Common.Models;
using Arragro.Core.DistributedCache;
using Arragro.Core.Docker;
using Arragro.Core.MailhogClient;
using Arragro.Providers.MailKitEmailProvider;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace Arragro.Providers.ImageServiceProvider.IntegrationTests
{
    [Serializable]
    [DataContract]
    public class MyClass
    {
        [DataMember(Order = 1)]
        public DateTimeOffset DateTimeOffset { get; set; }
    }

    public class MailKitEmailProviderTests : IDisposable
    {
        public MailKitEmailProviderTests()
        {
            DockerExtentions.StartDockerServicesAsync(new List<Func<DockerClient, Task<ContainerListResponse>>>
            {
                Mailhog.StartMailhog,
                LocalStripe.StartLocalStripe,
                Postgres.StartPostgres
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

            var assembly = typeof(ImageServiceTests).GetTypeInfo().Assembly;
            var bytes = ImageServiceTests.ReadFully(assembly.GetManifestResourceStream("Arragro.Providers.ImageServiceProvider.IntegrationTests.Resources.bear-hands.jpg"));

            var emailMessage = new EmailMessage("test", "test", "<h3>test</h3>", new EmailAddress("support@arragro.com", "tester mctest"));
            emailMessage.Headers.Add("test", "123");
            emailMessage.Ccs.Add(new EmailAddress("test@test.com"));
            emailMessage.Bccs.Add(new EmailAddress("test@test.com"));
            emailMessage.Attachments.Add("bear-hands.jpg", new EmailAttachment(bytes, "image/jpeg"));

            await emailProvider.SendEmailAsync(emailMessage);

            var httpClient = new HttpClient();
            var mailhogClient = new MailhogClient(httpClient);
            var messages = await mailhogClient.GetMessagesAsync();
            foreach (var item in messages)
            {
                var message = await mailhogClient.GetMessageAsync(item.Id);
                var consumable = message.ToConsumableMessage();
                var result = await mailhogClient.DeleteMessageAsync(item.Id);
            }
        }

        [Fact]
        public void test_protobuf_date_time_offset_serialization()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDistributedMemoryCache();

            serviceCollection.AddTransient<DistributedCacheManager>();
            serviceCollection.AddSingleton(new DistributedCacheEntryOptions { SlidingExpiration = new TimeSpan(0, 5, 0) });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var distributedCacheManager = serviceProvider.GetRequiredService<DistributedCacheManager>();

            DateTimeOffsetSurrogate.Configure();

            var myClass = new MyClass { DateTimeOffset = DateTimeOffset.UtcNow };
            distributedCacheManager.Set("myClass", myClass);
            myClass = distributedCacheManager.Get<MyClass>("myClass");

            DateTimeOffsetSurrogate.Configure();

            distributedCacheManager.Set("myClass", myClass);
            myClass = distributedCacheManager.Get<MyClass>("myClass");
        }
    }
}
