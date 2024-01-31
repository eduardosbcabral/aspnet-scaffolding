using Serilog.Events;

namespace Scaffolding.Configuration.Settings;

public interface ILogSettings
{
    /// <summary>
    /// Enable application log.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Application minimum log level.
    /// </summary>
    public LogEventLevel MinimumLevel { get; set; }

    /// <summary>
    /// Ignore log for specific routes.
    /// </summary>
    public string[] IgnoredRoutes { get; }
}
