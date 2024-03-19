using Microsoft.Extensions.Configuration;

using Scaffolding.Configuration.Implementations;

namespace Scaffolding.Configuration.Builders;

/// <summary>
/// Creates a new instance of <see cref="InterfacesConfigurationBuilder"/>.
/// </summary>
/// <param name="configuration">See <see cref="IConfiguration"/>.</param>
public class ScaffoldingConfigurationBuilder(IConfiguration configuration)
{
    private readonly List<ISettings> _settings = [];

    /// <summary>
    /// Adds a configuration settings. Ignored if already added.
    /// </summary>
    /// <param name="settings">Instance of an <see cref="ISettings"/>.</param>
    /// <typeparam name="TSettings"></typeparam>
    public ScaffoldingConfigurationBuilder With<TSettings>(TSettings settings) where TSettings : ISettings, new()
    {
        if (_settings.Any(s => s.GetType() == settings.GetType()))
        {
            return this;
        }

        _settings.Add(settings);

        return this;
    }

    /// <summary>
    /// Adds a configuration settings. Ignored if already added.
    /// </summary>
    /// <param name="settings">Instance of an <see cref="ISettings"/>.</param>
    /// <typeparam name="TSettings"></typeparam>
    public ScaffoldingConfigurationBuilder With<TSettings>() where TSettings : ISettings, new()
    {
        var settings = new TSettings();
        return With(settings);
    }

    /// <summary>
    /// Builds an object of ScaffoldingConfiguration.
    /// </summary>
    /// <returns><see cref="IScaffoldingConfiguration"/>.</returns>
    public IScaffoldingConfiguration Build()
    {
        return new ScaffoldingConfiguration(configuration, _settings);
    }
}
