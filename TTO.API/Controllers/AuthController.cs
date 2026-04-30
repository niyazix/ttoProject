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
public class AuthController(AppDbContext db, TokenService tokenService, PermissionService permissionService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "E-posta veya şifre hatalı." });

        if (!user.IsActive)
            return Unauthorized(new { message = "Hesabınız devre dışı." });

        user.LastLoginAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        var roles = await permissionService.GetUserRolesAsync(user.Id);
        var permissions = await permissionService.GetUserPermissionsAsync(user.Id);
        var token = tokenService.GenerateAccessToken(user, roles, permissions);

        return Ok(new LoginResponse(
            AccessToken: token,
            Email: user.Email,
            FullName: $"{user.FirstName} {user.LastName}",
            Roles: roles.ToList(),
            Permissions: permissions
        ));
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await db.Users.AnyAsync(u => u.Email == request.Email))
            return Conflict(new { message = "Bu e-posta adresi zaten kullanımda." });

        if (await db.Users.AnyAsync(u => u.Username == request.Username))
            return Conflict(new { message = "Bu kullanıcı adı zaten kullanımda." });

        var user = new User
        {
            Email = request.Email,
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsActive = true
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return Ok(new { id = user.Id, message = "Kullanıcı oluşturuldu." });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var user = await db.Users.FindAsync(userId);
        if (user == null) return NotFound();

        var roles = await permissionService.GetUserRolesAsync(userId);
        var permissions = await permissionService.GetUserPermissionsAsync(userId);

        return Ok(new
        {
            user.Id,
            user.Email,
            user.Username,
            user.FirstName,
            user.LastName,
            FullName = $"{user.FirstName} {user.LastName}",
            Roles = roles,
            Permissions = permissions
        });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var user = await db.Users.FindAsync(userId);
        if (user == null) return NotFound();

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            return BadRequest(new { message = "Mevcut şifre hatalı." });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await db.SaveChangesAsync();

        return Ok(new { message = "Şifre güncellendi." });
    }
}
