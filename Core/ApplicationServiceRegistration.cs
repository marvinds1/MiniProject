using System.Reflection;
using Core.Interface.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<TokenService>();

        return services;
    }
}