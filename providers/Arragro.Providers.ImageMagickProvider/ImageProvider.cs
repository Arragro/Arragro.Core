using Arragro.Core.Common.Interfaces.Providers;
using Arragro.Core.Common.Models;
using ImageMagick;
using System.IO;
using System.Threading.Tasks;

namespace Arragro.Providers.ImageMagickProvider
{
    public class ImageProvider : IImageProvider
    {
        class ProcessImageResult
        {
            public byte[] Bytes { get; set; }
            public string MimeType { get; set; }

            public ProcessImageResult()
            {
                MimeType = string.Empty;
            }
        }
        
        private ProcessImageResult ProcessImage(byte[] bytes, int? width = null, int quality = 80, bool asProgressiveJpeg = false)
        {
            var processImageResult = new ProcessImageResult();
            using (var image = new MagickImage(bytes))
            {
                using (var ms = new MemoryStream())
                {
                    if (width.HasValue)
                        image.Resize(width.Value, 0);
                    image.Strip();
                    image.Quality = quality;
                    if (asProgressiveJpeg)
                    {
                        image.Write(ms, MagickFormat.Pjpeg);
                        processImageResult.MimeType = "image/pjpeg";
                    }
                    else
                        image.Write(ms);
                    ms.Position = 0;
                    processImageResult.Bytes = ms.ToArray();
                }
            }
            return processImageResult;
        }

        private byte[] ProcessGif(byte[] bytes, int? width = null, int quality = 80)
        {
            byte[] output;
            using (MagickImageCollection collection = new MagickImageCollection(bytes))
            {
                collection.Coalesce();
                
                foreach (MagickImage image in collection)
                {
                    if (width.HasValue)
                        image.Resize(width.Value, 0);
                    image.Strip();
                    image.Quality = quality;
                    image.Density = new Density(72, 72);
                }

                using (var ms = new MemoryStream())
                {
                    collection.Write(ms);
                    ms.Position = 0;
                    output = ms.ToArray();
                }
            }
            return output;
        }

        public async Task<ImageProcessResult> GetImage(byte[] bytes, int width, int quality = 80, bool asProgressiveJpeg = false)
        {
            byte[] output;
            string mimeType = string.Empty;
    
            try
            {
                var info = new MagickImageInfo(bytes);

                switch (info.Format)
                {
                    case MagickFormat.Svg:
                    case MagickFormat.Gif:
                        output = bytes;
                        break;
                    //case MagickFormat.Gif:
                    //    output = ProcessGif(bytes, width, quality);
                    //    if (output.Length > bytes.Length)
                    //        output = bytes;
                    //    break;
                    default:
                        var result = ProcessImage(bytes, width, quality, asProgressiveJpeg);
                        output = result.Bytes;
                        mimeType = result.MimeType;
                        break;
                }
            }
            catch (MagickException)
            {
                return new ImageProcessResult { Bytes = bytes, IsImage = false, Size = bytes.Length };
            }

            var newImageInfo = new MagickImageInfo(output);
            var task = Task.Run(() => new ImageProcessResult { Bytes = output, IsImage = true, Width = newImageInfo.Width, Height = newImageInfo.Height, Size = output.Length, MimeType = mimeType });
            return await task;
        }

        public async Task<ImageProcessResult> GetImage(byte[] bytes, int quality, bool asProgressiveJpeg = false)
        {
            byte[] output;
            string mimeType = string.Empty;

            try
            {
                var info = new MagickImageInfo(bytes); switch (info.Format)
                {
                    case MagickFormat.Svg:
                    case MagickFormat.Gif:
                        output = bytes;
                        break;
                    //case MagickFormat.Gif:
                        //output = ProcessGif(bytes, quality: quality);
                        //if (output.Length > bytes.Length)
                        //    output = bytes;
                        //break;
                    default:
                        var result = ProcessImage(bytes, quality: quality, asProgressiveJpeg: asProgressiveJpeg);
                        output = result.Bytes;
                        mimeType = result.MimeType;
                        break;
                }
            }
            catch (MagickException)
            {
                return new ImageProcessResult { Bytes = bytes, IsImage = false, Size = bytes.Length };
            }

            var newImageInfo = new MagickImageInfo(output);
            var task = Task.Run(() => new ImageProcessResult { Bytes = output, IsImage = true, Width = newImageInfo.Width, Height = newImageInfo.Height, Size = output.Length, MimeType = mimeType });
            return await task;
        }
    }
}
