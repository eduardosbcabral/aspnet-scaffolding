using Serilog.Events;

namespace Scaffolding.Extensions.Logging;

public class ConsoleOptions
{
    /// <summary>
    /// Set if console is enabled
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Minimum Level
    /// </summary>
    public LogEventLevel? MinimumLevel { get; set; }
}
