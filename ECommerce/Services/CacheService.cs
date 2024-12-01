using StackExchange.Redis;
using E_Commerce.Interfaces;

namespace E_Commerce.Services
{
    public class CacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;

        public CacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<string> GetCacheValueAsync(string key)
        {
            var db = _redis.GetDatabase();
            return await db.StringGetAsync(key);
        }

        public async Task SetCacheValueAsync(string key, string value, TimeSpan? expiry = null)
        {
            var db = _redis.GetDatabase();
            await db.StringSetAsync(key, value, expiry);
        }
    }
}