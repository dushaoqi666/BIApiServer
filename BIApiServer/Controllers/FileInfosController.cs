
using Microsoft.AspNetCore.Mvc;
using BIApiServer.Services;
using BIApiServer.Models;
using BIApiServer.Models.InputDto;
using BIApiServer.Utils;
using BIApiServer.Exceptions;

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
        [HttpGet("getPageList")]
        public async Task<IActionResult> GetPageList([FromQuery] QueryBaseParameter param)
        {
            var response = await _service.GetPageListAsync(param);
            return Ok(response.Data);
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        /// <summary>
        /// 新增
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] FileInfos entity)
        {
            return Ok(await _service.AddAsync(entity));
        }

        /// <summary>
        /// 更新
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] FileInfos entity)
        {
            return Ok(await _service.UpdateAsync(entity));
        }

        /// <summary>
        /// 删除（软删除）
        /// </summary>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"ID为{id}的记录不存在");
            }

            entity.IsDeleted = true;
            return Ok(await _service.UpdateAsync(entity));
        }
    }
}