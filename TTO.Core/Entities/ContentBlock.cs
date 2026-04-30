namespace TTO.Core.Entities;

public class ContentBlock : BaseEntity
{
    public int PageId { get; set; }
    public string BlockType { get; set; } = null!; // richtext | image | banner | video | html | gallery
    public string? Content { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    public Page Page { get; set; } = null!;
}
