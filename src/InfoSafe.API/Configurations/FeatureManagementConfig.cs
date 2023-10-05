using InfoSafe.API.CustomFeatureFilters;
using InfoSafe.Write.Data;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;

namespace InfoSafe.API.Configurations
{
    public static class FeatureManagementConfig
    {
        public static void AddFeatureManagementConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services
                .AddFeatureManagement()
                .AddFeatureFilter<PercentageFilter>()
                .AddFeatureFilter<TimeWindowFilter>()
                .AddFeatureFilter<RandomFilter>();
        }

        public static void AddFeatureManagementConfiguration(this ConfigurationManager configuration, IWebHostEnvironment environment)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (environment == null) throw new ArgumentNullException(nameof(environment));

            if (!environment.IsDevelopment())
            {
                configuration.AddAzureAppConfiguration(options =>
                    options.Connect(configuration.GetConnectionString("AppConfigConnectionString"))
                    .UseFeatureFlags());
            }
        }

        public static void ApplyFeatureManagement(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            if (!environment.IsDevelopment())
            {
                app.UseAzureAppConfiguration();
            }
        }
    }
}
