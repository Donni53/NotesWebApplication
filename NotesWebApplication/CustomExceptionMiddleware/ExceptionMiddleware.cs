using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NotesWebApplication.Models.Responses;

namespace NotesWebApplication.CustomExceptionMiddleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception e)
        {
            var statusCode = 500;
            switch (e)
            {
                case ArgumentException _:
                case IndexOutOfRangeException _:
                    statusCode = 400;
                    break;
            }

            switch (e)
            {
                case ArgumentNullException _:
                case ArgumentOutOfRangeException _:
                    statusCode = 400;
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(
                new ErrorResponse
                {
                    Status = statusCode,
                    DeveloperMessage = $"{e.Source} {nameof(e)}",
                    UserMessage = e.Message,
                    MoreInfo = e.HelpLink,
                    ErrorCode = e.HResult
                }.ToString()
            );
        }
    }
}