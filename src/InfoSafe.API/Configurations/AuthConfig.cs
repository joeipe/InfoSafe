using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;

namespace InfoSafe.API.Configurations
{
    public static class AuthConfig
    {

        public static void AddAuthConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = configuration["AzureAD:Authority"];
                    options.Audience = configuration["AzureAD:Audience"];
                    //options.TokenValidationParameters.ValidateIssuer = false;
                    options.TokenValidationParameters.ValidIssuer = configuration["AzureAD:ValidIssuer"];
                });
        }

        public static void ApplyAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
