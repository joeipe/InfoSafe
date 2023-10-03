using InfoSafe.API.CustomSwaggerDocs;
using Microsoft.OpenApi.Models;

namespace InfoSafe.API.Configurations
{
    public static class SwaggerConfig
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "AccessHive API", Version = $"1.0" });
                options.DocumentFilter<HealthChecksFilter>();
            });
        }

        public static void ApplySwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}