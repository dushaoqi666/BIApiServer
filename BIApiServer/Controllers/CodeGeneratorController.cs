using Microsoft.AspNetCore.Mvc;
using BIApiServer.Utils;
using System.IO;
using System.Threading.Tasks;
using System.Linq.Expressions;
using BIApiServer.Exceptions;

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
using BIApiServer.Exceptions;
using SqlSugar;
using System.Linq.Expressions;

namespace BIApiServer.Services
{{
    /// <summary>
    /// {entityName}服务
    /// </summary>
    public class {entityName}Service : BaseService<{entityName}>
    {{
        private readonly AppDbContext _db;
        private readonly ILogger<{entityName}Service> _logger;

        public {entityName}Service(AppDbContext db, ILogger<{entityName}Service> logger) 
            : base(db, logger)
        {{
            _db = db;
            _logger = logger;
        }}

        #region 重写基础方法
        /// <summary>
        /// 获取分页列表
        /// </summary>
        public override async Task<ApiResponse<List<{entityName}>>> GetPageListAsync(QueryBaseParameter param)
        {{
            var response = new ApiResponse<List<{entityName}>>();
            try
            {{
                var query = base._dbClient.Queryable<{entityName}>()
                    .Where(it => !it.IsDeleted); // 显式添加软删除过滤
                
                // 添加查询条件
                if (!string.IsNullOrEmpty(param.Keyword))
                {{
                    query = query.Where(it => 
                        it.Name.Contains(param.Keyword)
                    );
                }}
                
                var total = await query.CountAsync();
                var data = await query
                    .OrderByDescending(it => it.CreateTime)
                    .ToPageListAsync(param.PageIndex, param.PageSize);

                response.Data = data;
                response.Total = total;
                return response;
            }}
            catch (Exception ex)
            {{
                _logger.LogError(ex, ""获取{entityName}列表失败"");
                throw new BIException(""获取列表失败："" + ex.Message);
            }}
        }}

        /// <summary>
        /// 获取单个实体
        /// </summary>
        public override async Task<{entityName}> GetByIdAsync(object id)
        {{
            try
            {{
                var entity = await base.GetByIdAsync(id);
                if (entity == null)
                {{
                    throw new NotFoundException($""ID为{{id}}的记录不存在"");
                }}
                return entity;
            }}
            catch (Exception ex)
            {{
                _logger.LogError(ex, ""获取{entityName}信息失败"");
                throw new BIException(""获取信息失败："" + ex.Message);
            }}
        }}

        /// <summary>
        /// 新增
        /// </summary>
        public override async Task<bool> AddAsync({entityName} entity)
        {{
            try
            {{
                // 业务验证
                if (entity == null)
                {{
                    throw new BusinessException(""数据不能为空"");
                }}

                return await base.AddAsync(entity);
            }}
            catch (Exception ex)
            {{
                _logger.LogError(ex, ""添加{entityName}失败"");
                throw new BIException(""添加失败："" + ex.Message);
            }}
        }}

        /// <summary>
        /// 更新
        /// </summary>
        public override async Task<bool> UpdateAsync({entityName} entity)
        {{
            try
            {{
                // 业务验证
                var oldEntity = await GetByIdAsync(entity.Id);
                if (oldEntity == null)
                {{
                    throw new NotFoundException($""ID为{{entity.Id}}的记录不存在"");
                }}

                return await base.UpdateAsync(entity);
            }}
            catch (Exception ex)
            {{
                _logger.LogError(ex, ""更新{entityName}失败"");
                throw new BIException(""更新失败："" + ex.Message);
            }}
        }}

        /// <summary>
        /// 删除
        /// </summary>
        public override async Task<bool> DeleteAsync(object id)
        {{
            try
            {{
                var entity = await GetByIdAsync(id);
                if (entity == null)
                {{
                    throw new NotFoundException($""ID为{{id}}的记录不存在"");
                }}

                return await base.DeleteAsync(id);
            }}
            catch (Exception ex)
            {{
                _logger.LogError(ex, ""删除{entityName}失败"");
                throw new BIException(""删除失败："" + ex.Message);
            }}
        }}
        #endregion

        #region 自定义方法
        // 可以在这里添加自定义业务方法
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
using BIApiServer.Exceptions;

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
        /// GET /api/{entityName.ToLower()}/getPageList?pageIndex=1&amp;pageSize=10
        /// </remarks>
        [HttpGet(""getPageList"")]
        [ProducesResponseType(typeof(List<{entityName}>), StatusCodes.Status200OK)]
        public async Task<List<{entityName}>> GetPageList([FromQuery] QueryBaseParameter param)
        {{
            var response = await _service.GetPageListAsync(param);
            return response.Data;
        }}

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// GET /api/{entityName.ToLower()}/getById/1
        /// </remarks>
        [HttpGet(""getById/{{id}}"")]
        public async Task<{entityName}> GetById(long id)
        {{
            return await _service.GetByIdAsync(id);
        }}

        /// <summary>
        /// 新增
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// POST /api/{entityName.ToLower()}/add
        /// </remarks>
        [HttpPost(""add"")]
        public async Task<bool> Add([FromBody] {entityName} entity)
        {{
            return await _service.AddAsync(entity);
        }}

        /// <summary>
        /// 更新
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// PUT /api/{entityName.ToLower()}/update
        /// </remarks>
        [HttpPut(""update"")]
        public async Task<bool> Update([FromBody] {entityName} entity)
        {{
            return await _service.UpdateAsync(entity);
        }}

        /// <summary>
        /// 删除（软删除）
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// DELETE /api/{entityName.ToLower()}/delete/1
        /// </remarks>
        [HttpDelete(""delete/{{id}}"")]
        public async Task<bool> Delete(long id)
        {{
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
            {{
                throw new NotFoundException($""ID为{{id}}的记录不存在"");
            }}
            
            entity.IsDeleted = true;
            return await _service.UpdateAsync(entity);
        }}

        /// <summary>
        /// 批量删除（软删除）
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// DELETE /api/{entityName.ToLower()}/batchDelete
        /// </remarks>
        [HttpDelete(""batchDelete"")]
        public async Task<bool> BatchDelete([FromBody] long[] ids)
        {{
            var entities = await _service.GetListAsync(it => ids.Contains(it.Id));
            foreach (var entity in entities)
            {{
                entity.IsDeleted = true;
            }}
            return await _service.UpdateRangeAsync(entities);
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