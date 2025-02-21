using BIApiServer.Models;
using BIApiServer.Models.InputDto;
using SqlSugar;
using BIApiServer.Exceptions;
using AutoMapper;
using BIApiServer.Models.Dtos;
using BIApiServer.Common.DbContexts;
using BIApiServer.Interfaces;

namespace BIApiServer.Services
{
    public class FileService : IScopedService
    {
        private readonly AppDbContext _db;
        private readonly IRedisService _redisService;
        private const string FILE_LIST_CACHE_KEY = "file:list:";
        private readonly ILogger<FileService> _logger;
        private readonly IMapper _mapper;

        public FileService(
            AppDbContext db,
            IRedisService redisService,
            ILogger<FileService> logger,
            IMapper mapper)
        {
            _db = db;
            _redisService = redisService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<List<FileInfoDto>>> GetFileListAsync(QueryBaseParameter param)
        {
            var response = new PagedApiResponse<List<FileInfoDto>>();

            try
            {
                // 参数验证
                if (param.PageSize > 100)
                {
                    throw new BusinessException("每页记录数不能超过100");
                }

                // 尝试从缓存获取数据
                var cacheKey = $"{FILE_LIST_CACHE_KEY}{param.PageIndex}:{param.PageSize}";
                var cachedResponse = await _redisService.GetObjectAsync<PagedApiResponse<List<FileInfoDto>>>(cacheKey);

                if (cachedResponse != null)
                {
                    return cachedResponse;
                }

                var query = _db.Default.Queryable<FileInfos>();
                var total = await query.CountAsync();

                if (total == 0)
                {
                    throw new NotFoundException("没有找到任何文件记录");
                }

                var data = await query
                    .OrderBy(it => it.CreateTime, OrderByType.Desc)
                    .ToPageListAsync(param.PageIndex, param.PageSize);

                response.Data = _mapper.Map<List<FileInfoDto>>(data);
                response.Total = total;

                await _redisService.SetObjectAsync(cacheKey, response, TimeSpan.FromMinutes(5));
                return response;
            }
            catch (BIException)
            {
                throw; // 直接抛出业务异常
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取文件列表失败");
                throw new BIException("获取文件列表失败：" + ex.Message);
            }
        }

        public async Task DeleteFileAsync(long fileId)
        {
            var file = await _db.Default.Queryable<FileInfos>()
                .FirstAsync(f => f.Id == fileId);

            if (file == null)
            {
                throw new NotFoundException($"ID为{fileId}的文件不存在");
            }


            // 执行删除操作
            // ...
        }

        // 如果需要查询包括已删除的记录
        public async Task<List<FileInfos>> GetAllIncludeDeletedAsync()
        {
            return await _db.Default.Queryable<FileInfos>()
                .Includes(x => x.IsDeleted) // 包含已删除的记录
                .ToListAsync();
        }

        // 软删除文件
        public async Task SoftDeleteAsync(long fileId)
        {
            var file = await _db.Default.Queryable<FileInfos>()
                .FirstAsync(f => f.Id == fileId);

            if (file == null)
            {
                throw new NotFoundException($"ID为{fileId}的文件不存在");
            }

            file.IsDeleted = true;
            await _db.Default.Updateable(file).ExecuteCommandAsync();
        }
    }
}