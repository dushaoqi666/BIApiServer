namespace BIApiServer.Common
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public ApiSettings ApiSettings { get; set; }
        // 其他配置...
    }

    public class ConnectionStrings
    {
        public string Default { get; set; }
        public string Redis { get; set; }
    }

    public class ApiSettings
    {
        public string ApiBaseUrlOne { get; set; }
    }
} 