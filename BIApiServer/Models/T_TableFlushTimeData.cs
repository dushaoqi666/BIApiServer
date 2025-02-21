using BIApiServer.Infrastructure;
using Newtonsoft.Json;
using SqlSugar;
using System.Drawing;

namespace BIApiServer.Models
{
    [SugarTable("t_table_flush_time", "定时更新数据表")]
    public class T_TableFlushTimeData : BaseTableEntity
    {
        [JsonConverter(typeof(LongConverter))]
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public long Id { get; set; }

        /// <summary>
        /// 表名称 
        /// </summary>
        [SugarColumn(DefaultValue = "''::character varying", Length = 100, ColumnDescription = "表名称")]
        public string? TableName { get; set; }

        /// <summary>
        /// 最后更新时间 
        /// </summary>
        [SugarColumn(ColumnDataType = "timestamp", DefaultValue = "'1900-01-01 00:00:00'", ColumnDescription = "最后更新时间")]
        public DateTime LastFlushTime { get; set; }

        /// <summary>
        /// 最初时间 
        /// </summary>
        [SugarColumn(ColumnDataType = "timestamp", DefaultValue = "'1900-01-01 00:00:00'", ColumnDescription = "最初时间")]
        public DateTime FirstTime { get; set; }
    }
}
