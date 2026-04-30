using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTO.API.DTOs;
using TTO.API.Services;
using TTO.Core.Entities;
using TTO.Infrastructure.Data;

namespace TTO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController(AppDbContext db, PermissionService permissionService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPublished([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int? categoryId = null)
    {
        var query = db.News
            .Include(n => n.Category)
            .Where(n => n.IsPublished && (n.PublishDate == null || n.PublishDate <= DateTime.UtcNow));

        if (categoryId.HasValue)
            query = query.Where(n => n.CategoryId == categoryId);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(n => n.PublishDate ?? n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NewsListItem(n.Id, n.Title, n.Slug, n.Summary, n.CoverImageUrl, n.Author, n.IsPublished, n.IsFeatured, n.PublishDate, n.ViewCount, n.Category != null ? n.Category.Name : null))
            .ToListAsync();

        return Ok(new { total, page, pageSize, items });
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var news = await db.News
            .Include(n => n.Category)
            .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag)
            .FirstOrDefaultAsync(n => n.Slug == slug && n.IsPublished);

        if (news == null) return NotFound();

        news.ViewCount++;
        await db.SaveChangesAsync();

        return Ok(new NewsDetail(
            news.Id, news.Title, news.Slug, news.Summary, news.Content,
            news.CoverImageUrl, news.Author, news.IsPublished, news.IsFeatured,
            news.PublishDate, news.ViewCount, news.CategoryId,
            news.Category?.Name,
            news.NewsTags.Select(nt => nt.Tag.Name).ToList()
        ));
    }

    [HttpGet("admin/all")]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "news.view"))
            return Forbid();

        var query = db.News.Include(n => n.Category);
        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NewsListItem(n.Id, n.Title, n.Slug, n.Summary, n.CoverImageUrl, n.Author, n.IsPublished, n.IsFeatured, n.PublishDate, n.ViewCount, n.Category != null ? n.Category.Name : null))
            .ToListAsync();

        return Ok(new { total, page, pageSize, items });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateNewsRequest request)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "news.create"))
            return Forbid();

        if (await db.News.AnyAsync(n => n.Slug == request.Slug))
            return Conflict(new { message = "Bu slug zaten kullanımda." });

        var news = new News
        {
            Title = request.Title,
            Slug = request.Slug,
            Summary = request.Summary,
            Content = request.Content,
            CoverImageUrl = request.CoverImageUrl,
            CategoryId = request.CategoryId,
            Author = request.Author,
            PublishDate = request.PublishDate,
            IsFeatured = request.IsFeatured,
            IsPublished = false,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        db.News.Add(news);
        await db.SaveChangesAsync();

        await SyncTagsAsync(news.Id, request.Tags ?? []);

        return CreatedAtAction(nameof(GetBySlug), new { slug = news.Slug }, new { news.Id });
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateNewsRequest request)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "news.edit"))
            return Forbid();

        var news = await db.News.FindAsync(id);
        if (news == null) return NotFound();

        news.Title = request.Title;
        news.Summary = request.Summary;
        news.Content = request.Content;
        news.CoverImageUrl = request.CoverImageUrl;
        news.CategoryId = request.CategoryId;
        news.Author = request.Author;
        news.PublishDate = request.PublishDate;
        news.IsFeatured = request.IsFeatured;
        news.UpdatedBy = userId;

        await db.SaveChangesAsync();
        await SyncTagsAsync(id, request.Tags ?? []);

        return NoContent();
    }

    [HttpPatch("{id:int}/publish")]
    [Authorize]
    public async Task<IActionResult> TogglePublish(int id)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "news.publish"))
            return Forbid();

        var news = await db.News.FindAsync(id);
        if (news == null) return NotFound();

        news.IsPublished = !news.IsPublished;
        news.UpdatedBy = userId;
        await db.SaveChangesAsync();
        return Ok(new { news.IsPublished });
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "news.delete"))
            return Forbid();

        var news = await db.News.FindAsync(id);
        if (news == null) return NotFound();

        news.DeletedAt = DateTime.UtcNow;
        news.DeletedBy = userId;
        await db.SaveChangesAsync();
        return NoContent();
    }

    private async Task SyncTagsAsync(int newsId, List<string> tagNames)
    {
        var existing = await db.NewsTags.Where(nt => nt.NewsId == newsId).ToListAsync();
        db.NewsTags.RemoveRange(existing);

        foreach (var name in tagNames.Distinct())
        {
            var slug = name.ToLower().Replace(" ", "-");
            var tag = await db.Tags.FirstOrDefaultAsync(t => t.Slug == slug)
                      ?? db.Tags.Add(new Tag { Name = name, Slug = slug }).Entity;
            await db.SaveChangesAsync();
            db.NewsTags.Add(new NewsTag { NewsId = newsId, TagId = tag.Id });
        }
        await db.SaveChangesAsync();
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
}
