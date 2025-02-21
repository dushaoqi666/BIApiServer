using Newtonsoft.Json;
using SqlSugar;

namespace BIApiServer.Infrastructure
{
    public abstract class BaseTableEntity : ITableEntity
    {
        #region 创建时间 CreateTime
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("create_time")]
        [SugarColumn(ColumnName = "create_time", ColumnDescription = "创建时间", IsNullable = false, CreateTableFieldSort = 99)]
        public virtual DateTime CreateTime { get; set; }
        #endregion
    }
}

