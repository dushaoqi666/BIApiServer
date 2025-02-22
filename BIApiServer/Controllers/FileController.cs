using BIApiServer.Models;
using BIApiServer.Models.InputDto;
using BIApiServer.Services;
using BIApiServer.Models.Dtos;
using BIApiServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace BIApiServer.Controllers
{
    /// <summary>
    /// 文件管理
    /// </summary>
    [Tags("文件管理")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public class FileController : ControllerBase
    {
        private readonly FileService _fileService;

        public FileController(FileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="param">查询参数</param>
        /// <returns>分页的文件列表数据</returns>
        /// <remarks>
        /// 示例请求:
        /// ```
        /// GET /api/file/list?pageIndex=1&pageSize=10
        /// ```
        /// </remarks>
        /// <response code="200">成功获取文件列表</response>
        /// <response code="400">请求参数无效</response>
        /// <response code="404">未找到任何文件</response>
        [ProducesResponseType(typeof(ApiResponse<List<FileInfoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [HttpGet("list")]
        public async Task<ApiResponse<List<FileInfoDto>>> GetFileList([FromQuery] QueryBaseParameter param)
        {
            return await _fileService.GetFileListAsync(param);
        }
    }
} 