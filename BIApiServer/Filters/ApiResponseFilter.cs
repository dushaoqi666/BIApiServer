// using BIApiServer.Utils;

using BIApiServer.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

public class ApiResponseFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult objectResult)
        {
            // 如果返回值已经是 ApiResponse，则不再包装
            if (objectResult.Value?.GetType().IsGenericType == true && 
                objectResult.Value.GetType().GetGenericTypeDefinition() == typeof(ApiResponse<>))
            {
                return;
            }

            var response = new ApiResponse<object>
            {
                Code = StatusCodes.Status200OK,
                Message = "Success",
                Data = objectResult.Value
            };

            // 如果响应头中包含总数，则添加到响应中
            if (context.HttpContext.Response.Headers.ContainsKey("X-Total-Count"))
            {
                if (int.TryParse(context.HttpContext.Response.Headers["X-Total-Count"], out int total))
                {
                    response.Total = total;
                }
                context.HttpContext.Response.Headers.Remove("X-Total-Count");
            }

            context.Result = new ObjectResult(response);
        }
    }
} 