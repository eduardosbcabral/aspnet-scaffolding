using Microsoft.Extensions.Logging;

namespace Scaffolding.Logging.Settings;

public interface ILogSettings
{
    /// <summary>
    /// Enable application log.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Application minimum log level.
    /// </summary>
    public LogLevel MinimumLevel { get; set; }

    /// <summary>
    /// Ignore log for specific routes.
    /// </summary>
    public string[] IgnoredRoutes { get; }
}
