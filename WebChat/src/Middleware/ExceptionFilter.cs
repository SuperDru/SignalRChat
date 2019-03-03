using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebChat.Middleware
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            const int code = 400;

            context.Result = new ObjectResult(new ApiError
            {
                Message = context.Exception.Message,
                Code = code
            })
            {
                StatusCode = code
            };
        }
    }
}