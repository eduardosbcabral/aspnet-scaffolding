using Destructurama;

using Microsoft.Extensions.Hosting;

using Scaffolding.Configuration;
using Scaffolding.Configuration.Settings.Implementations;
using Scaffolding.Logging.Formatters;
using Scaffolding.Logging.Serilog.Enrichers;
using Scaffolding.Logging.Settings.Implementations;

using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace Microsoft.Extensions.DependencyInjection;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddSerilogLogging(this IHostBuilder builder, IScaffoldingConfiguration configuration, string requestKeyHeader = "RequestKey")
    {
        return builder.UseSerilog((context, loggerCfg) =>
        {
            var applicationSettings = configuration.GetSettings<ApplicationSettings>();
            var logSettings = configuration.GetSettings<LogSettings>();

            loggerCfg
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithCorrelationId(requestKeyHeader)
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .Enrich.WithProperty(nameof(applicationSettings.Name), applicationSettings.Name)
                .Enrich.WithProperty(nameof(applicationSettings.Domain), applicationSettings.Domain)
                .Enrich.With(new RemovePropertiesEnricher());

            if (!context.HostingEnvironment.IsDevelopment())
            {
                loggerCfg
                    .WriteTo.Console()
                    .WriteTo.Debug();
            }
            else
            {
                loggerCfg
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    // Filter out ASP.NET Core infrastructre logs that are Information and below
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .Destructure.UsingAttributes(x => x.IgnoreNullProperties = true)
                    .WriteTo.Async(x => x.Console(new SnakeCaseRenderedCompactJsonFormatter()));
            }
        });
    }
}
    