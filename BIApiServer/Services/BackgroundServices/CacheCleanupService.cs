using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BIApiServer.Services.BackgroundServices
{
    public class CacheCleanupService : BackgroundService
    {
        private readonly ILogger<CacheCleanupService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public CacheCleanupService(
            ILogger<CacheCleanupService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("开始执行缓存清理任务");

                    // 只在Redis启用时执行清理
                    if (_configuration.GetValue<bool>("Redis:Enabled"))
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();
                            
                            // 这里可以添加具体的清理逻辑
                            // 例如：清理过期的文件列表缓存
                            // await redisService.RemoveKeyAsync("file:list:*");
                            
                            _logger.LogInformation("缓存清理完成");
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Redis未启用，跳过缓存清理");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "缓存清理任务执行失败");
                }

                // 每小时执行一次
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
} 