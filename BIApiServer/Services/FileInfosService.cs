
using BIApiServer.Common.DbContexts;
using BIApiServer.Interfaces;
using BIApiServer.Models;
using BIApiServer.Models.InputDto;
using BIApiServer.Utils;
using SqlSugar;

namespace BIApiServer.Services
{
    /// <summary>
    /// FileInfos服务
    /// </summary>
    public class FileInfosService : BaseService<FileInfos>
    {
        public FileInfosService(AppDbContext db, ILogger<FileInfosService> logger) 
            : base(db, logger)
        {
        }

        #region 基础CRUD调用示例
        /// <summary>
        /// 分页查询示例
        /// </summary>
        public async Task<ApiResponse<List<FileInfos>>> GetPageListExample(QueryBaseParameter param)
        {
            return await base.GetPageListAsync(param);
        }

        /// <summary>
        /// 获取单个实体示例
        /// </summary>
        public async Task<FileInfos> GetByIdExample(int id)
        {
            return await base.GetByIdAsync(id);
        }

        /// <summary>
        /// 新增示例
        /// </summary>
        public async Task<bool> AddExample(FileInfos entity)
        {
            return await base.AddAsync(entity);
        }

        /// <summary>
        /// 批量新增示例
        /// </summary>
        public async Task<bool> AddBatchExample(List<FileInfos> entities)
        {
            return await base.AddBatchAsync(entities);
        }

        /// <summary>
        /// 更新示例
        /// </summary>
        public async Task<bool> UpdateExample(FileInfos entity)
        {
            return await base.UpdateAsync(entity);
        }

        /// <summary>
        /// 删除示例
        /// </summary>
        public async Task<bool> DeleteExample(int id)
        {
            return await base.DeleteAsync(id);
        }

        /// <summary>
        /// 批量删除示例
        /// </summary>
        public async Task<bool> DeleteBatchExample(int[] ids)
        {
            return await base.DeleteBatchAsync(ids);
        }
        #endregion
    }
}