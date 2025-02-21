using Microsoft.AspNetCore.Mvc;
using BIApiServer.Models;
using BIApiServer.Services;

namespace BIApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("任务管理")]
    public class TaskManagementController : ControllerBase
    {
        private readonly TaskManagementService _taskService;
        private readonly ILogger<TaskManagementController> _logger;

        public TaskManagementController(
            TaskManagementService taskService,
            ILogger<TaskManagementController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskConfig>>> GetAllTasks()
        {
            return Ok(await _taskService.GetAllTasksAsync());
        }

        /// <summary>
        /// 添加新的定时任务
        /// </summary>
        /// <param name="task">任务配置</param>
        /// <returns>创建的任务信息</returns>
        /// <remarks>
        /// 示例请求:
        /// ```json
        /// {
        ///   "name": "定时获取文件列表",
        ///   "url": "/api/file/list",
        ///   "method": "GET",
        ///   "parameters": "{ \"pageIndex\": 1, \"pageSize\": 10 }",
        ///   "intervalSeconds": 5,
        ///   "isEnabled": true
        /// }
        /// ```
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(TaskConfig), StatusCodes.Status201Created)]
        public async Task<ActionResult<TaskConfig>> AddTask([FromBody] TaskConfig task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _taskService.AddTaskAsync(task);
            return CreatedAtAction(nameof(GetAllTasks), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(string id, TaskConfig task)
        {
            var result = await _taskService.UpdateTaskAsync(id, task);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        
        // [HttpGet("history/{taskId}")]
        // public async Task<ActionResult<IEnumerable<TaskExecutionHistory>>> GetTaskHistory(string taskId)
        // {
        //     // 实现获取任务执行历史的逻辑
        //     return Ok(await _taskService.GetTaskHistoryAsync(taskId));
        // }
    }
} 