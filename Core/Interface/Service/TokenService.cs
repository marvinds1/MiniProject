using Microsoft.Extensions.Caching.Distributed;
using Persistence.DatabaseContext;
using Persistence.Models;
using Persistence.Repositories;
using System;
using System.Threading.Tasks;

namespace Core.Interface.Service
{
    public class TokenService
    {
        private readonly IDistributedCache _cache;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly TimeSpan _tokenExpiry = TimeSpan.FromHours(1);

        public TokenService(IDistributedCache cache, IUserTokenRepository userTokenRepository)
        {
            _cache = cache;
            _userTokenRepository = userTokenRepository;
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving token to cache: {ex.Message}");
            }

            try
            {
                var userToken = await _userTokenRepository.GetUserTokenAsync(userId);
                if (userToken == null)
                {
                    userToken = new UserToken
                    {
                        UserId = userId,
                        LoginProvider = "custom",
                        Name = "AccessToken",
                        Value = token,
                        Expiry = DateTime.UtcNow.Add(_tokenExpiry)
                    };
                    await _userTokenRepository.AddUserTokenAsync(userToken);
                }
                else
                {
                    userToken.Value = token;
                    userToken.Expiry = DateTime.UtcNow.Add(_tokenExpiry);
                    await _userTokenRepository.UpdateUserTokenAsync(userToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving token to database: {ex.Message}");
            }
        }

        public async Task<string?> GetTokenAsync(string userId)
        {
            try
            {
                var token = await _cache.GetStringAsync(userId);
                if (!string.IsNullOrEmpty(token))
                {
                    return token;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving token from cache: {ex.Message}");
            }

            try
            {
                var userToken = await _userTokenRepository.GetUserTokenAsync(userId);
                return userToken?.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving token from database: {ex.Message}");
            }

            return null;
        }

        public async Task RemoveTokenAsync(string userId)
        {
            try
            {
                await _cache.RemoveAsync(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing token from cache: {ex.Message}");
            }

            try
            {
                await _userTokenRepository.RemoveUserTokenAsync(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing token from database: {ex.Message}");
            }
        }
    }
}