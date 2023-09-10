using Scaffolding.Extensions.Logging;
using Serilog;
using Serilog.Context;

namespace Scaffolding.Extensions.GracefulShutdown;

internal class GracefulShutdownMiddleware
{
    private readonly RequestDelegate _next;
    private readonly GracefulShutdownState _state;
    private readonly ShutdownSettings _shutdownSettings;
    private readonly LogSettings _logSettings;
    private readonly IHostApplicationLifetime _applicationLifetime;

    private DateTime _shutdownStarted;

    public GracefulShutdownMiddleware(
        RequestDelegate next,
        IHostApplicationLifetime applicationLifetime,
        GracefulShutdownState state,
        ShutdownSettings shutdownSettings,
        LogSettings logSettings)
    {
        _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _state = state ?? throw new ArgumentNullException(nameof(state));

        _shutdownSettings = shutdownSettings;
        _logSettings = logSettings;
        
        applicationLifetime.ApplicationStopping.Register(OnApplicationStopping);
        applicationLifetime.ApplicationStopped.Register(OnApplicationStopped);
    }

    public async Task Invoke(HttpContext context)
    {
        var ignoredRequest = _state.StopRequested;

        if (!ignoredRequest)
        {
            _state.NotifyRequestStarted();
        }

        try
        {
            await _next.Invoke(context);
        }
        finally
        {
            if (!ignoredRequest)
            {
                _state.NotifyRequestFinished();
            }
        }
    }

    private void OnApplicationStopping()
    {
        _state.NotifyStopRequested();
        _shutdownStarted = DateTime.UtcNow;
        var shutdownLimit = _shutdownStarted.Add(_shutdownSettings.ShutdownTimeoutTimeSpan);

        while (_state.RequestsInProgress > 0 && DateTime.UtcNow < shutdownLimit)
        {
            LogInfo("Application stopping, requests in progress: {RequestsInProgress}", _state.RequestsInProgress);
            Thread.Sleep(1000);
        }
    }

    private void OnApplicationStopped()
    {
        if (_state.RequestsInProgress > 0)
        {
            LogError("Application stopped, some requests were still in progress: {RequestsInProgress}", _state.RequestsInProgress);
        }
        else
        {
            LogInfo("Application stopped, requests in progress: {RequestsInProgress}", _state.RequestsInProgress);
        }

        _applicationLifetime.StopApplication();

        Log.CloseAndFlush();
    }

    public void LogInfo(string message, long requestsInProgress)
    {
        LogContext.PushProperty("RequestsInProgress", requestsInProgress);
        LogContext.PushProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

        Log.Logger.Information(_logSettings.TitlePrefix + " " + message);
    }

    public void LogError(string message, long requestsInProgress)
    {
        LogContext.PushProperty("RequestsInProgress", requestsInProgress);
        LogContext.PushProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

        Log.Logger.Error(_logSettings.TitlePrefix + " " + message);
    }
}
