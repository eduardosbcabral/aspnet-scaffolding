using Microsoft.Extensions.Configuration;

namespace Scaffolding.Configuration.Implementations;

/// <inheritdoc/>
internal class ScaffoldingConfiguration : IScaffoldingConfiguration
{
    private readonly List<ISettings> _settings;

    /// <summary>
    /// </summary>
    /// <param name="innerConfiguration">See <see cref="IConfiguration"/>.</param>
    /// <param name="settings">List of <see cref="ISettings"/>.</param>
    /// <param name="externalSettings"></param>
    public ScaffoldingConfiguration(IConfiguration innerConfiguration, List<ISettings> settings)
    {
        var messages = new List<string>();

        InnerConfiguration = innerConfiguration;
        _settings = settings;
        _settings.ForEach(s =>
        {
            InnerConfiguration.Bind(s.Section, s);
            if (s.IsValid(out var validationMessage)) return;
            messages.Add(validationMessage);
        });

        if (messages.Any())
        {
            throw new ArgumentException(string.Join("\n\n", messages));
        }
    }

    /// <inheritdoc/>
    public IConfiguration InnerConfiguration { get; }

    /// <inheritdoc/>
    public IReadOnlyList<ISettings> Settings => _settings;

    /// <inheritdoc/>
    public TSettings GetSettings<TSettings>() where TSettings : ISettings
    {
        if (!TryGetSettings<TSettings>(out var settings))
        {
            throw new NullReferenceException($"No settings of {typeof(TSettings).Name} type was found.");
        }

        return settings;
    }

    /// <inheritdoc />
    public bool TryGetSettings<TSettings>(out TSettings settings) where TSettings : ISettings
    {
        settings = (TSettings)_settings.FirstOrDefault(s => IsAssignableFrom<TSettings>(s.GetType()));
        return settings != null;
    }

    private static bool IsAssignableFrom<TSettings>(Type type)
    {
        return typeof(TSettings).IsAssignableFrom(type);
    }
}