using System.Linq.Expressions;
using BIApiServer.Common.DbContexts;
using BIApiServer.Interfaces;
using BIApiServer.Models.InputDto;
using BIApiServer.Models.Interfaces;
using BIApiServer.Utils;
using SqlSugar;

namespace BIApiServer.Services
{
    /// <summary>
    /// 基础服务类
    /// </summary>
    public class BaseService<T> : IScopedService where T : class, new()
    {
        protected readonly AppDbContext _db;
        protected readonly ILogger<BaseService<T>> _logger;
        protected readonly ISqlSugarClient _dbClient;

        public BaseService(AppDbContext db, ILogger<BaseService<T>> logger)
        {
            _db = db;
            _logger = logger;
            _dbClient = _db.Default;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        public virtual async Task<ApiResponse<List<T>>> GetPageListAsync(QueryBaseParameter param)
        {
            var response = new ApiResponse<List<T>>();
            try
            {
                var query = _dbClient.Queryable<T>();
                var total = await query.CountAsync();

                var data = await query
                    .OrderByDescending(GetOrderByExpression())
                    .ToPageListAsync(param.PageIndex, param.PageSize);

                response.Data = data;
                response.Total = total;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取{EntityName}列表失败", typeof(T).Name);
                throw;
            }
        }

        /// <summary>
        /// 获取默认排序表达式
        /// </summary>
        protected virtual Expression<Func<T, object>> GetOrderByExpression()
        {
            // 获取实体类型的所有属性
            var properties = typeof(T).GetProperties();
            
            // 尝试查找CreateTime属性
            var createTimeProperty = properties.FirstOrDefault(p => 
                p.Name.Equals("CreateTime", StringComparison.OrdinalIgnoreCase));
            
            if (createTimeProperty != null)
            {
                // 创建参数表达式 x =>
                var parameter = Expression.Parameter(typeof(T), "x");
                // 创建属性访问表达式 x.CreateTime
                var property = Expression.Property(parameter, createTimeProperty);
                // 创建转换表达式，处理可能的值类型到object的转换
                var conversion = Expression.Convert(property, typeof(object));
                // 组合成完整的lambda表达式 x => (object)x.CreateTime
                return Expression.Lambda<Func<T, object>>(conversion, parameter);
            }
            
            // 如果没有CreateTime属性，则使用Id属性
            var idProperty = properties.FirstOrDefault(p => 
                p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            
            if (idProperty != null)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, idProperty);
                var conversion = Expression.Convert(property, typeof(object));
                return Expression.Lambda<Func<T, object>>(conversion, parameter);
            }
            
            // 如果既没有CreateTime也没有Id属性，则使用第一个属性
            var firstProperty = properties.First();
            var param = Expression.Parameter(typeof(T), "x");
            var prop = Expression.Property(param, firstProperty);
            var conv = Expression.Convert(prop, typeof(object));
            return Expression.Lambda<Func<T, object>>(conv, param);
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        public virtual async Task<T> GetByIdAsync(object id)
        {
            return await _dbClient.Queryable<T>().InSingleAsync(id);
        }

        /// <summary>
        /// 新增
        /// </summary>
        public virtual async Task<bool> AddAsync(T entity)
        {
            return await _dbClient.Insertable(entity).ExecuteCommandAsync() > 0;
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        public virtual async Task<bool> AddBatchAsync(List<T> entities)
        {
            try
            {
                return await _dbClient.Insertable(entities).ExecuteCommandAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "批量新增{EntityName}失败", typeof(T).Name);
                throw;
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        public virtual async Task<bool> UpdateAsync(T entity)
        {
            return await _dbClient.Updateable(entity).ExecuteCommandAsync() > 0;
        }

        /// <summary>
        /// 删除
        /// </summary>
        public virtual async Task<bool> DeleteAsync(object id)
        {
            try
            {
                return await _dbClient.Deleteable<T>().In(id).ExecuteCommandAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除{EntityName}失败", typeof(T).Name);
                throw;
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        public virtual async Task<bool> DeleteBatchAsync<TKey>(TKey[] ids)
        {
            try
            {
                return await _dbClient.Deleteable<T>().In(ids).ExecuteCommandAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "批量删除{EntityName}失败", typeof(T).Name);
                throw;
            }
        }
    }
} 