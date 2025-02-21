namespace BIApiServer.Exceptions
{
    /// <summary>
    /// 资源未找到异常
    /// </summary>
    public class NotFoundException : BIException
    {
        public NotFoundException(string message = "请求的资源不存在") : base(message, 404)
        {
        }
    }
} 