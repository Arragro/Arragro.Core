using Arragro.Core.Common.Interfaces.Providers;
using Arragro.Core.Docker;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
namespace Arragro.Providers.ImageServiceProvider.IntegrationTests
{
    public class ImageServiceTests : IDisposable
    {
        // const string _server = "http://192.168.69.89:3000";
        const string _server = "http://localhost:3000";
        private readonly IServiceProvider _serviceProvider;

        public ImageServiceTests()
        {
            DockerExtentions.StartDockerServicesAsync(new List<Func<DockerClient, Task<ContainerListResponse>>>
            {
                ImageService.StartImageService
            }).Wait();

            var serviceCollection = new ServiceCollection();
            serviceCollection.ConfigureImageProvider(_server, 5000);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public void Dispose()
        {
            DockerExtentions.RemoveDockerServicesAsync().Wait();
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        [Fact]
        public async Task resize_image_jpg_quality_returns_successfully()
        {
            var imageService = _serviceProvider.GetRequiredService<IImageProvider>();
            var assembly = typeof(ImageServiceTests).GetTypeInfo().Assembly;
            var bytes = ReadFully(assembly.GetManifestResourceStream("Arragro.Providers.ImageServiceProvider.IntegrationTests.Resources.bear-hands.jpg"));
            var result = await imageService.GetImage(bytes);
            Assert.True(result.Size < bytes.Length);
            Assert.Equal("image/jpeg", result.MimeType);
            Assert.Equal(1201, result.Width);
            Assert.Equal(800, result.Height);
        }

        [Fact]
        public async Task resize_image_jpg_resize_returns_successfully()
        {
            var imageService = _serviceProvider.GetRequiredService<IImageProvider>();
            var assembly = typeof(ImageServiceTests).GetTypeInfo().Assembly;
            var bytes = ReadFully(assembly.GetManifestResourceStream("Arragro.Providers.ImageServiceProvider.IntegrationTests.Resources.bear-hands.jpg"));
            var result = await imageService.GetImage(bytes, 600, 80, true);
            Assert.True(result.Size < bytes.Length);
            Assert.Equal("image/jpeg", result.MimeType);
            Assert.Equal(600, result.Width);
            Assert.Equal(400, result.Height);
        }

        [Fact]
        public async Task resize_image_gif_quality_returns_successfully()
        {
            var imageService = _serviceProvider.GetRequiredService<IImageProvider>();
            var assembly = typeof(ImageServiceTests).GetTypeInfo().Assembly;
            var bytes = ReadFully(assembly.GetManifestResourceStream("Arragro.Providers.ImageServiceProvider.IntegrationTests.Resources.gold.gif"));
            var result = await imageService.GetImage(bytes);
            Assert.True(result.Size < bytes.Length);
            Assert.Equal("image/gif", result.MimeType);
            Assert.Equal(360, result.Width);
            Assert.Equal(202, result.Height);
        }

        [Fact]
        public async Task resize_image_gif_resize_returns_successfully()
        {
            var imageService = _serviceProvider.GetRequiredService<IImageProvider>();
            var assembly = typeof(ImageServiceTests).GetTypeInfo().Assembly;
            var bytes = ReadFully(assembly.GetManifestResourceStream("Arragro.Providers.ImageServiceProvider.IntegrationTests.Resources.gold.gif"));
            var result = await imageService.GetImage(bytes, 300, 80, true);
            Assert.True(result.Size < bytes.Length);
            Assert.Equal("image/gif", result.MimeType);
            Assert.Equal(300, result.Width);
            Assert.Equal(168, result.Height);
        }

        [Fact]
        public async Task resize_image_svg_returns_successfully()
        {
            var imageService = _serviceProvider.GetRequiredService<IImageProvider>();
            var assembly = typeof(ImageServiceTests).GetTypeInfo().Assembly;
            var bytes = ReadFully(assembly.GetManifestResourceStream("Arragro.Providers.ImageServiceProvider.IntegrationTests.Resources.ArragroCMSLogo.svg"));
            var result = await imageService.GetImage(bytes, 300, 80, true);
            Assert.False(result.IsImage);
            Assert.Equal(bytes.Length, result.Size);
        }
    }
}
