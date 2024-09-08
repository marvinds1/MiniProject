using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Core.Services
{
    public class RedisService
    {
        private readonly IDistributedCache _cache;
        private readonly CancellationToken _cancellationToken;

        public RedisService(IDistributedCache cache, CancellationToken cancellationToken = default)
        {
            _cache = cache;
            _cancellationToken = cancellationToken;
        }

        public async Task SaveToRedisAsync<T>(string cacheKey, IEnumerable<T> items, int expirationMinutes = 60)
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationMinutes)
            };

            var serializedData = JsonSerializer.Serialize(items);

            try
            {
                await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions, _cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save data to Redis: {ex.Message}");
            }
        }

        public async Task<IEnumerable<T>?> GetFromRedisAsync<T>(string cacheKey)
        {
            try
            {
                var cachedData = await _cache.GetStringAsync(cacheKey, _cancellationToken);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    return JsonSerializer.Deserialize<IEnumerable<T>>(cachedData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis error: {ex.Message}");
            }
            return null;
        }

        public async Task<bool> IsRedisAvailableAsync()
        {
            try
            {
                await _cache.GetStringAsync("RedisTestConnection", _cancellationToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task RemoveFromRedisAsync(string cacheKey)
        {
            try
            {
                await _cache.RemoveAsync(cacheKey, _cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to remove data from Redis: {ex.Message}");
            }
        }
    }
}
