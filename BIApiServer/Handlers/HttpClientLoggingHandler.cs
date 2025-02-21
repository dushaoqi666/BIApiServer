using System.Net.Http.Headers;

namespace BIApiServer.Handlers
{
    public class HttpClientLoggingHandler : DelegatingHandler
    {
        private readonly ILogger<HttpClientLoggingHandler> _logger;

        public HttpClientLoggingHandler(ILogger<HttpClientLoggingHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"HTTP {request.Method} {request.RequestUri}");

            var response = await base.SendAsync(request, cancellationToken);

            _logger.LogInformation($"HTTP Response Status Code: {response.StatusCode}");

            return response;
        }
    }
} 