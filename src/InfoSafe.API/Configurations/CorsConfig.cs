using InfoSafe.API.CustomFeatureFilters;
using Microsoft.FeatureManagement.FeatureFilters;
using Microsoft.FeatureManagement;

namespace InfoSafe.API.Configurations
{
    public static class CorsConfig
    {
        public static void AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", corsBuilder =>
                {
                    corsBuilder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed(origin => origin == configuration["ClientUri"])
                    .AllowCredentials();
                });
            });
        }

        public static void ApplyCors(this IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");
        }
    }
}
