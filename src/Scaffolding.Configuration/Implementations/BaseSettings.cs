using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Scaffolding.Configuration.Implementations;

/// <summary>
/// Basic implementation of <see cref="ISettings"/>
/// </summary>
public class BaseSettings : ISettings
{
    /// <summary>
    /// Creates a new instance of <see cref="BaseSettings"/>.
    /// </summary>
    /// <param name="section">The configuration section.</param>
    public BaseSettings(string section)
    {
        Section = section;
    }

    /// <inheritdoc/>
    public string Section { get; private set; }

    /// <inheritdoc/>
    public virtual bool IsValid(out string validationMessage)
    {
        validationMessage = string.Empty;
        var validations = new List<string>();
        if (string.IsNullOrWhiteSpace(Section))
        {
            validations.Add($"The {nameof(Section)} field is required.");
        }

        var objectProperties = TypeDescriptor.GetProperties(GetType());
        foreach (PropertyDescriptor propertyDescriptor in objectProperties)
        {
            var requiredAttribute = (RequiredAttribute)propertyDescriptor.Attributes[typeof(RequiredAttribute)];
            if (requiredAttribute is null) continue;

            var value = GetType().GetProperty(propertyDescriptor.Name)?.GetValue(this);
            if (!requiredAttribute.IsValid(value))
            {
                validations.Add(requiredAttribute.FormatErrorMessage(propertyDescriptor.Name));
            }
        }

        if (!validations.Any()) return true;

        var name = string.IsNullOrWhiteSpace(Section) ? GetType().FullName : Section;
        validations.Insert(0, $"{name} settings validations: \n");
        validationMessage = string.Join("\n", validations);
        return false;
    }

    public virtual void SetSection(string section)
    {
        if (!string.IsNullOrEmpty(section))
        {
            Section = section;
        }
    }
}