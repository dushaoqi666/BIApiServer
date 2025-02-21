using System.Net.Http;
using System.Text;
using BIApiServer.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BIApiServer.Services
{
    public class TaskExecutorService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TaskExecutorService> _logger;
        private readonly string _baseUrl;
        private readonly IWebHostEnvironment _environment;

        public TaskExecutorService(
            IServiceScopeFactory serviceScopeFactory,
            IHttpClientFactory httpClientFactory,
            ILogger<TaskExecutorService> logger,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5166";
            _environment = environment;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var taskService = scope.ServiceProvider.GetRequiredService<TaskManagementService>();
                        var tasks = await taskService.GetAllTasksAsync();
                        
                        foreach (var task in tasks.Where(t => t.IsEnabled))
                        {
                            if (!ShouldExecuteTask(task)) continue;

                            await ExecuteTaskAsync(task, taskService);
                            task.LastExecutionTime = DateTime.Now;
                            await taskService.UpdateTaskAsync(task.Id, task);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "执行任务时发生错误");
                }

                await Task.Delay(1000, stoppingToken); // 每秒检查一次
            }
        }

        private bool ShouldExecuteTask(TaskConfig task)
        {
            if (!task.LastExecutionTime.HasValue) return true;

            var timeSinceLastExecution = DateTime.Now - task.LastExecutionTime.Value;
            return timeSinceLastExecution.TotalSeconds >= task.IntervalSeconds;
        }

        private async Task ExecuteTaskAsync(TaskConfig task, TaskManagementService taskService)
        {
            try
            {
                _logger.LogInformation("开始执行任务: {TaskName}", task.Name);

                var client = _httpClientFactory.CreateClient("TaskExecutor");
                var url = task.Url.StartsWith("http") ? task.Url : $"{_baseUrl.TrimEnd('/')}/{task.Url.TrimStart('/')}";
                var request = new HttpRequestMessage(new HttpMethod(task.Method), url);

                _logger.LogInformation("请求URL: {Url}", url);

                if (!string.IsNullOrEmpty(task.Parameters))
                {
                    request.Content = new StringContent(task.Parameters, Encoding.UTF8, "application/json");
                }

                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation(
                    "任务执行完成: {TaskName}, 状态码: {StatusCode}, 响应: {Response}",
                    task.Name,
                    (int)response.StatusCode,
                    content);

                // TODO: 这里可以添加数据库记录逻辑
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行任务 {TaskName} 时发生错误", task.Name);
                // TODO: 这里可以添加数据库错误记录逻辑
            }
        }
    }
} 