using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.DatabaseContext;
using Persistence.Repositories;
using Persistence.Repositories.ToDo;
using Persistence.Repositories.ToDoDetail;
using StackExchange.Redis;
using System;

namespace Persistence
{
    public static class PersistenceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            string dbConnection = configuration.GetConnectionString("Database");
            string redisConnection = configuration.GetConnectionString("Redis");

            // Mendaftarkan ApplicationDBContext
            services.AddDbContext<ApplicationDBContext>(opt =>
                opt.UseMySql(dbConnection, ServerVersion.AutoDetect(dbConnection))
            );

            // Mendaftarkan StackExchangeRedisCache
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = redisConnection;
            });

            // Mendaftarkan ConnectionMultiplexer sebagai singleton
            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                try
                {
                    return ConnectionMultiplexer.Connect(redisConnection);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to connect to Redis: {ex.Message}");
                    throw;
                }
            });

            // Mendaftarkan ICacheService dengan RedisCacheService
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<ITodoRepository, TodoRepository>();
            services.AddScoped<ITodoDetailRepository, TodoDetailRepository>();

            return services;
        }
    }
}
