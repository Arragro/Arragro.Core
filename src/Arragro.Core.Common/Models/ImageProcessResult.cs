namespace Arragro.Core.Common.Models
{
    public class ImageProcessDetailsResult
    {
        public bool IsImage { get; set; }
        public long Size { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string MimeType { get; set; }
    }

    public class ImageProcessResult : ImageProcessDetailsResult
    {
        public byte[] Bytes { get; set; }
    }
}
