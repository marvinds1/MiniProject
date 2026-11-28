using System.Reflection;
using Core.Admin;
using Core.Caching;
using Core.Interface.Service.Auth;
using Core.Interface.Service.Todo;
using Core.Interface.Service.TodoDetail;
using Core.Options;
using Core.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        services.AddOptions<JwtOptions>().Bind(configuration.GetSection(JwtOptions.Section)).ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<CacheOptions>().Bind(configuration.GetSection(CacheOptions.Section)).ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<AdminOptions>().Bind(configuration.GetSection(AdminOptions.Section));

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<JwtTokenFactory>();
        services.AddSingleton<UserDataCache>();
        services.AddScoped<ISessionValidator, SessionValidator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITodoService, TodoService>();
        services.AddScoped<ITodoDetailService, TodoDetailService>();
        services.AddScoped<AdminBootstrapper>();

        return services;
    }
}
