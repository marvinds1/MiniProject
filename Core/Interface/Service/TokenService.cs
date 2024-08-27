using Microsoft.Extensions.Caching.Distributed;
using Persistence.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Persistence.DatabaseContext;
using System;
using Persistence.Models.Persistence.Models;

public class TokenService
{
    private readonly IDistributedCache _cache;
    private readonly ApplicationDBContext _context;
    private readonly TimeSpan _tokenExpiry = TimeSpan.FromHours(1);

    public TokenService(IDistributedCache cache, ApplicationDBContext context)
    {
        _cache = cache;
        _context = context;
    }

    public async Task StoreTokenAsync(string userId, string token)
    {
        try
        {
            await _cache.SetStringAsync(userId, token, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _tokenExpiry
            });
        }
        catch (Exception)
        {
            // Jika Redis tidak tersedia, gunakan database
            var userToken = await _context.UserTokens.FindAsync(userId);
            if (userToken == null)
            {
                userToken = new UserToken
                {
                    UserId = userId,
                    LoginProvider = "JWT",
                    Name = "AccessToken",
                    Value = token,
                    Expiry = DateTime.UtcNow.Add(_tokenExpiry)
                };
                _context.UserTokens.Add(userToken);
            }
            else
            {
                userToken.Value = token;
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task<string?> GetTokenAsync(string userId)
    {
        try
        {
            return await _cache.GetStringAsync(userId);
        }
        catch (Exception)
        {
            // Jika Redis tidak tersedia, gunakan database
            var userToken = await _context.UserTokens.FindAsync(userId);
            return userToken?.Value;
        }
    }

    public async Task RemoveTokenAsync(string userId)
    {
        try
        {
            await _cache.RemoveAsync(userId);
        }
        catch (Exception)
        {
            var userToken = await _context.UserTokens.FindAsync(userId);
            if (userToken != null)
            {
                _context.UserTokens.Remove(userToken);
                await _context.SaveChangesAsync();
            }
        }
    }
}
