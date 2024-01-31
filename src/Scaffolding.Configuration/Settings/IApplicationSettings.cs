namespace Scaffolding.Configuration.Settings;

/// <summary>
/// Application basic settings.
/// </summary>
public interface IApplicationSettings : ISettings
{
    /// <summary>
    /// Get/Set application name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Get/Set application team.
    /// </summary>
    string Domain { get; }

    /// <summary>
    /// Get/Set application version.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Get/Set application port.
    /// </summary>
    int Port { get; }
}