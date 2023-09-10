using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Exceptions;

namespace Scaffolding.Extensions.GracefulShutdown;

public static class GracefulShutdownExtensions
{
    public static void HandleGracefulShutdown(this ControllerBase _, ShutdownSettings shutdownSettings)
    {
        if (shutdownSettings?.Enabled == true)
        {
            if (shutdownSettings.GracefulShutdownState.StopRequested && shutdownSettings.Redirect)
            {
                throw new PermanentRedirectException(null);
            }
            else if (shutdownSettings.GracefulShutdownState.StopRequested)
            {
                throw new ServiceUnavailableException("Service is unavailable for temporary maintenance.");
            }
        }
    }

    public static void SetupGracefulShutdown(this WebApplicationBuilder builder)
    {
        var shutdownSettings = builder.Configuration.GetSection("ShutdownSettings").Get<ShutdownSettings>();

        builder.Services.AddSingleton(shutdownSettings);
        builder.Services.AddSingleton(shutdownSettings.GracefulShutdownState);
        builder.Services.AddSingleton<IRequestsCountProvider>(shutdownSettings.GracefulShutdownState);
    }

    public static void UseGracefulShutdown(this WebApplication app)
    {
        var shutdownSettings = app.Services.GetService<ShutdownSettings>();

        if (shutdownSettings?.Enabled == true)
        {
            app.UseMiddleware<GracefulShutdownMiddleware>();
        }
    }
}
