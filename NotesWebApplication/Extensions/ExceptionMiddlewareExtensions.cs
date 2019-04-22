using Microsoft.AspNetCore.Builder;
using NotesWebApplication.CustomExceptionMiddleware;

namespace NotesWebApplication.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}