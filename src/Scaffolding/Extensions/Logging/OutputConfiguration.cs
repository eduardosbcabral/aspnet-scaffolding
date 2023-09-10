using Serilog.Events;

namespace Scaffolding.Extensions.Logging;

internal class OutputConfiguration
{
    /// <summary>
    /// Override namespace minimum level 
    /// </summary>
    public Dictionary<string, LogEventLevel> OverrideMinimumLevel { get; set; }

    /// <summary>
    /// Enrich Properties
    /// </summary>
    public Dictionary<string, object> EnrichProperties { get; set; }

    /// <summary>
    /// Console
    /// </summary>
    public ConsoleOptions Console { get; set; }

    /// <summary>
    /// Minimum level
    /// </summary>
    public LogEventLevel MinimumLevel { get; set; }

    /// <summary>
    /// Enable enrich by environment and context
    /// </summary>
    public bool EnableEnrichWithEnvironment { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public OutputConfiguration()
    {
        this.Console = new ConsoleOptions();
        this.OverrideMinimumLevel = new Dictionary<string, LogEventLevel>();
        this.EnrichProperties = new Dictionary<string, object>();
    }
}