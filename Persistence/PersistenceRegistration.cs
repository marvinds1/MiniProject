using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.DatabaseContext;
using Persistence.Repositories;
//using Persistence.Repositories.ToDo;
//using Persistence.Repositories.ToDoDetail;
using StackExchange.Redis;

namespace Persistence
{
    public static class PersistenceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            string dbConnection = configuration.GetConnectionString("Database");
            string redisConnection = configuration.GetConnectionString("Redis");

            services.AddDbContext<ApplicationDBContext>(opt =>
                opt.UseMySql(dbConnection, ServerVersion.AutoDetect(dbConnection))
            );

            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = redisConnection;
            });

            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                try
                {
                    return ConnectionMultiplexer.Connect(redisConnection);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Failed to connect to Redis: {ex.Message}"); 
                }
            });

            services.AddScoped<ICacheService, RedisCacheService>();
            //services.AddScoped<ITodoRepository, TodoRepository>();
            //services.AddScoped<ITodoDetailRepository, TodoDetailRepository>();
            services.AddScoped<IUserTokenRepository, UserTokenRepository>();

            return services;
        }
    }
}
