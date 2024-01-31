using Scaffolding.Logging.Formatter;

using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace Scaffolding.Logging;

public class LoggerBuilder
{
    public LoggerConfiguration LoggerConfiguration { get; private set; }

    public LoggerBuilder()
    {
        LoggerConfiguration = new LoggerConfiguration();
    }

    public LoggerBuilder WithCustomLogger(ILogger customLogger)
    {
        LoggerConfiguration.WriteTo.Logger(customLogger);
        return this;
    }

    public LoggerBuilder WithDefaultConfiguration()
    {
        LoggerConfiguration
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithCorrelationId()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithThreadName()
            .Enrich.WithThreadId()
            .Destructure.ToMaximumDepth(15);
        return this;
    }

    public LoggerBuilder WithProperty(string name, object value)
    {
        LoggerConfiguration.Enrich.WithProperty(name, value, true);
        return this;
    }

    //public LoggerBuilder WriteToConsoleSnakeCase()
    //{
    //    LoggerConfiguration.WriteTo.Async(x => x.Console(new SnakeCaseRenderedCompactJsonFormatter()));
    //    return this;
    //}

    public LoggerBuilder WriteToConsolePascalCase()
    {
        LoggerConfiguration.WriteTo.Async(x => x.Console(new RenderedCompactJsonFormatter()));
        return this;
    }

    public static LoggerBuilder Instance() => new();

    public ILogger Build() => LoggerConfiguration.CreateLogger();
}
