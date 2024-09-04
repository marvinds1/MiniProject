using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Authorization;
using Persistence.DatabaseContext;
using Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.helpers
{
    public class TokenAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TokenAuthorizationMiddleware(RequestDelegate next, IDistributedCache cache, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _cache = cache;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var authorizeAttributes = endpoint.Metadata.GetOrderedMetadata<AuthorizeAttribute>();
                var allowAnonymousAttributes = endpoint.Metadata.GetOrderedMetadata<AllowAnonymousAttribute>();

                if (allowAnonymousAttributes.Any())
                {
                    await _next(context);
                    return;
                }

                if (authorizeAttributes.Any())
                {
                    var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                    if (string.IsNullOrEmpty(token))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token not found");
                        return;
                    }

                    var jwtHelper = new JwtTokenHelpers();

                    var userId = jwtHelper.GetUserIdFromToken(token);

                    if (string.IsNullOrEmpty(userId))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync($"Invalid token: {userId}, {token}");
                        return;
                    }

                    string cachedToken = null;

                    try
                    {
                        cachedToken = await _cache.GetStringAsync(userId);
                    }
                    catch (Exception redisEx)
                    {
                        Console.WriteLine($"Redis error: {redisEx.Message}");
                    }

                    if (cachedToken == null)
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                            try
                            {
                                var userToken = await dbContext.UserToken
                                                            .Where(ut => ut.UserId == userId && ut.LoginProvider == "custom" && ut.Name == "AccessToken")
                                                            .Select(ut => ut.Value)
                                                            .FirstOrDefaultAsync();

                                if (userToken != null)
                                {
                                    cachedToken = userToken;

                                    try
                                    {
                                        await _cache.SetStringAsync(userId, cachedToken);
                                    }
                                    catch (Exception redisEx)
                                    {
                                        Console.WriteLine($"Redis error while setting cache: {redisEx.Message}");
                                    }
                                }

                            } catch (Exception e)
                            {
                                await context.Response.WriteAsync(e.Message);
                                return;
                            }

                        }
                    }

                    if (cachedToken != token)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync($"Token is not valid1: {cachedToken}");
                        return;
                    }

                }
            }

            await _next(context);
        }
    }

}

