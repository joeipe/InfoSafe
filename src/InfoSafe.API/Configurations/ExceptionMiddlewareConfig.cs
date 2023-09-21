using InfoSafe.API.CustomExceptionMiddleware;

namespace InfoSafe.API.Configurations
{
    public static class ExceptionMiddlewareConfig
    {
        public static void ApplyCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}