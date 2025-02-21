using System.Text.Json;
using StackExchange.Redis;

namespace BIApiServer.Services
{
    public interface IRedisService
    {
        Task SetValueAsync(string key, string value);
        Task<string> GetValueAsync(string key);
        Task SetObjectAsync<T>(string key, T value);
        Task SetObjectAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<T> GetObjectAsync<T>(string key);
        Task<bool> RemoveKeyAsync(string key);
        Task<bool> SetKeyExpiryAsync(string key, TimeSpan expiry);
    }
} 