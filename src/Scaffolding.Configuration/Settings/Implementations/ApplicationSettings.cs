using Scaffolding.Configuration.Implementations;
using System.ComponentModel.DataAnnotations;

namespace Scaffolding.Configuration.Settings.Implementations;

/// <inheritdoc cref="IApplicationSettings"/>
/// <summary>
/// Creates a new instance of <see cref="IApplicationSettings"/>.
/// </summary>
/// <param name="section">The section in the appsettings file to be loaded from.</param>
public class ApplicationSettings(string section) : BaseSettings(section), IApplicationSettings
{
    /// <summary>
    /// Creates a new instance of <see cref="IApplicationSettings"/> with "Application" as section.
    /// </summary>
    public ApplicationSettings() : this("Application")
    {
    }

    /// <inheritdoc/>
    [Required]
    public string Name { get; set; }

    /// <inheritdoc/>
    [Required]
    [RegularExpression("^v.*\\d*$")]
    public string Version { get; set; }

    /// <inheritdoc/>
    [Required]
    public string Domain { get; set; }

    /// <inheritdoc/>
    [Required]
    public int Port { get; set; }
}