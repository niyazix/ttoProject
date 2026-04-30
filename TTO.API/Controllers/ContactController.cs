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
public class ContactController(AppDbContext db, PermissionService permissionService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Send([FromBody] SendContactRequest request)
    {
        var message = new ContactMessage
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            Subject = request.Subject,
            Message = request.Message,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        };

        db.ContactMessages.Add(message);
        await db.SaveChangesAsync();
        return Ok(new { message = "Mesajınız iletildi." });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] bool? unreadOnly = null)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "contact.view"))
            return Forbid();

        var query = db.ContactMessages.AsQueryable();
        if (unreadOnly == true)
            query = query.Where(m => !m.IsRead);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new { m.Id, m.FullName, m.Email, m.Subject, m.IsRead, m.IsReplied, m.CreatedAt })
            .ToListAsync();

        return Ok(new { total, page, pageSize, items });
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "contact.view"))
            return Forbid();

        var msg = await db.ContactMessages.FindAsync(id);
        if (msg == null) return NotFound();

        if (!msg.IsRead)
        {
            msg.IsRead = true;
            await db.SaveChangesAsync();
        }

        return Ok(msg);
    }

    [HttpPatch("{id:int}/replied")]
    [Authorize]
    public async Task<IActionResult> MarkReplied(int id)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "contact.view"))
            return Forbid();

        var msg = await db.ContactMessages.FindAsync(id);
        if (msg == null) return NotFound();

        msg.IsReplied = true;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (!await permissionService.HasPermissionAsync(userId, "contact.delete"))
            return Forbid();

        var msg = await db.ContactMessages.FindAsync(id);
        if (msg == null) return NotFound();

        msg.DeletedAt = DateTime.UtcNow;
        msg.DeletedBy = userId;
        await db.SaveChangesAsync();
        return NoContent();
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
}

public record SendContactRequest(string FullName, string Email, string? Phone, string Subject, string Message);
