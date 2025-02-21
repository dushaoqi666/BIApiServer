namespace BIApiServer.Exceptions
{
    /// <summary>
    /// 业务异常
    /// </summary>
    public class BusinessException : BIException
    {
        public BusinessException(string message) : base(message, 400)
        {
        }
    }
} 