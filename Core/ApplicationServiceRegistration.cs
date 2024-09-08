using System.Reflection;
using Core.Interface.Service;
using Core.Interface.Service.Auth;
using Core.Interface.Service.Todo;
using Core.Interface.Service.TodoDetail;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITodoService, TodoService>();
        services.AddScoped<ITodoDetailService, TodoDetailService>();
        services.AddSingleton<RedisService>();
        services.AddScoped<TokenService>();

        return services;
    }
}