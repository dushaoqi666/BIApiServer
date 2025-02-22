using BIApiServer.Interfaces;
using BIApiServer.Models.Interfaces;
using SqlSugar;

namespace BIApiServer.Models
{
    public abstract class EntityBases
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
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnName = "update_time")]
        public DateTime UpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn(ColumnName = "is_deleted")]
        public bool IsDeleted { get; set; } = false;
    }
}