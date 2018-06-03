using Arragro.Core.Common.Models;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Interfaces.Providers
{
    public interface IImageProvider
    {
        Task<ImageProcessResult> GetImage(byte[] bytes, int quality = 80, bool asProgressiveJpeg = false);
        Task<ImageProcessResult> GetImage(byte[] bytes, int width, int quality = 80, bool asProgressiveJpeg = false);
    }
}
