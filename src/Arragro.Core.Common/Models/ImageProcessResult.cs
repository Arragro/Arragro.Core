namespace Arragro.Core.Common.Models
{
    public class ImageProcessResult
    {
        public byte[] Bytes { get; set; }
        public bool IsImage { get; set; }
        public long Size { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string MimeType { get; set; }
    }
}
