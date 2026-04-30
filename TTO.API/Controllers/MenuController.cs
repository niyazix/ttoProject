using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTO.API.Services;
using TTO.Core.Entities;
using TTO.Core.Enums;
using TTO.Infrastructure.Data;

namespace TTO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController(AppDbContext db, PermissionService permissionService) : ControllerBase
{
    [HttpGet("{group}")]
    public async Task<IActionResult> GetMenu(string group)
    {
        var entities = await db.MenuItems
            .Include(m => m.Children.Where(c => c.IsActive))
            .Where(m => m.MenuGroup == group && m.ParentId == null && m.IsActive)
            .OrderBy(m => m.SortOrder)
            .ToListAsync();

        // Sayfa slug'larını ayrı sorguda çek
        var allIds = entities.Select(m => m.PageId)
            .Concat(entities.SelectMany(m => m.Children).Select(c => c.PageId))
            .Where(id => id.HasValue).Select(id => id!.Value)
            .Distinct().ToList();

        var slugMap = allIds.Any()
            ? await db.Pages.IgnoreQueryFilters()
                .Where(p => allIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.Slug)
            : new Dictionary<int, string>();

        string? Url(MenuItem m) => m.LinkType switch
        {
            LinkType.Page => m.PageId.HasValue && slugMap.TryGetValue(m.PageId.Value, out var slug)
                ? "/" + slug
                : null,
            LinkType.Url  => string.IsNullOrEmpty(m.Url) ? null : m.Url,
            _             => null
        };

        var items = entities.Select(m => new MenuItemDto(
            m.Id, m.Title, m.LinkType.ToString(), m.PageId, Url(m),
            m.Level, m.SortOrder, m.OpenInNewTab, m.Icon,
            m.Children.OrderBy(c => c.SortOrder)
                .Select(c => new MenuItemDto(c.Id, c.Title, c.LinkType.ToString(), c.PageId,
                    Url(c), c.Level, c.SortOrder, c.OpenInNewTab, c.Icon,
                    new List<MenuItemDto>()))
                .ToList()
        )).ToList();

        return Ok(items);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateMenuItemRequest request)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "menu.manage"))
            return Forbid();

        // Maksimum 2 seviye derinlik
        if (request.ParentId.HasValue)
        {
            var parent = await db.MenuItems.FindAsync(request.ParentId.Value);
            if (parent?.ParentId != null)
                return BadRequest(new { message = "Menü maksimum 2 seviye derinliğe sahip olabilir." });
        }

        var level = request.ParentId.HasValue ? 1 : 0;
        var menuItem = new MenuItem
        {
            Title = request.Title,
            LinkType = Enum.Parse<LinkType>(request.LinkType, ignoreCase: true),
            PageId = request.PageId,
            Url = request.Url,
            ParentId = request.ParentId,
            Level = level,
            SortOrder = request.SortOrder,
            MenuGroup = request.MenuGroup,
            OpenInNewTab = request.OpenInNewTab,
            Icon = request.Icon,
            IsActive = true,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        db.MenuItems.Add(menuItem);
        await db.SaveChangesAsync();
        return Ok(new { menuItem.Id });
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMenuItemRequest request)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "menu.manage"))
            return Forbid();

        var item = await db.MenuItems.FindAsync(id);
        if (item == null) return NotFound();

        item.Title = request.Title;
        item.LinkType = Enum.Parse<LinkType>(request.LinkType, ignoreCase: true);
        item.PageId = request.PageId;
        item.Url = request.Url;
        item.SortOrder = request.SortOrder;
        item.OpenInNewTab = request.OpenInNewTab;
        item.Icon = request.Icon;
        item.IsActive = request.IsActive;
        item.UpdatedBy = userId;

        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "menu.manage"))
            return Forbid();

        var item = await db.MenuItems
            .Include(m => m.Children)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (item == null) return NotFound();

        // Cascade soft delete
        var now = DateTime.UtcNow;
        item.DeletedAt = now;
        item.DeletedBy = userId;
        foreach (var child in item.Children)
        {
            child.DeletedAt = now;
            child.DeletedBy = userId;
        }

        await db.SaveChangesAsync();
        return NoContent();
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
}

public record MenuItemDto(int Id, string Title, string LinkType, int? PageId, string? Url, int Level, int SortOrder, bool OpenInNewTab, string? Icon, List<MenuItemDto> Children);
public record CreateMenuItemRequest(string Title, string LinkType, int? PageId, string? Url, int? ParentId, string MenuGroup, int SortOrder, bool OpenInNewTab, string? Icon);
public record UpdateMenuItemRequest(string Title, string LinkType, int? PageId, string? Url, int SortOrder, bool OpenInNewTab, string? Icon, bool IsActive);
