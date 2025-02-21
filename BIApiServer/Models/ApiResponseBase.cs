namespace BIApiServer.Models
{
    public class ApiResponseBase<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; } = 200;

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; } = "获取成功";

        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }
    }

    public class PagedApiResponse<T> : ApiResponseBase<T>
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public long Total { get; set; }
    }
} 