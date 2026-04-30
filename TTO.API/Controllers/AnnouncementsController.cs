using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTO.API.Services;
using TTO.Core.Entities;
using TTO.Infrastructure.Data;

namespace TTO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnnouncementsController(AppDbContext db, PermissionService permissionService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPublished([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var now = DateTime.UtcNow;
        var query = db.Announcements
            .Include(a => a.Category)
            .Where(a => a.IsPublished
                && (a.PublishDate == null || a.PublishDate <= now)
                && (a.ExpiryDate == null || a.ExpiryDate > now));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.IsPinned)
            .ThenByDescending(a => a.PublishDate ?? a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new { a.Id, a.Title, a.Slug, a.Summary, a.CoverImageUrl, a.IsPinned, a.PublishDate, a.ViewCount, CategoryName = a.Category != null ? a.Category.Name : null })
            .ToListAsync();

        return Ok(new { total, page, pageSize, items });
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var announcement = await db.Announcements
            .Include(a => a.Category)
            .FirstOrDefaultAsync(a => a.Slug == slug && a.IsPublished);

        if (announcement == null) return NotFound();

        announcement.ViewCount++;
        await db.SaveChangesAsync();

        return Ok(announcement);
    }

    [HttpGet("admin/all")]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "announcements.view"))
            return Forbid();

        var total = await db.Announcements.CountAsync();
        var items = await db.Announcements
            .Include(a => a.Category)
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new { a.Id, a.Title, a.Slug, a.IsPublished, a.IsPinned, a.PublishDate, a.ExpiryDate, a.ViewCount })
            .ToListAsync();

        return Ok(new { total, page, pageSize, items });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateAnnouncementRequest request)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "announcements.create"))
            return Forbid();

        if (await db.Announcements.AnyAsync(a => a.Slug == request.Slug))
            return Conflict(new { message = "Bu slug zaten kullanımda." });

        var announcement = new Announcement
        {
            Title = request.Title,
            Slug = request.Slug,
            Summary = request.Summary,
            Content = request.Content,
            CoverImageUrl = request.CoverImageUrl,
            CategoryId = request.CategoryId,
            IsPinned = request.IsPinned,
            PublishDate = request.PublishDate,
            ExpiryDate = request.ExpiryDate,
            IsPublished = false,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        db.Announcements.Add(announcement);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBySlug), new { slug = announcement.Slug }, new { announcement.Id });
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAnnouncementRequest request)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "announcements.edit"))
            return Forbid();

        var a = await db.Announcements.FindAsync(id);
        if (a == null) return NotFound();

        a.Title = request.Title;
        a.Summary = request.Summary;
        a.Content = request.Content;
        a.CoverImageUrl = request.CoverImageUrl;
        a.CategoryId = request.CategoryId;
        a.IsPinned = request.IsPinned;
        a.PublishDate = request.PublishDate;
        a.ExpiryDate = request.ExpiryDate;
        a.UpdatedBy = userId;

        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id:int}/publish")]
    [Authorize]
    public async Task<IActionResult> TogglePublish(int id)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "announcements.publish"))
            return Forbid();

        var a = await db.Announcements.FindAsync(id);
        if (a == null) return NotFound();

        a.IsPublished = !a.IsPublished;
        a.UpdatedBy = userId;
        await db.SaveChangesAsync();
        return Ok(new { a.IsPublished });
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "announcements.delete"))
            return Forbid();

        var a = await db.Announcements.FindAsync(id);
        if (a == null) return NotFound();

        a.DeletedAt = DateTime.UtcNow;
        a.DeletedBy = userId;
        await db.SaveChangesAsync();
        return NoContent();
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
}

public record CreateAnnouncementRequest(string Title, string Slug, string? Summary, string Content, string? CoverImageUrl, int? CategoryId, bool IsPinned, DateTime? PublishDate, DateTime? ExpiryDate);
public record UpdateAnnouncementRequest(string Title, string? Summary, string Content, string? CoverImageUrl, int? CategoryId, bool IsPinned, DateTime? PublishDate, DateTime? ExpiryDate);
