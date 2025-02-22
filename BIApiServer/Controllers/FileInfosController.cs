
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
        /// GET /api/fileinfos/list?pageIndex=1&amp;pageSize=10
        /// </remarks>
        [HttpGet("list")]
        [ProducesResponseType(typeof(List<FileInfos>), StatusCodes.Status200OK)]
        public async Task<List<FileInfos>> GetList([FromQuery] QueryBaseParameter param)
        {
            var response = await _service.GetPageListAsync(param);
            Response.Headers.Add("X-Total-Count", response.Total.ToString());
            return response.Data;
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// GET /api/fileinfos/1
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<FileInfos> Get(long id)
        {
            return await _service.GetByIdAsync(id);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// POST /api/fileinfos
        /// </remarks>
        [HttpPost]
        public async Task<bool> Post([FromBody] FileInfos entity)
        {
            return await _service.AddAsync(entity);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// PUT /api/fileinfos
        /// </remarks>
        [HttpPut]
        public async Task<bool> Put([FromBody] FileInfos entity)
        {
            return await _service.UpdateAsync(entity);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// DELETE /api/fileinfos/1
        /// </remarks>
        [HttpDelete("{id}")]
        public async Task<bool> Delete(long id)
        {
            return await _service.DeleteAsync(id);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <remarks>
        /// 示例请求:
        /// DELETE /api/fileinfos/batch
        /// </remarks>
        [HttpDelete("batch")]
        public async Task<bool> BatchDelete([FromBody] long[] ids)
        {
            return await _service.DeleteBatchAsync(ids);
        }
    }
}