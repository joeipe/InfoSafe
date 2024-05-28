using InfoSafe.Write.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text;
using System.Text.Json;

namespace InfoSafe.API.Configurations
{
    public static class HealthCheckConfig
    {
        public static void AddHealthCheckConfiguration(this IServiceCollection services)
        {
            services
                .AddHealthChecks()
                .AddDbContextCheck<WriteDbContext>(); //.AddDbContextCheck<WriteDbContext>(tags: new string[] { "ready" });
        }

        public static IEndpointRouteBuilder MapDefaultHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false,
                ResponseWriter = WriteJsonResponse
            });

            endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                //Predicate = reg => reg.Tags.Contains("ready"),
                ResponseWriter = WriteJsonResponse
            });

            return endpoints;
        }

        private static Task WriteJsonResponse(HttpContext context, HealthReport healthReport)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var options = new JsonWriterOptions { Indented = true };

            using var memoryStream = new MemoryStream();
            using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WriteString("status", healthReport.Status.ToString());
                jsonWriter.WriteStartObject("results");

                foreach (var healthReportEntry in healthReport.Entries)
                {
                    jsonWriter.WriteStartObject(healthReportEntry.Key);
                    jsonWriter.WriteString("status", healthReportEntry.Value.Status.ToString());
                    jsonWriter.WriteString("description", healthReportEntry.Value.Description);
                    jsonWriter.WriteStartObject("data");

                    foreach (var item in healthReportEntry.Value.Data)
                    {
                        jsonWriter.WritePropertyName(item.Key);

                        JsonSerializer.Serialize(jsonWriter, item.Value, item.Value?.GetType() ?? typeof(object));
                    }

                    jsonWriter.WriteEndObject();
                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            return context.Response.WriteAsync(Encoding.UTF8.GetString(memoryStream.ToArray()));
        }
    }
}