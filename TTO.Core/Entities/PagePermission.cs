namespace TTO.Core.Entities;

public class PagePermission : BaseEntity
{
    public int PageId { get; set; }
    public int? UserId { get; set; }
    public int? RoleId { get; set; }
    public bool CanView { get; set; } = true;
    public bool CanEdit { get; set; } = false;
    public bool CanPublish { get; set; } = false;
    public bool CanDelete { get; set; } = false;

    public Page Page { get; set; } = null!;
    public User? User { get; set; }
    public Role? Role { get; set; }
}
