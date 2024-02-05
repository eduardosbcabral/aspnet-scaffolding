using Scaffolding.Configuration;

namespace Scaffolding.Documentation.Settings;

public interface IDocumentationSettings : ISettings
{
    /// <summary>
    /// Enable application documentation.
    /// </summary>
    bool Enabled { get; set; }
}
