using System;

namespace Arragro.Core.Common.Models
{
    public class CreateImageFromImageResult
    {
        public ImageProcessResult ImageProcessResult { get; set; }
        public Uri Uri { get; set; }
        public Uri ThumbnailUri { get; set; }
    }
}
