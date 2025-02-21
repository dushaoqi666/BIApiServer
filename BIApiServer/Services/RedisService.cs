using System.Text.Json;
using StackExchange.Redis;

namespace BIApiServer.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _redisDb;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        // 设置 Key 的值
        public async Task SetValueAsync(string key, string value)
        {
            await _redisDb.StringSetAsync(key, value);
        }

        // 获取 Key 的值
        public async Task<string> GetValueAsync(string key)
        {
            return await _redisDb.StringGetAsync(key);
        }

        // 设置对象
        public async Task SetObjectAsync<T>(string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            await _redisDb.StringSetAsync(key, json);
        }

        // 设置对象并设置过期时间
        public async Task SetObjectAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value);
            await _redisDb.StringSetAsync(key, json, expiry);
        }

        // 获取对象
        public async Task<T> GetObjectAsync<T>(string key)
        {
            var json = await _redisDb.StringGetAsync(key);
            if (json.IsNullOrEmpty)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json);
        }

        // 移除 Key
        public async Task<bool> RemoveKeyAsync(string key)
        {
            return await _redisDb.KeyDeleteAsync(key);
        }

        // 设置 Key 的过期时间
        public async Task<bool> SetKeyExpiryAsync(string key, TimeSpan expiry)
        {
            return await _redisDb.KeyExpireAsync(key, expiry);
        }
    }

    public class NullRedisService : IRedisService
    {
        public Task SetValueAsync(string key, string value)
        {
            return Task.CompletedTask;
        }

        public Task<string> GetValueAsync(string key)
        {
            return Task.FromResult<string>(null);
        }

        public Task SetObjectAsync<T>(string key, T value)
        {
            return Task.CompletedTask;
        }

        public Task SetObjectAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            return Task.CompletedTask;
        }

        public Task<T> GetObjectAsync<T>(string key)
        {
            return Task.FromResult<T>(default);
        }

        public Task<bool> RemoveKeyAsync(string key)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SetKeyExpiryAsync(string key, TimeSpan expiry)
        {
            return Task.FromResult(true);
        }
    }
} 