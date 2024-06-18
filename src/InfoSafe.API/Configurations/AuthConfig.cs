using InfoSafe.API.CustomAuthorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace InfoSafe.API.Configurations
{
    public static class AuthConfig
    {
        public static void AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.AddHttpContextAccessor();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = configuration["AzureAD:Authority"];
                    options.Audience = configuration["AzureAD:Audience"];
                    //options.TokenValidationParameters.ValidateIssuer = false;
                    options.TokenValidationParameters.ValidIssuer = configuration["AzureAD:ValidIssuer"];
                });
        }

        public static void AddAuthorizationConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "IsAdmin",
                    policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser();
                        policyBuilder.AddRequirements(new MustBeAdminRequirement());
                    });
            });
            services.AddScoped<IAuthorizationHandler, MustBeAdminHandler>();
        }

        public static void ApplyAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}