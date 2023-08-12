using System;

namespace Galery.Data.Entities
{
    public class ImageInfo
    {
        public Guid Id { get; set; }
        public string FullPath { get; set; } = null!;
        public string PreviewFullPath { get; set; } = null!;

        public long PixelWidth { get; set; } = 0;
        public long PixelHeight { get; set; } = 0;
        public long FileSize { get; set; } = 0;
        public string FormatedFileSize { get => $"{Math.Ceiling(this.FileSize / 1024.0):N} KB"; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public string FormatedCreationDate { get => this.CreationTime.ToString("yyyy\'-\'MM\'-\'dd"); }

    }
}
