using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Scaffolding.Models;
using System.Text;
using System.Text.Json;

namespace Scaffolding.Extensions.Healthcheck
{
    public static class HealthcheckMiddleware
    {
        public static void UseScaffoldingHealthchecks(this IApplicationBuilder app)
        {
            var apiSettings = app.ApplicationServices.GetService<ApiSettings>();
            var healthcheckSettings = app.ApplicationServices.GetService<HealthcheckSettings>();

            if (healthcheckSettings?.Enabled == true)
            {
                var options = new HealthCheckOptions
                {
                    AllowCachingResponses = true,
                    ResponseWriter = WriteResponse
                };

                var path = new PathString($"{apiSettings.GetFullPath()}/{healthcheckSettings.Path?.Trim('/')}");
                app.UseHealthChecks(path, options);
            }
        }

        private static Task WriteResponse(HttpContext context, HealthReport healthReport)
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
                    jsonWriter.WriteString("status",
                        healthReportEntry.Value.Status.ToString());

                    if (!string.IsNullOrEmpty(healthReportEntry.Value.Description))
                    {
                        jsonWriter.WriteString("description",
                            healthReportEntry.Value.Description);
                    }

                    if (healthReportEntry.Value.Data != null && healthReportEntry.Value.Data.Count > 0)
                    {
                        jsonWriter.WriteStartObject("data");

                        foreach (var item in healthReportEntry.Value.Data)
                        {
                            jsonWriter.WritePropertyName(item.Key);

                            JsonSerializer.Serialize(jsonWriter, item.Value,
                                item.Value?.GetType() ?? typeof(object));
                        }

                        jsonWriter.WriteEndObject();
                    }


                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            return context.Response.WriteAsync(
                Encoding.UTF8.GetString(memoryStream.ToArray()));
        }

        public static string GetFullPath(ApiSettings apiSettings, HealthcheckSettings healthcheckSettings)
        {
            var fullPath = apiSettings.GetFullPath();
            var finalPathPart = healthcheckSettings.Path?.Trim('/');

            return (fullPath ?? "") + "/" + (finalPathPart ?? "healthcheck");
        }
    }
}
