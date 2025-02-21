using BIApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ApiResponseFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult objectResult)
        {
            if (objectResult.Value is not ApiResponseBase<object>)
            {
                context.Result = new ObjectResult(new ApiResponseBase<object>
                {
                    Code = 200,
                    Message = "success",
                    Data = objectResult.Value
                });
            }
        }
    }
} 