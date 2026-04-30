using TTO.API.Services;

namespace TTO.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<TokenService>();
        services.AddScoped<PermissionService>();
        return services;
    }
}
