using InfoSafe.API.AutoMapper;

namespace InfoSafe.API.Configurations
{
    public static class AutoMapperConfig
    {
        public static void AddAutoMapperConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAutoMapper(typeof(AutoMapperProfileSetup));

            AutoMapperProfileSetup.RegisterMappings();
        }
    }
}