namespace Scaffolding.Extensions.RequestKey;

internal class RequestKeyMiddleware : IMiddleware
{
    private readonly RequestKey _requestKey;

    public RequestKeyMiddleware(RequestKey requestKey)
    {
        _requestKey = requestKey;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            context.Request.EnableBuffering();
        }
        catch (Exception) { }

        if (context.Request.Headers.ContainsKey(RequestKeyServiceExtension.RequestKeyHeaderName))
        {
            _requestKey.Value = context.Request.Headers[RequestKeyServiceExtension.RequestKeyHeaderName];
        }
        else
        {
            _requestKey.Value = Guid.NewGuid().ToString();
        }

        context.Items.Add(RequestKeyServiceExtension.RequestKeyHeaderName, _requestKey.Value);
        context.Response.Headers.Add(RequestKeyServiceExtension.RequestKeyHeaderName, _requestKey.Value);

        await next(context);
    }
}

internal static class RequestKeyMiddlewareExtension
{
    public static void UseRequestKey(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestKeyMiddleware>();
    }
}