using InfoSafe.API.Configurations;
using InfoSafe.Infra.Bus;
using InfoSafe.Infra.Bus.ExtraForLearning;
using InfoSafe.Infra.Bus.Interfaces;
using InfoSafe.Read.Data.Queries;
using InfoSafe.Write.Data.Commands;
using InfoSafe.Write.Data.EventDispatchers;
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
            .ReadFrom.Configuration(ctx.Configuration)
            .Enrich.WithProperty("ApplicationName", "info safe"));
    }
    else
    {
        builder.Host.UseSerilog();
    }

    builder.Services.AddApplicationInsightsTelemetry(option =>
        option.ConnectionString = builder.Configuration.GetConnectionString("ApplicationInsightsConnectionString")
    );

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
    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddScoped<IBus>(c => new RmqFanOutServiceBus(builder.Configuration.GetConnectionString("RMQConnectionString")));
    }
    else
    {
        builder.Services.AddScoped<IBus>(c => new AzServiceBus(builder.Configuration.GetConnectionString("AzBusConnectionString")));
    }
    builder.Services.AddScoped<MessageBus>();
    builder.Services.AddScoped<EventDispatcher>();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerConfiguration();

    builder.Configuration.AddFeatureManagementConfiguration(builder.Environment);
    builder.Services.AddFeatureManagementConfiguration(builder.Environment);

    builder.Services.AddHealthCheckConfiguration();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    app.ApplySwagger();

    app.ApplyCustomExceptionMiddleware();

    app.ApplyFeatureManagement(app.Environment);

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.MapDefaultHealthChecks();

    app.ApplyDatabaseSchema(app.Environment);

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