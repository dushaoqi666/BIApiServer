using BIApiServer.Exceptions;
using BIApiServer.Interfaces;
using BIApiServer.Models;
using BIApiServer.Models.InputDto;
using BIApiServer.Models.Dtos;
using Refit;

namespace BIApiServer.Services
{
    public class FileClientService
    {
        private readonly IFileApi _fileApi;

        public FileClientService(IFileApi fileApi)
        {
            _fileApi = fileApi;
        }

        public async Task<ApiResponse<List<FileInfoDto>>> GetFileListAsync(QueryBaseParameter param)
        {
            try
            {
                return await _fileApi.GetFileList(param);
            }
            catch (ApiException ex)
            {
                // Refit 会将非成功的 HTTP 响应包装为 ApiException
                throw new BusinessException($"获取文件列表失败: {ex.Message}");
            }
        }

        public async Task DeleteFileAsync(long fileId)
        {
            try
            {
                await _fileApi.DeleteFile(fileId);
            }
            catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new NotFoundException($"ID为{fileId}的文件不存在");
            }
            catch (ApiException ex)
            {
                throw new BusinessException($"删除文件失败: {ex.Message}");
            }
        }
    }
} 