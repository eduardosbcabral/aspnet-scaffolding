namespace Scaffolding.Extensions.Logging;

public class LogMiddleware
{
    private readonly RequestDelegate Next;

    private readonly ICommunicationLogger CommunicationLogger;

    public LogMiddleware(
        RequestDelegate next,
        ICommunicationLogger communicationLogger)
    {
        this.Next = next;
        this.CommunicationLogger = communicationLogger;
    }

    public async Task Invoke(HttpContext context)
    {
        var originalResponse = context.Response.Body;
        context.Response.Body = new MemoryStream();

        await this.Next(context);

        if (context.Items.ContainsKey("Exception"))
        {
            var exception = (Exception)context.Items["Exception"];
            await this.CommunicationLogger.LogData(context, exception);
        }
        else
        {
            await this.CommunicationLogger.LogData(context);
        }

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        await context.Response.Body.CopyToAsync(originalResponse);
    }

}

public static class LogMiddlewareExtension
{
    public static void UseScaffoldingSerilog(this IApplicationBuilder app)
    {
        app.UseMiddleware<LogMiddleware>();
    }
}