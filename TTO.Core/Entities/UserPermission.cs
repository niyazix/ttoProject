namespace TTO.Core.Entities;

public class UserPermission
{
    public int UserId { get; set; }
    public int PermissionId { get; set; }
    public bool IsGranted { get; set; } = true;

    public User User { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
