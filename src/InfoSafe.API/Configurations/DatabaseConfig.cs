using InfoSafe.Read.Data;
using InfoSafe.Write.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Data.SqlClient;

namespace InfoSafe.API.Configurations
{
    public static class DatabaseConfig
    {
        public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped(x =>
                new ReadDbContext(new SqlConnection(configuration.GetConnectionString("DBConnectionString")))
            );

            services.AddDbContext<WriteDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DBConnectionString"))
            );
        }

        public static void ApplyDatabaseSchema(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
            try
            {
                if (!environment.IsEnvironment("IntegrationTest"))
                {
                    var writeDbContext = serviceScope?.ServiceProvider.GetRequiredService<WriteDbContext>();
                    serviceScope?.ServiceProvider.GetRequiredService<WriteDbContext>().Database.Migrate();
                }
            }
            catch (Exception)
            {
                Thread.Sleep(TimeSpan.FromSeconds(15));
                app.ApplyDatabaseSchema(environment);
            }
        }
    }
}