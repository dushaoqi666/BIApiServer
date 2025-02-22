using Refit;
using BIApiServer.Models;
using BIApiServer.Models.InputDto;
using BIApiServer.Models.Dtos;

namespace BIApiServer.Interfaces
{
    public interface IFileApi
    {
        [Get("/api/file/list")]
        Task<ApiResponse<List<FileInfoDto>>> GetFileList([Query] QueryBaseParameter param);

        [Delete("/api/file/{id}")]
        Task DeleteFile(long id);

        // 可以添加更多API定义
    }
} 