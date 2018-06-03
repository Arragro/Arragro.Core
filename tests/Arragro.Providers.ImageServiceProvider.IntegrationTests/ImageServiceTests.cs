using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace Arragro.Providers.ImageServiceProvider.IntegrationTests
{
    public class ImageServiceTests
    {
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
        public void resize_image_jpg_quality_returns_successfully()
        {
            var imageService = new ImageProvider("http://localhost:3000");
            var assembly = typeof(ImageServiceTests).GetTypeInfo().Assembly;
            var bytes = ReadFully(assembly.GetManifestResourceStream("Arragro.Providers.ImageServiceProvider.IntegrationTests.Resources.bear-hands.jpg"));
            var result = imageService.GetImage(bytes).Result;
            Assert.True(result.Size < bytes.Length);
            Assert.Equal("image/jpeg", result.MimeType);
            Assert.Equal(1201, result.Width);
            Assert.Equal(800, result.Height);
        }

        [Fact]
        public void resize_image_jpg_resize_returns_successfully()
        {
            var imageService = new ImageProvider("http://localhost:3000");
            var assembly = typeof(ImageServiceTests).GetTypeInfo().Assembly;
            var bytes = ReadFully(assembly.GetManifestResourceStream("Arragro.Providers.ImageServiceProvider.IntegrationTests.Resources.bear-hands.jpg"));
            var result = imageService.GetImage(bytes, 600, 80, true).Result;
            Assert.True(result.Size < bytes.Length);
            Assert.Equal("image/jpeg", result.MimeType);
            Assert.Equal(600, result.Width);
            Assert.Equal(400, result.Height);
        }

        [Fact]
        public void resize_image_gif_quality_returns_successfully()
        {
            var imageService = new ImageProvider("http://localhost:3000");
            var assembly = typeof(ImageServiceTests).GetTypeInfo().Assembly;
            var bytes = ReadFully(assembly.GetManifestResourceStream("Arragro.Providers.ImageServiceProvider.IntegrationTests.Resources.gold.gif"));
            var result = imageService.GetImage(bytes).Result;
            Assert.True(result.Size < bytes.Length);
            Assert.Equal("image/gif", result.MimeType);
            Assert.Equal(360, result.Width);
            Assert.Equal(202, result.Height);
        }

        [Fact]
        public void resize_image_gif_resize_returns_successfully()
        {
            var imageService = new ImageProvider("http://localhost:3000");
            var assembly = typeof(ImageServiceTests).GetTypeInfo().Assembly;
            var bytes = ReadFully(assembly.GetManifestResourceStream("Arragro.Providers.ImageServiceProvider.IntegrationTests.Resources.gold.gif"));
            var result = imageService.GetImage(bytes, 300, 80, true).Result;
            Assert.True(result.Size < bytes.Length);
            Assert.Equal("image/gif", result.MimeType);
            Assert.Equal(300, result.Width);
            Assert.Equal(168, result.Height);
        }

        [Fact]
        public void resize_image_svg_returns_successfully()
        {
            var imageService = new ImageProvider("http://localhost:3000");
            var assembly = typeof(ImageServiceTests).GetTypeInfo().Assembly;
            var bytes = ReadFully(assembly.GetManifestResourceStream("Arragro.Providers.ImageServiceProvider.IntegrationTests.Resources.ArragroCMSLogo.svg"));
            var result = imageService.GetImage(bytes, 300, 80, true).Result;
            Assert.False(result.IsImage);
            Assert.Equal(bytes.Length, result.Size);
        }
    }
}
