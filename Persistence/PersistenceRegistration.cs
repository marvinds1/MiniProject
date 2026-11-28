using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.DatabaseContext;
using Persistence.Repositories.RefreshToken;
using Persistence.Repositories.ToDo;
using Persistence.Repositories.ToDoDetail;

namespace Persistence
{
    public static class PersistenceRegistration
    {
        public static readonly ServerVersion DefaultServerVersion = new MySqlServerVersion(new Version(8, 4, 0));

        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConnection = configuration.GetConnectionString("Database");
            if (string.IsNullOrWhiteSpace(dbConnection))
                throw new InvalidOperationException("ConnectionStrings:Database is not configured.");

            var serverVersion = configuration["Database:ServerVersion"] is { Length: > 0 } v
                ? ServerVersion.Parse(v)
                : DefaultServerVersion;

            services.AddDbContext<ApplicationDBContext>(opt =>
                opt.UseMySql(dbConnection, serverVersion, mysql => mysql.EnableRetryOnFailure()));

            var redisConnection = configuration.GetConnectionString("Redis");
            if (string.IsNullOrWhiteSpace(redisConnection))
                services.AddDistributedMemoryCache();
            else
                services.AddStackExchangeRedisCache(opt => opt.Configuration = redisConnection);

            services.AddScoped<ITodoRepository, TodoRepository>();
            services.AddScoped<ITodoDetailRepository, TodoDetailRepository>();
            services.AddScoped<IRefreshTokenStore, RefreshTokenStore>();

            return services;
        }
    }
}
