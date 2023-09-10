namespace Scaffolding.Extensions.Logging;

public class LogSettings
{
    public bool DebugEnabled { get; set; }
    public string TitlePrefix { get; set; }
    public string[] JsonBlacklistRequest { get; set; }
    public string[] JsonBlacklistResponse { get; set; }
    public string[] HeaderBlacklist { get; set; }
    public string[] HttpContextBlacklist { get; set; }
    public string[] QueryStringBlacklist { get; set; }
    public string InformationTitle { get; set; }
    public string ErrorTitle { get; set; }
    public List<string> IgnoredRoutes { get; set; } = new();
    public ConsoleOptions Console { get; set; }

    public string GetInformationTitle()
    {
        if (string.IsNullOrWhiteSpace(InformationTitle))
            return CommunicationLogger.DefaultInformationTitle;

        return InformationTitle;
    }

    public string GetErrorTitle()
    {
        if (string.IsNullOrWhiteSpace(ErrorTitle))
            return CommunicationLogger.DefaultErrorTitle;

        return ErrorTitle;
    }
}
