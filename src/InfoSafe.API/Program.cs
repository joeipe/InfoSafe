using InfoSafe.API.Configurations;
using InfoSafe.API.CustomSwaggerDocs;
using InfoSafe.Read.Data.Queries;
using InfoSafe.Write.Data;
using InfoSafe.Write.Data.Commands;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    if (builder.Environment.EnvironmentName != "IntegrationTest")
    {
        builder.Host.UseSerilog((ctx, lc) => lc
            .WriteTo.Console()
            .ReadFrom.Configuration(ctx.Configuration));
    }
    else
    {
        builder.Host.UseSerilog();
    }

    // Add services to the container.
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(Queries).Assembly);
        cfg.RegisterServicesFromAssembly(typeof(ContactSaveCommand).Assembly);
    });
    builder.Services.AddDatabaseConfiguration(builder.Configuration);
    builder.Services.AddAutoMapperConfiguration();
    builder.Services.AddScoped<InfoSafe.Write.Data.Repositories.Interfaces.IContactRepository, InfoSafe.Write.Data.Repositories.ContactRepository>();
    builder.Services.AddScoped<InfoSafe.Read.Data.Repositories.Interfaces.IContactRepository, InfoSafe.Read.Data.Repositories.ContactRepository>();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "AccessHive API", Version = $"1.0" });
        options.DocumentFilter<HealthChecksFilter>();
    });

    builder.Services
        .AddHealthChecks()
        .AddDbContextCheck<WriteDbContext>();

    builder.Logging.AddApplicationInsights(
        configureTelemetryConfiguration: (config) =>
            config.ConnectionString = builder.Configuration.GetConnectionString("ApplicationInsightsConnectionString"),
            configureApplicationInsightsLoggerOptions: (options) => { }
    );
    builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("your-category", LogLevel.Trace);

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyCustomExceptionMiddleware();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.MapDefaultHealthChecks();

    app.ApplyDatabaseSchema(app.Environment.EnvironmentName);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}

public partial class Program
{ }