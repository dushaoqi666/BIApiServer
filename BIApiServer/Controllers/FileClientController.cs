using BIApiServer.Models;
using BIApiServer.Models.InputDto;
using BIApiServer.Services;
using BIApiServer.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BIApiServer.Controllers
{
    [Route("api/file-client")]
    [ApiController]
    public class FileClientController : ControllerBase
    {
        private readonly FileClientService _fileClientService;

        public FileClientController(FileClientService fileClientService)
        {
            _fileClientService = fileClientService;
        }

        [HttpGet("list")]
        public async Task<PagedApiResponse<List<FileInfoDto>>> GetFileList([FromQuery] QueryBaseParameter param)
        {
            return await _fileClientService.GetFileListAsync(param);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(long id)
        {
            await _fileClientService.DeleteFileAsync(id);
            return Ok();
        }
    }
} 