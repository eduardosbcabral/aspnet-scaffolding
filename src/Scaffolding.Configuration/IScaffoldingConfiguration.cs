using Microsoft.Extensions.Configuration;

namespace Scaffolding.Configuration;

/// <summary>
/// Specifies an IConfiguration abstraction contract.
/// </summary>
public interface IScaffoldingConfiguration
{
    /// <summary>
    /// Instance of <see cref="IConfiguration"/>.
    /// </summary>
    /// <value></value>
    IConfiguration InnerConfiguration { get; }

    /// <summary>
    /// List of <see cref="ISettings">Settings</see> loaded from appsettings file.
    /// </summary>
    /// <value></value>
    IReadOnlyList<ISettings> Settings { get; }

    /// <summary>
    /// Get an instance of <see cref="ISettings">ISettings</see> from the Settings property.
    /// </summary>
    /// <typeparam name="TSettings">The inherited <see cref="ISettings">ISettings</see> type.</typeparam>
    /// <returns></returns>
    TSettings GetSettings<TSettings>() where TSettings : ISettings;

    /// <summary>
    /// Try get an instance of <see cref="ISettings">ISettings</see> from the Settings property.
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    /// <returns></returns>
    bool TryGetSettings<TSettings>(out TSettings settings) where TSettings : ISettings;
}