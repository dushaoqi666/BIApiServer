using System.Collections.Concurrent;
using System.Text;
using BIApiServer.Models;
using Microsoft.AspNetCore.Antiforgery;

namespace BIApiServer.Services
{
    /// <summary>
    /// 后台任务执行服务
    /// 负责管理和执行定时任务，支持并发控制和任务重叠检测
    /// </summary>
    public class TaskExecutorService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TaskExecutorService> _logger;
        private readonly string _baseUrl;
        private readonly IWebHostEnvironment _environment;
        
        /// <summary>
        /// 用于跟踪正在执行的任务及其执行信息
        /// </summary>
        private readonly ConcurrentDictionary<string, TaskExecutionInfo> _runningTasks;
        
        /// <summary>
        /// 用于控制并发任务数的信号量
        /// </summary>
        private readonly SemaphoreSlim _semaphore;

        /// <summary>
        /// 任务执行信息类，用于跟踪任务的执行状态
        /// </summary>
        private class TaskExecutionInfo
        {
            public Task Task { get; set; }                           // 任务本身
            public DateTime StartTime { get; set; }                  // 任务开始时间
            public DateTime NextAllowedExecutionTime { get; set; }   // 下次允许记录日志的时间
        }

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
            _baseUrl = configuration["ApiSettings:ApiBaseUrlOne"] ?? "https://localhost:5166";
            _environment = environment;
            _runningTasks = new ConcurrentDictionary<string, TaskExecutionInfo>();
            
            // 从配置中读取最大并发数，默认为10
            var maxConcurrent = configuration.GetValue<int>("TaskExecutor:MaxConcurrent", 10);
            _semaphore = new SemaphoreSlim(maxConcurrent);
        }

        /// <summary>
        /// 服务的主执行循环
        /// </summary>
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
                        
                        // 筛选出需要执行的任务并异步执行
                        var executionTasks = tasks
                            .Where(t => t.IsEnabled && ShouldExecuteTask(t))
                            .Select(task => ProcessTaskAsync(task, taskService, stoppingToken));

                        // 异步执行所有任务但不阻塞主循环
                        _ = Task.WhenAll(executionTasks);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "执行任务循环时发生错误");
                }

                // 每秒检查一次任务
                await Task.Delay(1000, stoppingToken);
            }
        }

        /// <summary>
        /// 处理单个任务的执行
        /// </summary>
        private async Task ProcessTaskAsync(TaskConfig task, TaskManagementService taskService, CancellationToken stoppingToken)
        {
            // 检查任务是否已在运行
            if (_runningTasks.TryGetValue(task.Id, out var executionInfo))
            {
                // 每60秒记录一次日志，避免日志刷屏
                if (DateTime.Now >= executionInfo.NextAllowedExecutionTime)
                {
                    var runningTime = (DateTime.Now - executionInfo.StartTime).TotalMinutes;
                    _logger.LogInformation(
                        "任务 {TaskName} (ID: {TaskId}) 正在执行中，已运行 {Minutes} 分钟，跳过本次执行",
                        task.Name,
                        task.Id,
                        runningTime.ToString("F1"));

                    // 更新下次允许记录日志的时间
                    executionInfo.NextAllowedExecutionTime = DateTime.Now.AddSeconds(60);
                }
                return;
            }

            try
            {
                // 等待获取执行许可
                await _semaphore.WaitAsync(stoppingToken);

                var executionTask = ExecuteTaskWithTimeoutAsync(task, taskService, stoppingToken);
                var taskInfo = new TaskExecutionInfo
                {
                    Task = executionTask,
                    StartTime = DateTime.Now,
                    NextAllowedExecutionTime = DateTime.Now.AddSeconds(60)
                };

                if (_runningTasks.TryAdd(task.Id, taskInfo))
                {
                    await executionTask;
                }
            }
            finally
            {
                _runningTasks.TryRemove(task.Id, out _);
                _semaphore.Release();
            }
        }

        /// <summary>
        /// 执行带超时控制的任务
        /// </summary>
        private async Task ExecuteTaskWithTimeoutAsync(TaskConfig task, TaskManagementService taskService, CancellationToken stoppingToken)
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            // 设置超时时间为任务间隔的80%
            var timeout = TimeSpan.FromSeconds(task.IntervalSeconds * 0.8);
            timeoutCts.CancelAfter(timeout);

            try
            {
                _logger.LogInformation(
                    "开始执行任务: {TaskName} (ID: {TaskId})，预计执行时间：{Timeout}秒",
                    task.Name,
                    task.Id,
                    timeout.TotalSeconds);

                await ExecuteTaskAsync(task, taskService);
                
                // 更新任务的最后执行时间
                task.LastExecutionTime = DateTime.Now;
                try
                {
                    await taskService.UpdateTaskAsync(task.Id, task);
                }
                catch (AntiforgeryValidationException ex)
                {
                    _logger.LogWarning(ex, "更新任务时发生验证错误: {TaskId}", task.Id);
                }

                _logger.LogInformation(
                    "任务执行完成: {TaskName} (ID: {TaskId})，实际执行时间：{Duration}秒",
                    task.Name,
                    task.Id,
                    (DateTime.Now - _runningTasks[task.Id].StartTime).TotalSeconds.ToString("F1"));
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                _logger.LogWarning(
                    "任务执行超时: {TaskName} (ID: {TaskId})，已执行：{Duration}秒",
                    task.Name,
                    task.Id,
                    (DateTime.Now - _runningTasks[task.Id].StartTime).TotalSeconds.ToString("F1"));
            }
        }

        /// <summary>
        /// 判断任务是否应该执行
        /// </summary>
        private bool ShouldExecuteTask(TaskConfig task)
        {
            if (!task.LastExecutionTime.HasValue) return true;

            var timeSinceLastExecution = DateTime.Now - task.LastExecutionTime.Value;
            return timeSinceLastExecution.TotalSeconds >= task.IntervalSeconds;
        }

        /// <summary>
        /// 执行具体的HTTP任务
        /// </summary>
        private async Task ExecuteTaskAsync(TaskConfig task, TaskManagementService taskService)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient("TaskExecutor");
                var url = task.Url.StartsWith("http") ? task.Url : $"{_baseUrl.TrimEnd('/')}/{task.Url.TrimStart('/')}";
                using var request = new HttpRequestMessage(new HttpMethod(task.Method), url);

                _logger.LogInformation("请求URL: {Url}", url);

                if (!string.IsNullOrEmpty(task.Parameters))
                {
                    request.Content = new StringContent(task.Parameters, Encoding.UTF8, "application/json");
                }

                using var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "任务执行返回非成功状态码: {TaskName} (ID: {TaskId}), 状态码: {StatusCode}, 响应: {Response}",
                        task.Name,
                        task.Id,
                        (int)response.StatusCode,
                        content);
                    return;
                }

                _logger.LogInformation(
                    "任务执行成功: {TaskName} (ID: {TaskId}), 状态码: {StatusCode}, 响应: {Response}",
                    task.Name,
                    task.Id,
                    (int)response.StatusCode,
                    content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行任务 {TaskName} (ID: {TaskId}) 时发生错误", task.Name, task.Id);
                throw;
            }
        }

        /// <summary>
        /// 服务停止时等待所有任务完成
        /// </summary>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            var runningTasks = _runningTasks.Values.Select(info => info.Task).ToList();
            if (runningTasks.Any())
            {
                _logger.LogInformation("等待所有正在运行的任务完成...");
                await Task.WhenAll(runningTasks);
            }

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _semaphore.Dispose();
            base.Dispose();
        }
    }
}