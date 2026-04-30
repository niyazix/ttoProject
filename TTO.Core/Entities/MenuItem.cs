using TTO.Core.Enums;

namespace TTO.Core.Entities;

public class MenuItem : BaseEntity
{
    public string Title { get; set; } = null!;
    public LinkType LinkType { get; set; } = LinkType.None;
    public int? PageId { get; set; }
    public string? Url { get; set; }
    public int? ParentId { get; set; }
    public int Level { get; set; } = 0;
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public bool OpenInNewTab { get; set; } = false;
    public string? Icon { get; set; }
    public string MenuGroup { get; set; } = "main"; // main | footer | sidebar

    public Page? Page { get; set; }
    public MenuItem? Parent { get; set; }
    public ICollection<MenuItem> Children { get; set; } = [];
}
