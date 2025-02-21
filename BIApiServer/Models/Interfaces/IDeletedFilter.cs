namespace BIApiServer.Models.Interfaces
{
    /// <summary>
    /// 软删除过滤接口
    /// </summary>
    public interface IDeletedFilter
    {
        /// <summary>
        /// 是否已删除
        /// </summary>
        bool IsDeleted { get; set; }
    }
}