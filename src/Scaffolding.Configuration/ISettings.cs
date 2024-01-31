namespace Scaffolding.Configuration;

/// <summary>
/// Specifies basic settings contract.
/// </summary>
public interface ISettings
{
    /// <summary>
    /// The appsettings file section to be loaded from.
    /// </summary>
    /// <value></value>
    string Section { get; }

    /// <summary>
    /// Executes settings validation, throwing exception if is invalid.
    /// </summary>
    bool IsValid(out string validationMessage);

    /// <summary>
    /// Sets appSettings section.
    /// </summary>
    void SetSection(string section);
}