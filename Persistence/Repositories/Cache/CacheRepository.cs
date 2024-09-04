using Newtonsoft.Json;
using StackExchange.Redis;

namespace Persistence.Repositories
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _database;

        public RedisCacheService(ConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public bool CheckActive()
        {
            try
            {
                _database.Ping();
                return true;
            }
            catch (RedisConnectionException)
            {
                return false;
            }
        }

        public void Add(string key, object data)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(data);
                _database.StringSet(key, jsonData, TimeSpan.FromMinutes(60));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error adding data to Redis", ex);
            }
        }

        public bool Any(string key)
        {
            try
            {
                return _database.KeyExists(key);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error checking key existence in Redis", ex);
            }
        }

        public List<T>? Get<T>(string key)
        {
            try
            {
                if (Any(key) && CheckActive())
                {
                    string jsonData = _database.StringGet(key);
                    return JsonConvert.DeserializeObject<List<T>>(jsonData);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving data from Redis", ex);
            }

            return default;
        }

        public void Remove(string key)
        {
            try
            {
                _database.KeyDelete(key);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error removing data from Redis", ex);
            }
        }

        public void Clear()
        {
            try
            {
                var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints()[0]);
                server.FlushDatabase();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error clearing Redis database", ex);
            }
        }
    }
}
