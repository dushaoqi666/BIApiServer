namespace BIApiServer.Exceptions
{
    /// <summary>
    /// 自定义异常基类
    /// </summary>
    public class BIException : Exception
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int Code { get; }

        public BIException(string message, int code = 500) : base(message)
        {
            Code = code;
        }
    }
} 