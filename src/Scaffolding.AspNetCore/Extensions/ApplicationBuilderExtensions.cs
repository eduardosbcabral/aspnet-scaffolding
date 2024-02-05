using Microsoft.AspNetCore.Builder;

using Scaffolding.Configuration.Settings.Implementations;
using Scaffolding.Logging.Serilog.Helpers;

using Serilog;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds Serilog request logging to the request processing pipeline.
    /// Call it early in the pipeline to capture as much as possible.
    /// </summary>
    public static IApplicationBuilder UseSerilogLogging(this IApplicationBuilder appBuilder)
    {
        LogHelper.DEFAULT_USER_CLAIM = "Mundi.Employee";
        return appBuilder
            // Log requests
            .UseSerilogRequestLogging(
                // Don't log health check endpoints
                opts =>
                {
                    opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
                    opts.GetLevel = LogHelper.ExcludeHealthChecks;
                    opts.MessageTemplate = "[{Name}] [{Operation}] {RequestMethod} {Path} responded {StatusCode} in {Elapsed:0.0000} ms";
                });
    }

    public static IApplicationBuilder UseScaffoldingPathBase(this IApplicationBuilder app, string pathBase)
    {
        var applicationSettings = app.ApplicationServices.GetRequiredService<ApplicationSettings>();
        return app.UsePathBase($"{pathBase}/{applicationSettings.Version}");
    }
}
