using Scaffolding.Extensions.Docs;
using Scaffolding.Extensions.Healthcheck;
using Scaffolding.Extensions.RequestKey;
using Scaffolding.Extensions.TimeElapsed;
using Scaffolding.Models;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;

namespace Scaffolding.Extensions.Logging;

public static class LoggingServiceExtension
{
    public static void SetupScaffoldingSerilog(
        this WebApplicationBuilder builder,
        Serilog.ILogger customLogger = null)
    {
        var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>();
        var logSettings = builder.Configuration.GetSection("LogSettings").Get<LogSettings>();
        var healthcheckSettings = builder.Configuration.GetSection("HealthcheckSettings").Get<HealthcheckSettings>();

        if (logSettings == null) return;

        builder.Services.AddSingleton(logSettings);

        if (string.IsNullOrWhiteSpace(apiSettings.Domain))
        {
            throw new Exception("Missing 'Domain' configuration.");
        }

        if (string.IsNullOrWhiteSpace(apiSettings.Name))
        {
            throw new Exception("Missing 'Name' configuration.");
        }

        var outputConfiguration = new OutputConfiguration()
        {
            MinimumLevel = LogEventLevel.Debug,
            Console = logSettings.Console
        };
        AddOverrideMinimumLevel(outputConfiguration);
        AddEnrichProperty(outputConfiguration, apiSettings);
        EnableDebug(logSettings, outputConfiguration);

        var loggerConfiguration = new LoggerConfiguration();

        if (customLogger != null)
        {
            loggerConfiguration.WriteTo.Logger(customLogger);
        }

        loggerConfiguration
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithThreadName()
            .Enrich.WithThreadId()
            .WriteTo.Console(outputConfiguration.MinimumLevel)
            .Destructure.ToMaximumDepth(15);

        OverrideMinimumLevel(loggerConfiguration, outputConfiguration);
        EnrichProperties(loggerConfiguration, outputConfiguration);
        IgnoreRoutes(apiSettings, logSettings, healthcheckSettings);

        Log.Logger = loggerConfiguration.CreateLogger();

        var config = new LogConfiguration
        {
            Version = apiSettings?.BuildVersion,
            InformationTitle = $"{logSettings?.TitlePrefix}{logSettings?.GetInformationTitle()}",
            ErrorTitle = $"{logSettings?.TitlePrefix}{logSettings?.GetErrorTitle()}",
            BlacklistRequest = logSettings?.JsonBlacklistRequest,
            BlacklistResponse = logSettings?.JsonBlacklistResponse,
            HeaderBlacklist = logSettings?.HeaderBlacklist,
            HttpContextBlacklist = logSettings?.HttpContextBlacklist,
            QueryStringBlacklist = logSettings?.QueryStringBlacklist,
            RequestKeyProperty = RequestKeyServiceExtension.RequestKeyHeaderName,
            TimeElapsedProperty = TimeElapsedServiceExtension.TimeElapsedHeaderName,
            IgnoredRoutes = logSettings?.IgnoredRoutes.ToArray()
        };

        builder.Services.AddScoped(x => new LogAdditionalInfo(x.GetService<IHttpContextAccessor>()));
        builder.Services.AddSingleton((Func<IServiceProvider, ICommunicationLogger>)(x => new CommunicationLogger(config)));
    }

    private static void AddOverrideMinimumLevel(OutputConfiguration outputConfiguration)
    {
        outputConfiguration.OverrideMinimumLevel["Microsoft"] = LogEventLevel.Warning;
        outputConfiguration.OverrideMinimumLevel["System"] = LogEventLevel.Error;
    }

    private static void AddEnrichProperty(OutputConfiguration outputConfiguration, ApiSettings apiSettings)
    {
        outputConfiguration.EnrichProperties["Domain"] = apiSettings.Domain;
        outputConfiguration.EnrichProperties["Application"] = apiSettings.Name;
    }

    private static void EnableDebug(LogSettings logSettings, OutputConfiguration outputConfiguration)
    {
        if (logSettings?.DebugEnabled == true)
        {
            logSettings.Console.MinimumLevel = outputConfiguration.MinimumLevel;
            SelfLog.Enable(delegate (string msg)
            {
                msg = "==== Serilog Debug ==== \n" + msg + "\n=======================";
                Console.WriteLine(msg);
            });
        }
    }

    private static void OverrideMinimumLevel(LoggerConfiguration loggerConfiguration, OutputConfiguration outputConfiguration)
    {
        loggerConfiguration.MinimumLevel.Is(outputConfiguration.MinimumLevel);
        foreach (var item in outputConfiguration.OverrideMinimumLevel)
        {
            loggerConfiguration.MinimumLevel.Override(item.Key, item.Value);
        }
    }

    private static void EnrichProperties(LoggerConfiguration loggerConfiguration, OutputConfiguration outputConfiguration)
    {
        foreach (var enrichProperty in outputConfiguration.EnrichProperties)
        {
            loggerConfiguration.Enrich.WithProperty(enrichProperty.Key, enrichProperty.Value);
        }
    }

    private static void IgnoreRoutes(
        ApiSettings apiSettings,
        LogSettings logSettings,
        HealthcheckSettings healthcheckSettings)
    {
        List<string> ignoredRoutes;
        if (DocsServiceExtension.DocsSettings == null)
        {
            ignoredRoutes = new List<string>();
        }
        else
        {
            ignoredRoutes = DocsServiceExtension.DocsSettings.GetDocsFinalRoutes().ToList();
        }

        if (healthcheckSettings.LogEnabled == false)
        {
            ignoredRoutes.Add(HealthcheckMiddleware.GetFullPath(apiSettings, healthcheckSettings));
        }

        logSettings.IgnoredRoutes.AddRange(ignoredRoutes);
    }
}
