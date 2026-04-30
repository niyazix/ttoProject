namespace TTO.API.DTOs;

public record LoginRequest(string Email, string Password);

public record LoginResponse(string AccessToken, string Email, string FullName, List<string> Roles, List<string> Permissions);

public record RegisterRequest(string Email, string Username, string Password, string FirstName, string LastName);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
