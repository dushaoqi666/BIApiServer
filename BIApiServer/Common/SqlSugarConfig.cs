// 引入SqlSugar库

using SqlSugar;
// 引入接口模型
using BIApiServer.Models.Interfaces;

// 定义命名空间
namespace BIApiServer.Common
{
    // 定义静态类SqlSugarConfig
    public static class SqlSugarConfig
    {
        // 定义SqlSugarScope实例
        private static SqlSugarScope _db;

        // 定义锁对象以确保线程安全
        private static readonly object _lock = new object();

        // 定义配置接口
        private static IConfiguration _configuration;

        // 初始化方法，接收IConfiguration参数
        public static void Initialize(IConfiguration configuration)
        {
            // 设置配置对象
            _configuration = configuration;
            // 获取数据库类型
            var dbType = configuration["Database:Type"];
            // 获取连接字符串配置
            var connectionStrings = configuration.GetSection($"Database:ConnectionStrings:{dbType}")
                .Get<Dictionary<string, string>>();

            // 创建连接配置列表
            var connectionConfigs = connectionStrings.Select(conn => new ConnectionConfig()
            {
                // 配置ID
                ConfigId = conn.Key,
                // 配置连接字符串
                ConnectionString = conn.Value,
                // 获取数据库类型
                DbType = GetDbType(dbType),
                // 自动关闭连接
                IsAutoCloseConnection = true,
                // 初始化主键类型
                InitKeyType = InitKeyType.Attribute,
                // AOP事件配置
                AopEvents = new AopEvents
                {
                    // SQL执行日志事件
                    OnLogExecuting = (sql, parameters) =>
                    {
                        // 设置控制台字体颜色为绿色
                        Console.ForegroundColor = ConsoleColor.Green;
                        // 输出SQL日志
                        Console.WriteLine($@"
==================== SQL Log ====================
Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
SQL: {sql}
Parameters: {string.Join(", ", parameters.Select(p => $"{p.ParameterName}={p.Value}"))}
===============================================");
                        // 重置控制台字体颜色
                        Console.ResetColor();
                    },
                    // SQL错误事件
                    OnError = (exp) =>
                    {
                        // 设置控制台字体颜色为红色
                        Console.ForegroundColor = ConsoleColor.Red;
                        // 输出SQL错误信息
                        Console.WriteLine($@"
==================== SQL Error ====================
Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
Error: {exp.Message}
SQL: {exp.Sql}
================================================");
                        // 重置控制台字体颜色
                        Console.ResetColor();
                    },
                    // 数据执行事件
                    DataExecuting = (oldValue, entityInfo) =>
                    {
                        // 检查实体是否为自动生成类型
                        if (entityInfo.EntityValue is IAutoGenerated entity)
                        {
                            // 根据操作类型处理数据
                            switch (entityInfo.OperationType)
                            {
                                // 插入操作
                                case DataFilterType.InsertByObject:
                                    // 设置创建时间
                                    if (entityInfo.PropertyName == "CreateTime")
                                    {
                                        entityInfo.SetValue(DateTime.Now);
                                    }

                                    // 设置更新时间
                                    if (entityInfo.PropertyName == "UpdateTime")
                                    {
                                        entityInfo.SetValue(DateTime.Now);
                                    }

                                    // 设置删除标志
                                    if (entityInfo.PropertyName == "IsDeleted")
                                    {
                                        entityInfo.SetValue(false);
                                    }

                                    break;
                                // 更新操作
                                case DataFilterType.UpdateByObject:
                                    // 设置更新时间
                                    if (entityInfo.PropertyName == "UpdateTime")
                                    {
                                        entityInfo.SetValue(DateTime.Now);
                                    }

                                    break;
                            }
                        }
                    }
                }
            }).ToList();

            // 创建SqlSugarScope实例
            _db = new SqlSugarScope(connectionConfigs);

            // 全局设置参数
            _db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices
            {
                EntityService = (property, column) =>
                {
                    if (property.PropertyType.IsEnum)
                    {
                        column.DataType = "int";
                    }
                }
            };
        }

        // 获取数据库类型的方法
        private static DbType GetDbType(string dbType)
        {
            // 根据数据库类型返回对应的DbType
            return dbType?.ToUpper() switch
            {
                "POSTGRESQL" => DbType.PostgreSQL,
                "MYSQL" => DbType.MySql,
                // 抛出不支持的数据库类型异常
                _ => throw new ArgumentException($"Unsupported database type: {dbType}")
            };
        }

        // 获取SqlSugarScope实例的方法
        public static SqlSugarScope GetInstance()
        {
            // 如果未初始化则抛出异常
            return _db ?? throw new InvalidOperationException("Database not initialized");
        }
    }
}