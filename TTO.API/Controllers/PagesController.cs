using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TTO.API.DTOs;
using TTO.API.Services;
using TTO.Core.Entities;
using TTO.Core.Enums;
using TTO.Infrastructure.Data;

namespace TTO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PagesController(AppDbContext db, PermissionService permissionService) : ControllerBase
{
    // PUBLIC — yayınlanmış sayfaları listele
    [HttpGet]
    public async Task<IActionResult> GetPublished()
    {
        var pages = await db.Pages
            .Where(p => p.IsPublished)
            .OrderBy(p => p.SortOrder)
            .Select(p => new PageListItem(p.Id, p.Title, p.Slug, p.IsPublished, p.IsSystem, p.PageType, p.SortOrder, p.ParentId, p.UpdatedAt))
            .ToListAsync();
        return Ok(pages);
    }

    // PUBLIC — slug ile sayfa detayı
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var page = await db.Pages
            .Include(p => p.ContentBlocks.Where(cb => cb.IsActive))
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);

        if (page == null) return NotFound();

        return Ok(new PageDetail(
            page.Id, page.Title, page.Slug, page.MetaTitle, page.MetaDescription,
            page.IsPublished, page.IsSystem, page.PageType, page.SortOrder, page.ParentId,
            page.ContentBlocks.OrderBy(cb => cb.SortOrder)
                .Select(cb => new ContentBlockDto(cb.Id, cb.BlockType, cb.Content, cb.SortOrder, cb.IsActive))
                .ToList()
        ));
    }

    // ADMIN — tüm sayfalar
    [HttpGet("admin/all")]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "pages.view"))
            return Forbid();

        var pages = await db.Pages
            .OrderBy(p => p.SortOrder)
            .Select(p => new PageListItem(p.Id, p.Title, p.Slug, p.IsPublished, p.IsSystem, p.PageType, p.SortOrder, p.ParentId, p.UpdatedAt))
            .ToListAsync();
        return Ok(pages);
    }

    // ADMIN — yeni sayfa oluştur
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreatePageRequest request)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "pages.create"))
            return Forbid();

        if (await db.Pages.AnyAsync(p => p.Slug == request.Slug))
            return Conflict(new { message = "Bu slug zaten kullanımda." });

        var page = new Page
        {
            Title = request.Title,
            Slug = request.Slug,
            MetaTitle = request.MetaTitle,
            MetaDescription = request.MetaDescription,
            PageType = request.PageType,
            ParentId = request.ParentId,
            SortOrder = request.SortOrder,
            IsPublished = false,
            IsSystem = false,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        db.Pages.Add(page);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBySlug), new { slug = page.Slug }, new { page.Id, page.Slug });
    }

    // ADMIN — güncelle
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePageRequest request)
    {
        var userId = GetUserId();
        var page = await db.Pages.FindAsync(id);
        if (page == null) return NotFound();

        if (!await permissionService.HasPagePermissionAsync(userId, id, "edit"))
            return Forbid();

        page.Title = request.Title;
        page.MetaTitle = request.MetaTitle;
        page.MetaDescription = request.MetaDescription;
        page.SortOrder = request.SortOrder;
        page.ParentId = request.ParentId;
        page.UpdatedBy = userId;

        await db.SaveChangesAsync();
        return NoContent();
    }

    // ADMIN — yayınla / yayından kaldır
    [HttpPatch("{id:int}/publish")]
    [Authorize]
    public async Task<IActionResult> TogglePublish(int id)
    {
        var userId = GetUserId();
        var page = await db.Pages.FindAsync(id);
        if (page == null) return NotFound();

        if (!await permissionService.HasPagePermissionAsync(userId, id, "publish"))
            return Forbid();

        page.IsPublished = !page.IsPublished;
        page.UpdatedBy = userId;
        await db.SaveChangesAsync();
        return Ok(new { page.IsPublished });
    }

    // ADMIN — sil (sistem sayfaları silinemez)
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var page = await db.Pages.Include(p => p.ContentBlocks).FirstOrDefaultAsync(p => p.Id == id);
        if (page == null) return NotFound();

        if (page.IsSystem)
            return BadRequest(new { message = "Sistem sayfaları silinemez." });

        if (!await permissionService.HasPagePermissionAsync(userId, id, "delete"))
            return Forbid();

        // Cascade soft delete
        var now = DateTime.UtcNow;
        page.DeletedAt = now;
        page.DeletedBy = userId;
        foreach (var block in page.ContentBlocks)
        {
            block.DeletedAt = now;
            block.DeletedBy = userId;
        }

        await db.SaveChangesAsync();
        return NoContent();
    }

    // ADMIN — sayfa bloklarını listele
    [HttpGet("{id:int}/blocks")]
    [Authorize]
    public async Task<IActionResult> GetBlocks(int id)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPagePermissionAsync(userId, id, "edit"))
            return Forbid();

        var blocks = await db.ContentBlocks
            .Where(b => b.PageId == id)
            .OrderBy(b => b.SortOrder)
            .Select(b => new ContentBlockDto(b.Id, b.BlockType, b.Content, b.SortOrder, b.IsActive))
            .ToListAsync();

        return Ok(blocks);
    }

    // ADMIN — içerik bloğu ekle
    [HttpPost("{id:int}/blocks")]
    [Authorize]
    public async Task<IActionResult> AddBlock(int id, [FromBody] UpsertContentBlockRequest request)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPagePermissionAsync(userId, id, "edit"))
            return Forbid();

        var block = new ContentBlock
        {
            PageId    = id,
            BlockType = request.BlockType,
            Content   = request.Content,
            SortOrder = request.SortOrder,
            IsActive  = request.IsActive,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        db.ContentBlocks.Add(block);
        await db.SaveChangesAsync();
        return Ok(new { block.Id });
    }

    // ADMIN — içerik bloğu güncelle
    [HttpPut("{id:int}/blocks/{blockId:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateBlock(int id, int blockId, [FromBody] UpsertContentBlockRequest request)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPagePermissionAsync(userId, id, "edit"))
            return Forbid();

        var block = await db.ContentBlocks.FirstOrDefaultAsync(b => b.Id == blockId && b.PageId == id);
        if (block == null) return NotFound();

        block.BlockType = request.BlockType;
        block.Content   = request.Content;
        block.SortOrder = request.SortOrder;
        block.IsActive  = request.IsActive;
        block.UpdatedBy = userId;

        await db.SaveChangesAsync();
        return NoContent();
    }

    // ADMIN — içerik bloğu sil
    [HttpDelete("{id:int}/blocks/{blockId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteBlock(int id, int blockId)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPagePermissionAsync(userId, id, "edit"))
            return Forbid();

        var block = await db.ContentBlocks.FirstOrDefaultAsync(b => b.Id == blockId && b.PageId == id);
        if (block == null) return NotFound();

        block.DeletedAt = DateTime.UtcNow;
        block.DeletedBy = userId;
        await db.SaveChangesAsync();
        return NoContent();
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
}
