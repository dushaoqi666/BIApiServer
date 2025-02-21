using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

namespace BIApiServer.Common
{
    public class RedisConfig
    {
        private static ConnectionMultiplexer _redis;
        private static readonly object _lock = new object();

        public static ConnectionMultiplexer GetInstance(IConfiguration configuration)
        {
            if (_redis == null)
            {
                lock (_lock)
                {
                    if (_redis == null)
                    {
                        var connectionString = configuration.GetConnectionString("Redis");
                        _redis = ConnectionMultiplexer.Connect(connectionString);
                    }
                }
            }
            return _redis;
        }

        public static IDatabase GetDatabase()
        {
            return _redis.GetDatabase();
        }
    }
} 