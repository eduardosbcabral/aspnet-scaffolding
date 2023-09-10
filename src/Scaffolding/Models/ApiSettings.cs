using Scaffolding.Extensions.Json;
using TimeZoneConverter;

namespace Scaffolding.Models
{
    public class ApiSettings
    {
        public string Name { get; set; }
        public int Port { get; set; }
        public string EnvironmentVariablesPrefix { get; set; }
        public string Version { get; set; }
        public string PathBase { get; set; }
        public string Domain { get; set; }
        public string BuildVersion { get; set; }
        public string[] SupportedCultures { get; set; }
        public string RequestKeyProperty { get; set; }
        public string TimeElapsedProperty { get; set; }
        public string TimezoneHeader { get; set; }
        public string TimezoneDefault { get; set; }
        public JsonSerializerEnum JsonSerializer { get; set; }
        public TimeZoneInfo TimezoneDefaultInfo => TZConvert.GetTimeZoneInfo(TimezoneDefault);

        public ApiSettings()
        {
            Name = "DefaultApp";
            Port = 8000;
            Domain = "DefaultDomain";
            BuildVersion = "1.0.0";
        }

        public string GetFullPath()
        {
            if (!PathBase.StartsWith('/'))
            {
                PathBase = $"/{PathBase}";
            }

            if (PathBase.Contains("{version}", StringComparison.OrdinalIgnoreCase))
            {
                return PathBase.Replace("{version}", Version, StringComparison.OrdinalIgnoreCase);
            }
            return PathBase + (!string.IsNullOrEmpty(Version) ? $"/{Version}" : "");
        }
    }

}
