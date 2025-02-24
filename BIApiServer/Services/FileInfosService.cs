
using BIApiServer.Common.DbContexts;
using BIApiServer.Interfaces;
using BIApiServer.Models;
using BIApiServer.Models.InputDto;
using BIApiServer.Utils;
using BIApiServer.Exceptions;
using SqlSugar;
using System.Linq.Expressions;

namespace BIApiServer.Services
{
    /// <summary>
    /// FileInfos服务
    /// </summary>
    public class FileInfosService : BaseService<FileInfos>
    {
        private readonly AppDbContext _db;
        private readonly ILogger<FileInfosService> _logger;

        public FileInfosService(AppDbContext db, ILogger<FileInfosService> logger) 
            : base(db, logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        public override async Task<ApiResponse<List<FileInfos>>> GetPageListAsync(QueryBaseParameter param)
        {
            var response = new ApiResponse<List<FileInfos>>();
            try
            {
                var query = base._dbClient.Queryable<FileInfos>()
                    .Where(it => !it.IsDeleted); // 显式添加软删除过滤

                if (!string.IsNullOrEmpty(param.Keyword))
                {
                    query = query.Where(it => it.Name.Contains(param.Keyword));
                }

                var total = await query.CountAsync();
                var data = await query
                    .OrderByDescending(it => it.CreateTime)
                    .ToPageListAsync(param.PageIndex, param.PageSize);

                response.Data = data;
                response.Total = total;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取FileInfos列表失败");
                throw new BIException("获取列表失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        public override async Task<FileInfos> GetByIdAsync(object id)
        {
            try
            {
                var entity = await base.GetByIdAsync(id);
                if (entity == null)
                {
                    throw new NotFoundException($"ID为{id}的记录不存在");
                }
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取FileInfos信息失败");
                throw new BIException("获取信息失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        public override async Task<bool> AddAsync(FileInfos entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new BusinessException("数据不能为空");
                }

                return await base.AddAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加FileInfos失败");
                throw new BIException("添加失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public override async Task<bool> UpdateAsync(FileInfos entity)
        {
            try
            {
                var oldEntity = await GetByIdAsync(entity.Id);
                if (oldEntity == null)
                {
                    throw new NotFoundException($"ID为{entity.Id}的记录不存在");
                }

                return await base.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新FileInfos失败");
                throw new BIException("更新失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public override async Task<bool> DeleteAsync(object id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity == null)
                {
                    throw new NotFoundException($"ID为{id}的记录不存在");
                }

                return await base.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除FileInfos失败");
                throw new BIException("删除失败：" + ex.Message);
            }
        }
    }
}