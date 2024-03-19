using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

using Scaffolding.AspNetCore.Extensions;

namespace Scaffolding.AspNetCore.RequestKey;

public class RequestKeyMiddleware(RequestKey requestKey) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            context.Request.EnableBuffering();
        }
        catch (Exception) { }

        if (context.Request.Headers.TryGetValue(RequestKeyExtensions.RequestKeyHeaderName, out StringValues value))
        {
            if (value.ToString() is not null)
            {
                requestKey = value.ToString();
            }
        }
        else
        {
            requestKey.Value = Guid.NewGuid().ToString();
        }

        context.Items.Add(RequestKeyExtensions.RequestKeyHeaderName, requestKey.Value);
        context.Response.Headers.Append(RequestKeyExtensions.RequestKeyHeaderName, requestKey.Value);

        await next(context);
    }
}
