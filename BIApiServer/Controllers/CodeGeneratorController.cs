using Microsoft.AspNetCore.Mvc;
using BIApiServer.Utils;
using System.IO;
using System.Threading.Tasks;

namespace BIApiServer.Controllers
{
    /// <summary>
    /// 代码生成器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CodeGeneratorController : ControllerBase
    {
        private readonly ILogger<CodeGeneratorController> _logger;
        private readonly IWebHostEnvironment _env;

        public CodeGeneratorController(ILogger<CodeGeneratorController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// 生成Controller和Service代码
        /// </summary>
        /// <param name="entityName">实体类名称</param>
        /// <param name="dbContextName">数据库上下文名称（默认BIDB）</param>
        /// <returns></returns>
        [HttpPost("generate")]
        public IActionResult GenerateCode([FromQuery] string entityName, [FromQuery] string dbContextName = "Default")
        {
            try
            {
                // 生成代码
                GenerateServiceFile(entityName);
                GenerateControllerFile(entityName);

                return Ok($"Successfully generated code for {entityName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成代码失败");
                return BadRequest($"Error generating code: {ex.Message}");
            }
        }

        private void GenerateServiceFile(string entityName)
        {
            var serviceContent = $@"
using BIApiServer.Common.DbContexts;
using BIApiServer.Interfaces;
using BIApiServer.Models;
using BIApiServer.Models.InputDto;
using BIApiServer.Utils;
using SqlSugar;

namespace BIApiServer.Services
{{
    /// <summary>
    /// {entityName}服务
    /// </summary>
    public class {entityName}Service : BaseService<{entityName}>
    {{
        public {entityName}Service(AppDbContext db, ILogger<{entityName}Service> logger) 
            : base(db, logger)
        {{
        }}

        #region 基础CRUD调用示例
        /// <summary>
        /// 分页查询示例
        /// </summary>
        public async Task<ApiResponse<List<{entityName}>>> GetPageListExample(QueryBaseParameter param)
        {{
            return await base.GetPageListAsync(param);
        }}

        /// <summary>
        /// 获取单个实体示例
        /// </summary>
        public async Task<{entityName}> GetByIdExample(int id)
        {{
            return await base.GetByIdAsync(id);
        }}

        /// <summary>
        /// 新增示例
        /// </summary>
        public async Task<bool> AddExample({entityName} entity)
        {{
            return await base.AddAsync(entity);
        }}

        /// <summary>
        /// 批量新增示例
        /// </summary>
        public async Task<bool> AddBatchExample(List<{entityName}> entities)
        {{
            return await base.AddBatchAsync(entities);
        }}

        /// <summary>
        /// 更新示例
        /// </summary>
        public async Task<bool> UpdateExample({entityName} entity)
        {{
            return await base.UpdateAsync(entity);
        }}

        /// <summary>
        /// 删除示例
        /// </summary>
        public async Task<bool> DeleteExample(int id)
        {{
            return await base.DeleteAsync(id);
        }}

        /// <summary>
        /// 批量删除示例
        /// </summary>
        public async Task<bool> DeleteBatchExample(int[] ids)
        {{
            return await base.DeleteBatchAsync(ids);
        }}
        #endregion
    }}
}}";

            var servicesPath = System.IO.Path.Combine(_env.ContentRootPath, "Services");
            System.IO.Directory.CreateDirectory(servicesPath);
            System.IO.File.WriteAllText(
                System.IO.Path.Combine(servicesPath, $"{entityName}Service.cs"), 
                serviceContent);
            
            _logger.LogInformation("Generated service file for entity: {EntityName}", entityName);
        }

        private void GenerateControllerFile(string entityName)
        {
            var controllerContent = $@"
using Microsoft.AspNetCore.Mvc;
using BIApiServer.Services;
using BIApiServer.Models;
using BIApiServer.Models.InputDto;
using BIApiServer.Utils;

namespace BIApiServer.Controllers
{{
    /// <summary>
    /// {entityName}管理
    /// </summary>
    [Tags(""{entityName}管理"")]
    [ApiController]
    [Route(""api/[controller]"")]
    public class {entityName}Controller : ControllerBase
    {{
        private readonly {entityName}Service _service;
        private readonly ILogger<{entityName}Controller> _logger;

        public {entityName}Controller({entityName}Service service, ILogger<{entityName}Controller> logger)
        {{
            _service = service;
            _logger = logger;
        }}

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// GET /api/{entityName.ToLower()}/list?pageIndex=1&amp;pageSize=10
        /// </remarks>
        [HttpGet(""list"")]
        [ProducesResponseType(typeof(List<{entityName}>), StatusCodes.Status200OK)]
        public async Task<List<{entityName}>> GetList([FromQuery] QueryBaseParameter param)
        {{
            var response = await _service.GetPageListAsync(param);
            Response.Headers.Add(""X-Total-Count"", response.Total.ToString());
            return response.Data;
        }}

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// GET /api/{entityName.ToLower()}/1
        /// </remarks>
        [HttpGet(""{{id}}"")]
        public async Task<{entityName}> Get(long id)
        {{
            return await _service.GetByIdAsync(id);
        }}

        /// <summary>
        /// 新增
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// POST /api/{entityName.ToLower()}
        /// </remarks>
        [HttpPost]
        public async Task<bool> Post([FromBody] {entityName} entity)
        {{
            return await _service.AddAsync(entity);
        }}

        /// <summary>
        /// 更新
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// PUT /api/{entityName.ToLower()}
        /// </remarks>
        [HttpPut]
        public async Task<bool> Put([FromBody] {entityName} entity)
        {{
            return await _service.UpdateAsync(entity);
        }}

        /// <summary>
        /// 删除
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// DELETE /api/{entityName.ToLower()}/1
        /// </remarks>
        [HttpDelete(""{{id}}"")]
        public async Task<bool> Delete(long id)
        {{
            return await _service.DeleteAsync(id);
        }}

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// DELETE /api/{entityName.ToLower()}/batch
        /// </remarks>
        [HttpDelete(""batch"")]
        public async Task<bool> BatchDelete([FromBody] long[] ids)
        {{
            return await _service.DeleteBatchAsync(ids);
        }}
    }}
}}";

            var controllersPath = System.IO.Path.Combine(_env.ContentRootPath, "Controllers");
            System.IO.Directory.CreateDirectory(controllersPath);
            System.IO.File.WriteAllText(
                System.IO.Path.Combine(controllersPath, $"{entityName}Controller.cs"), 
                controllerContent);
            
            _logger.LogInformation("Generated controller file for entity: {EntityName}", entityName);
        }
    }
} 