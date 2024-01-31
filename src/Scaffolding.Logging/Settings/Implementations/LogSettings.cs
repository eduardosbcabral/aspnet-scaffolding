using Scaffolding.Configuration.Implementations;
using Scaffolding.Configuration.Settings;

using Serilog.Events;

using System.ComponentModel.DataAnnotations;

namespace Scaffolding.Logging.Settings.Implementations;

public class LogSettings(string section) : BaseSettings(section), ILogSettings
{
    /// <summary>
    /// Creates a new instance of <see cref="IApplicationSettings"/> with "Application" as section.
    /// </summary>
    public LogSettings() : this("Log")
    {
    }

    /// <inheritdoc/>
    [Required]
    public bool Enabled { get; set; }

    /// <inheritdoc/>
    [Required]
    public LogEventLevel MinimumLevel { get; set; }

    /// <inheritdoc/>
    public string[] IgnoredRoutes { get; set; } = [];
}