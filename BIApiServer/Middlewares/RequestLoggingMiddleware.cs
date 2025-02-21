using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;
        var requestTime = DateTime.Now;

        _logger.LogInformation($"Request {request.Method} {request.Path} started at {requestTime}");

        await _next(context);

        var responseTime = DateTime.Now;
        var duration = responseTime - requestTime;

        _logger.LogInformation($"Request completed in {duration.TotalMilliseconds}ms");
    }
} 