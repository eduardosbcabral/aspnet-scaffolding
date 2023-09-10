using System.Diagnostics;

namespace Scaffolding.Extensions.TimeElapsed;

internal class TimeElapsedMiddleware
{
    private readonly RequestDelegate Next;

    public TimeElapsedMiddleware(RequestDelegate next)
    {
        Next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = new Stopwatch();
        string timeIsMs = "-1";

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[TimeElapsedServiceExtension.TimeElapsedHeaderName] = timeIsMs;
            return Task.CompletedTask;
        });

        stopwatch.Start();

        await Next(context);

        stopwatch.Stop();
        timeIsMs = stopwatch.ElapsedMilliseconds.ToString();
        context.Items[TimeElapsedServiceExtension.TimeElapsedHeaderName] = timeIsMs;
    }
}

internal static class TimeElapsedMiddlewareExtension
{
    public static void UseTimeElapsed(this IApplicationBuilder app)
    {
        app.UseMiddleware<TimeElapsedMiddleware>();
    }
}
