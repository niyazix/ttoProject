namespace TTO.Core.Entities;

public class Announcement : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Summary { get; set; }
    public string Content { get; set; } = null!;
    public string? CoverImageUrl { get; set; }
    public int? CategoryId { get; set; }
    public bool IsPublished { get; set; } = false;
    public bool IsPinned { get; set; } = false;
    public DateTime? PublishDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int ViewCount { get; set; } = 0;

    public Category? Category { get; set; }
}
