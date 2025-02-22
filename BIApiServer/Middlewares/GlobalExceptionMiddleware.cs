using System.Net;
using System.Text.Json;
using BIApiServer.Exceptions;
using BIApiServer.Models;
using BIApiServer.Utils;

namespace BIApiServer.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new ApiResponse<object>();

            switch (exception)
            {
                case BIException biEx:
                    response.Code = biEx.Code;
                    response.Message = biEx.Message;
                    context.Response.StatusCode = biEx.Code;
                    break;

                case UnauthorizedAccessException:
                    response.Code = 401;
                    response.Message = "未授权的访问";
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;

                default:
                    _logger.LogError(exception, "An unexpected error occurred.");
                    response.Code = 500;
                    response.Message = "服务器内部错误";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var result = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(result);
        }
    }
} 