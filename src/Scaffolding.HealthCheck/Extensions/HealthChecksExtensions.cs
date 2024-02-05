using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

using Scaffolding.HealthCheck.ResponseWriter;

namespace Scaffolding.HealthCheck.Extensions;

/// <summary>
/// Extends application health check functionalities.
/// </summary>
public static class HealthChecksExtensions
{
    /// <summary>
    /// Extends <see cref="IApplicationBuilder"/> to use health checks.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseScaffoldingHealthCheck(this IApplicationBuilder app)
    {
        return app.RegisteringHealthChecks("/healthcheck/ready", "/healthcheck/live");
    }

    private static IApplicationBuilder RegisteringHealthChecks(this IApplicationBuilder app, string healthcheckReady, string healthcheckLive)
    {
        return app
           .UseHealthChecks(
                healthcheckReady,
                new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = HealthCheckResponseWriter.WriteResponse
                })
           .UseHealthChecks(
                healthcheckLive,
                new HealthCheckOptions
                {
                    Predicate = healthCheck => healthCheck.Tags.Contains("live")
                });
    }
}
