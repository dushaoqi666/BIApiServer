namespace BIApiServer.Exceptions
{
    /// <summary>
    /// 未授权异常
    /// </summary>
    public class UnauthorizedException : BIException
    {
        public UnauthorizedException(string message = "未授权的访问") : base(message, 401)
        {
        }
    }
} 