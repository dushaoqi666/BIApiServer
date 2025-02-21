using System.Net.Http.Headers;

namespace BIApiServer.Handlers
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 在这里添加认证头或其他通用处理
            request.Headers.Authorization = 
                new AuthenticationHeaderValue("Bearer", "your-token-here");

            return await base.SendAsync(request, cancellationToken);
        }
    }
} 