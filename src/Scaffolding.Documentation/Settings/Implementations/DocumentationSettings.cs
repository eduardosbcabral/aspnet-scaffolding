using Scaffolding.Configuration.Implementations;
using Scaffolding.Configuration.Settings;
using Scaffolding.Documentation.Settings;

using System.ComponentModel.DataAnnotations;

namespace Scaffolding.Logging.Settings.Implementations;

public class DocumentationSettings(string section) : BaseSettings(section), IDocumentationSettings
{
    /// <summary>
    /// Creates a new instance of <see cref="IApplicationSettings"/> with "Application" as section.
    /// </summary>
    public DocumentationSettings() : this("Docs")
    {
    }

    /// <inheritdoc/>
    [Required]
    public bool Enabled { get; set; }
}