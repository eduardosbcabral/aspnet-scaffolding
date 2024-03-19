using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Scaffolding.Logging.Settings.Implementations;

using Serilog;

namespace Scaffolding.AspNetCore.Filters;

public class RequestResponseLoggingEndpointFilter(IDiagnosticContext diagnosticContext) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.FirstOrDefault();

        if (request is not null)
        {
            diagnosticContext.Set("RequestBody", request, true);
        }

        if (context.HttpContext.Request.Headers.Any())
        {
            var logSettings = context.HttpContext.RequestServices.GetService<LogSettings>();
            if (logSettings is not null)
            {
                var headers = RequestResponseLoggingEndpointFilterHelpers.GetHeaders(context.HttpContext.Request.Headers, logSettings.LogHeaders);
                if (headers.Count > 0)
                    diagnosticContext.Set("RequestHeaders", headers, true);
            }
        }

        var response = await next(context);

        if (context.HttpContext.GetEndpoint() is { } e)
        {
            diagnosticContext.Set("Operation", $"{e.DisplayName}");
        }

        if (response is not null)
        {
            diagnosticContext.Set("ResponseBody", response, true);
        }

        if (context.HttpContext.Response.Headers.Any())
        {
            var logSettings = context.HttpContext.RequestServices.GetService<LogSettings>();
            if (logSettings is not null)
            {
                var headers = RequestResponseLoggingEndpointFilterHelpers.GetHeaders(context.HttpContext.Response.Headers, logSettings.LogHeaders);
                if (headers.Count > 0)
                    diagnosticContext.Set("ResponseHeaders", headers, true);
            }
        }

        return response;
    }
}
