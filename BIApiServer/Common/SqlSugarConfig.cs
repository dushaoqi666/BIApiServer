using BIApiServer.Models.Interfaces;
using BIApiServer.Models;
using SqlSugar;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BIApiServer.Common
{
    public static class SqlSugarConfig
    {
        private static SqlSugarScope _db;
        private static readonly object _lock = new object();
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
            var dbType = configuration["Database:Type"];
            var connectionStrings = configuration.GetSection($"Database:ConnectionStrings:{dbType}").Get<Dictionary<string, string>>();

            var connectionConfigs = connectionStrings.Select(conn => new ConnectionConfig()
            {
                ConfigId = conn.Key,
                ConnectionString = conn.Value,
                DbType = GetDbType(dbType),
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            }).ToList();

            _db = new SqlSugarScope(connectionConfigs);
        }

        private static DbType GetDbType(string dbType)
        {
            return dbType?.ToUpper() switch
            {
                "POSTGRESQL" => DbType.PostgreSQL,
                "MYSQL" => DbType.MySql,
                _ => throw new ArgumentException($"Unsupported database type: {dbType}")
            };
        }

        public static SqlSugarScope GetInstance()
        {
            return _db ?? throw new InvalidOperationException("Database not initialized");
        }
    }
}