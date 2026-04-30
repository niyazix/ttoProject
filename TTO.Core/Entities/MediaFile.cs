namespace TTO.Core.Entities;

public class MediaFile : BaseEntity
{
    public string FileName { get; set; } = null!;
    public string OriginalName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public string FolderPath { get; set; } = "/";
    public string FileType { get; set; } = null!; // image | document | video
    public string MimeType { get; set; } = null!;
    public long FileSize { get; set; }
    public string? AltText { get; set; }
}
