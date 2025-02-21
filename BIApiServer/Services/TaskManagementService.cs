using System.Text.Json;
using BIApiServer.Interfaces;
using BIApiServer.Models;

namespace BIApiServer.Services
{
    public class TaskManagementService : IScopedService
    {
        private readonly string _configPath;
        private readonly ILogger<TaskManagementService> _logger;

        public TaskManagementService(ILogger<TaskManagementService> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            
            // 直接使用程序根目录
            _configPath = Path.Combine(env.ContentRootPath, "taskconfig.json");

            // 如果文件不存在，创建一个空的 JSON 数组
            if (!File.Exists(_configPath))
            {
                File.WriteAllText(_configPath, "[]");
            }
        }

        public async Task<List<TaskConfig>> GetAllTasksAsync()
        {
            if (!File.Exists(_configPath))
            {
                return new List<TaskConfig>();
            }

            var json = await File.ReadAllTextAsync(_configPath);
            return JsonSerializer.Deserialize<List<TaskConfig>>(json) ?? new List<TaskConfig>();
        }

        public async Task<TaskConfig> AddTaskAsync(TaskConfig task)
        {
            var tasks = await GetAllTasksAsync();

            var existingTask = tasks.FirstOrDefault(t =>
                t.Url == task.Url &&
                t.Method == task.Method &&
                t.Parameters == task.Parameters);

            if (existingTask != null)
            {
                _logger.LogInformation("任务已存在，返回现有任务");
                return existingTask;
            }

            tasks.Add(task);
            await SaveTasksAsync(tasks);
            return task;
        }

        public async Task<bool> UpdateTaskAsync(string id, TaskConfig task)
        {
            var tasks = await GetAllTasksAsync();
            var index = tasks.FindIndex(t => t.Id == id);
            if (index == -1) return false;

            tasks[index] = task;
            await SaveTasksAsync(tasks);
            return true;
        }

        public async Task<bool> DeleteTaskAsync(string id)
        {
            var tasks = await GetAllTasksAsync();
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;

            tasks.Remove(task);
            await SaveTasksAsync(tasks);
            return true;
        }

        private async Task SaveTasksAsync(List<TaskConfig> tasks)
        {
            var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(_configPath, json);
        }
    }
}