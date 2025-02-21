using SqlSugar;
using BIApiServer.Models.Interfaces;

namespace BIApiServer.Models
{
    public abstract class Entity : IDeletedFilter
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "create_time", IsOnlyIgnoreUpdate = true)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnName = "update_time", IsOnlyIgnoreInsert = true)]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn(ColumnName = "is_deleted")]
        public bool IsDeleted { get; set; }
    }
}