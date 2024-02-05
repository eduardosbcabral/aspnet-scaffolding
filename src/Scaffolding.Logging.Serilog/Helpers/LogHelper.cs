using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

using Serilog;
using Serilog.Events;

using System.IdentityModel.Tokens.Jwt;

namespace Scaffolding.Logging.Serilog.Helpers;

public static class LogHelper
{
    /// <summary>
    /// Replacement of the default implementation of RequestLoggingOptions.GetLevel.
    /// Excludes health check endpoints from logging by decreasing their log level.
    /// </summary>
    public static LogEventLevel ExcludeHealthChecks(HttpContext ctx, double _, Exception? ex) =>
        ex != null
            ? LogEventLevel.Error
            : ctx.Response.StatusCode > 499
                ? LogEventLevel.Error
                : IsHealthCheckEndpoint(ctx) // Not an error, check if it was a health check
                    ? LogEventLevel.Verbose // Was a health check, use Verbose
                    : LogEventLevel.Information;

    private readonly static string[] POSSIBLE_HEALTHCHECK_PATHS = [
        "healthcheck",
        "health-check",
    ];

    private static bool IsHealthCheckEndpoint(HttpContext ctx)
    {
        var endpoint = ctx.GetEndpoint();
        if (endpoint is not null)
        {
            return string.Equals(
                endpoint.DisplayName,
                "Health checks",
                StringComparison.Ordinal);
        }
        else if (ctx.Request.Path.HasValue)
        {
            return POSSIBLE_HEALTHCHECK_PATHS.Any(x => ctx.Request.Path.Value.Contains(x));
        }

        return false;
    }

    public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var request = httpContext.Request;

        // Set all the common properties available for every request
        diagnosticContext.Set("Host", request.Host);
        diagnosticContext.Set("Protocol", request.Protocol);
        diagnosticContext.Set("Scheme", request.Scheme);
        diagnosticContext.Set("Path", request.Path);

        // Only set it if available. You're not sending sensitive data in a querystring right?!
        if (request.QueryString.HasValue)
        {
            diagnosticContext.Set("QueryString", request.QueryString.Value);
        }

        // Set the content-type of the Response at this point
        diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

        // Retrieve the IEndpointFeature selected for the request
        var endpoint = httpContext.GetEndpoint();
        if (endpoint is not null)
        {
            diagnosticContext.Set("Operation", endpoint.DisplayName);
        }

        EnrichJwtToken(diagnosticContext, httpContext);
    }

    public static string DEFAULT_USER_CLAIM = "Employee";

    private static void EnrichJwtToken(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var request = httpContext.Request;

        var authenticationMethod = request.Headers[HeaderNames.Authorization].ToString() switch
        {
            var authorization when !string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer ") => "Bearer",
            _ => "No Auth"
        };

        diagnosticContext.Set("AuthenticationMethod", authenticationMethod);

        if (authenticationMethod.Equals("Bearer"))
        {
            var jwt = request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();

            var token = handler.ReadJwtToken(jwt);

            var userLog = token.Payload.GetValueOrDefault(DEFAULT_USER_CLAIM) switch
            {
                true => token.Payload.GetValueOrDefault("email")?.ToString() ?? "",
                false => token.Payload.GetValueOrDefault("client_id")?.ToString() ?? "",
                _ => ""
            };

            if (userLog is not "")
            {
                var accessor = new HttpContextAccessor();

                var log = new LogAdditionalInformation(accessor);

                log.Data.Add("User", userLog);
            }
        }
    }
}
