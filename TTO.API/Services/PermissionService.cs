using Microsoft.EntityFrameworkCore;
using TTO.Infrastructure.Data;

namespace TTO.API.Services;

public class PermissionService(AppDbContext db)
{
    public async Task<bool> HasPermissionAsync(int userId, string permissionName)
    {
        // Admin her şeye erişir
        var isAdmin = await db.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == "Admin" && ur.Role.IsSystem);
        if (isAdmin) return true;

        // Kullanıcıya özel explicit deny
        var userOverride = await db.UserPermissions
            .Include(up => up.Permission)
            .FirstOrDefaultAsync(up => up.UserId == userId && up.Permission.Name == permissionName);
        if (userOverride != null) return userOverride.IsGranted;

        // Rol bazlı yetki
        return await db.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .AnyAsync(rp => rp.Permission.Name == permissionName);
    }

    public async Task<bool> HasPagePermissionAsync(int userId, int pageId, string action)
    {
        // Admin bypass
        var isAdmin = await db.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == "Admin" && ur.Role.IsSystem);
        if (isAdmin) return true;

        // Kullanıcı bazlı page permission (kullanıcı > rol)
        var userPagePerm = await db.PagePermissions
            .FirstOrDefaultAsync(pp => pp.PageId == pageId && pp.UserId == userId);
        if (userPagePerm != null)
            return action switch
            {
                "view"    => userPagePerm.CanView,
                "edit"    => userPagePerm.CanEdit,
                "publish" => userPagePerm.CanPublish,
                "delete"  => userPagePerm.CanDelete,
                _         => false
            };

        // Rol bazlı page permission
        var roleIds = await db.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        var rolePerm = await db.PagePermissions
            .Where(pp => pp.PageId == pageId && pp.RoleId != null && roleIds.Contains(pp.RoleId!.Value))
            .ToListAsync();

        if (!rolePerm.Any()) return false;

        return action switch
        {
            "view"    => rolePerm.Any(p => p.CanView),
            "edit"    => rolePerm.Any(p => p.CanEdit),
            "publish" => rolePerm.Any(p => p.CanPublish),
            "delete"  => rolePerm.Any(p => p.CanDelete),
            _         => false
        };
    }

    public async Task<List<string>> GetUserPermissionsAsync(int userId)
    {
        var isAdmin = await db.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == "Admin" && ur.Role.IsSystem);
        if (isAdmin)
            return await db.Permissions.Select(p => p.Name).ToListAsync();

        var rolePermissions = await db.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .ToListAsync();

        var userGranted = await db.UserPermissions
            .Where(up => up.UserId == userId && up.IsGranted)
            .Select(up => up.Permission.Name)
            .ToListAsync();

        var userDenied = await db.UserPermissions
            .Where(up => up.UserId == userId && !up.IsGranted)
            .Select(up => up.Permission.Name)
            .ToListAsync();

        return rolePermissions.Union(userGranted).Except(userDenied).Distinct().ToList();
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
    {
        return await db.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name)
            .ToListAsync();
    }
}
