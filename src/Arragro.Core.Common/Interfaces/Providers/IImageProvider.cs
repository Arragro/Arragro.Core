using Arragro.Core.Common.Models;

namespace Arragro.Core.Common.Interfaces.Providers
{
    public interface IImageProvider
    {
        ImageProcessResult GetImage(byte[] bytes, int quality = 80, bool asProgressiveJpeg = false);
        ImageProcessResult GetImage(byte[] bytes, int width, int quality = 80, bool asProgressiveJpeg = false);
    }
}
