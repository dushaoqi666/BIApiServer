using Newtonsoft.Json;

namespace BIApiServer.Infrastructure
{
    public interface ITableEntity
    {
        #region 创建时间 CreateTime
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("create_time")]
        DateTime CreateTime { get; set; }
        #endregion

    }
}
