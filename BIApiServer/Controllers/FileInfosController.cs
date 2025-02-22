
using BIApiServer.Exceptions;
using Microsoft.AspNetCore.Mvc;
using BIApiServer.Services;
using BIApiServer.Models;
using BIApiServer.Models.InputDto;
using BIApiServer.Utils;

namespace BIApiServer.Controllers
{
    /// <summary>
    /// FileInfos管理
    /// </summary>
    [Tags("FileInfos管理")]
    [ApiController]
    [Route("api/[controller]")]
    public class FileInfosController : ControllerBase
    {
        private readonly FileInfosService _service;
        private readonly ILogger<FileInfosController> _logger;

        public FileInfosController(FileInfosService service, ILogger<FileInfosController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// GET /api/fileinfos/getPageList?pageIndex=1&amp;pageSize=10
        /// </remarks>
        [HttpGet("getPageList")]
        [ProducesResponseType(typeof(List<FileInfos>), StatusCodes.Status200OK)]
        public async Task<List<FileInfos>> GetPageList([FromQuery] QueryBaseParameter param)
        {
            var response = await _service.GetPageListAsync(param);
            return response.Data;
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// GET /api/fileinfos/getById/1
        /// </remarks>
        [HttpGet("getById/{id}")]
        public async Task<FileInfos> GetById(long id)
        {
            return await _service.GetByIdAsync(id);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// POST /api/fileinfos/add
        /// </remarks>
        [HttpPost("add")]
        public async Task<bool> Add([FromBody] FileInfos entity)
        {
            return await _service.AddAsync(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// PUT /api/fileinfos/update
        /// </remarks>
        [HttpPut("update")]
        public async Task<bool> Update([FromBody] FileInfos entity)
        {
            return await _service.UpdateAsync(entity);
        }

        /// <summary>
        /// 删除（软删除）
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// DELETE /api/fileinfos/delete/1
        /// </remarks>
        [HttpDelete("delete/{id}")]
        public async Task<bool> Delete(long id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"ID为{id}的记录不存在");
            }
            
            entity.IsDeleted = true;
            return await _service.UpdateAsync(entity);
        }

        /// <summary>
        /// 批量删除（软删除）
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// DELETE /api/fileinfos/batchDelete
        /// </remarks>
        [HttpDelete("batchDelete")]
        public async Task<bool> BatchDelete([FromBody] long[] ids)
        {
            var entities = await _service.GetListAsync(it => ids.Contains(it.Id));
            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
            }
            return await _service.UpdateRangeAsync(entities);
        }
    }
}