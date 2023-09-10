namespace Scaffolding.Extensions.Logging;

public class LogConfiguration
{
    public string[] BlacklistRequest { get; set; }
    public string[] BlacklistResponse { get; set; }
    public string[] HeaderBlacklist { get; set; }
    public string[] QueryStringBlacklist { get; set; }
    public string[] HttpContextBlacklist { get; set; }
    public string InformationTitle { get; set; }
    public string ErrorTitle { get; set; }
    public string RequestKeyProperty { get; set; }
    public string TimeElapsedProperty { get; set; }
    public string Version { get; set; }
    public string[] IgnoredRoutes { get; set; }
}
