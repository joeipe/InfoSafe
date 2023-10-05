using InfoSafe.API.CustomFeatureFilters;
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
    }
}
