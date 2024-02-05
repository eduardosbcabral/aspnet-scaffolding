using Microsoft.AspNetCore.Http;

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
            diagnosticContext.Set("RequestHeaders", GetHeaders(context.HttpContext.Request.Headers), true);
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
            diagnosticContext.Set("ResponseHeaders", GetHeaders(context.HttpContext.Response.Headers), true);
        }

        return response;
    }

    private static Dictionary<string, string> GetHeaders(IHeaderDictionary headers)
    {
        var dic = new Dictionary<string, string>();
        foreach (var item in headers)
        {
            var value = item.Value.ToString();
            dic[item.Key] = value;
        }

        return dic;
    }
}
