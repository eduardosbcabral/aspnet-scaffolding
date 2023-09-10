using Scaffolding.Extensions.Cors;
using Scaffolding.Extensions.CultureInfo;
using Scaffolding.Extensions.Docs;
using Scaffolding.Extensions.ExceptionHandler;
using Scaffolding.Extensions.GracefulShutdown;
using Scaffolding.Extensions.Healthcheck;
using Scaffolding.Extensions.Json;
using Scaffolding.Extensions.Logging;
using Scaffolding.Extensions.RateLimiting;
using Scaffolding.Extensions.RequestKey;
using Scaffolding.Extensions.TimeElapsed;
using Scaffolding.Models;
using Scaffolding.Utilities;
using Serilog;
using Serilog.Context;
using System.Collections.Specialized;
using System.Reflection;

namespace Scaffolding;

public static class Api
{
    public static WebApplicationBuilder Initialize(string[] args = null, string customUrls = null, params Assembly[] executingAssemblies)
    {
        var builder = WebApplication.CreateBuilder(args);

        var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>();
        if (apiSettings == null)
        {
            throw new Exception("'ApiSettings' section in the appsettings.json is required.");
        }

        builder.Configuration
            .AddJsonFile($"appsettings.{EnvironmentUtility.GetCurrentEnvironment()}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables(apiSettings.EnvironmentVariablesPrefix);

        builder.WebHost.UseUrls(customUrls ?? $"http://*:{apiSettings.Port}");
        builder.Services.AddSingleton(apiSettings);
        // It is needed because we cannot inject a service before
        // the configuration is built. This context is used by
        // the newtonsoft json settings configuration.
        var httpContextAccessor = new HttpContextAccessor();
        builder.Services.AddSingleton<IHttpContextAccessor>(httpContextAccessor);
        builder.Services.AddOptions();

        builder.SetupRateLimiting();
        builder.SetupSwaggerDocs();
        builder.SetupAllowCors();
        builder.SetupGracefulShutdown();
        builder.SetupRequestKey(apiSettings.RequestKeyProperty);
        builder.SetupTimeElapsed(apiSettings.TimeElapsedProperty);
        builder.ConfigureJsonSettings();

        var mvc = builder.Services.AddControllers(x =>
        {
            x.EnableEndpointRouting = false;
        });

        foreach (var assembly in executingAssemblies)
        {
            mvc.AddApplicationPart(assembly);
        }

        mvc.SetupScaffoldingJsonSettings(apiSettings, httpContextAccessor);

        return builder;
    }

    /// <summary>
    /// Use scaffolding logging
    /// </summary>
    /// <param name="customLogger">Custom logger to combine with the existing one from scaffolding.</param>
    public static void UseLogging(this WebApplicationBuilder builder, Serilog.ILogger customLogger = null)
    {
        builder.SetupScaffoldingSerilog(customLogger);
    }

    public static void UseDefaultConfiguration(this WebApplication app)
    {
        var apiSettings = app.Services.GetService<ApiSettings>();

        app.UseScaffoldingDocumentation();
        app.UseGracefulShutdown();
        app.UseScaffoldingRateLimiting();
        app.UseScaffoldingHealthchecks();
        app.UseScaffoldingSerilog();
        app.UseTimeElapsed();
        app.UseScaffoldingRequestLocalization();
        app.UseScaffoldingExceptionHandler();
        app.UsePathBase(new(apiSettings.GetFullPath()));
        app.UseRouting();
        app.UseCors();
        app.UseMvc();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    public static async Task RunAsync(WebApplication app)
    {
        var apiSettings = app.Services.GetService<ApiSettings>();
        LogContext.PushProperty("Application", apiSettings.Name);
        LogContext.PushProperty("Version", apiSettings.BuildVersion);
        Log.Logger.Information($"[{apiSettings.Name}] is running with version {apiSettings.BuildVersion}");
        await app.RunAsync();
    }
}
