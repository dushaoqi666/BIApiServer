using System.Linq.Expressions;
using BIApiServer.Common.DbContexts;
using BIApiServer.Exceptions;
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
        /// 获取查询对象（带软删除过滤）
        /// </summary>
        protected ISugarQueryable<T> GetQuery()
        {
            var query = _dbClient.Queryable<T>();
            if (typeof(IDeletedFilter).IsAssignableFrom(typeof(T)))
            {
                query = query.Where("is_deleted = @isDeleted", new { isDeleted = false });
            }
            return query;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        public virtual async Task<ApiResponse<List<T>>> GetPageListAsync(QueryBaseParameter param)
        {
            var response = new ApiResponse<List<T>>();
            try
            {
                var query = GetQuery();
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
            return await GetQuery().InSingleAsync(id);
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
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException($"ID为{id}的记录不存在");
            }
            
            if (entity is IDeletedFilter softDelete)
            {
                softDelete.IsDeleted = true;
                return await UpdateAsync(entity);
            }
            
            return await _dbClient.Deleteable<T>().In(id).ExecuteCommandAsync() > 0;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        public virtual async Task<bool> DeleteBatchAsync<TKey>(TKey[] ids)
        {
            var entities = await GetListAsync(it => ids.Contains((TKey)it.GetType().GetProperty("Id").GetValue(it)));
            foreach (var entity in entities)
            {
                if (entity is IDeletedFilter softDelete)
                {
                    softDelete.IsDeleted = true;
                }
            }
            return await UpdateRangeAsync(entities);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        public virtual async Task<bool> UpdateRangeAsync(IEnumerable<T> entities)
        {
            // 转换为 List<T>
            var entityList = entities.ToList();
            return await _dbClient.Updateable(entityList).ExecuteCommandAsync() > 0;
        }

        /// <summary>
        /// 获取列表（根据条件）
        /// </summary>
        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await GetQuery()
                .Where(whereExpression)
                .ToListAsync();
        }
    }
} 