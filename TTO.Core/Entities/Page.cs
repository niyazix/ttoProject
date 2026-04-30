using TTO.Core.Enums;

namespace TTO.Core.Entities;

public class Page : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public bool IsPublished { get; set; } = false;
    public bool IsSystem { get; set; } = false;
    public PageType PageType { get; set; } = PageType.Dynamic;
    public int SortOrder { get; set; } = 0;
    public int? ParentId { get; set; }

    public Page? Parent { get; set; }
    public ICollection<Page> Children { get; set; } = [];
    public ICollection<ContentBlock> ContentBlocks { get; set; } = [];
    public ICollection<MenuItem> MenuItems { get; set; } = [];
    public ICollection<PagePermission> PagePermissions { get; set; } = [];
}
