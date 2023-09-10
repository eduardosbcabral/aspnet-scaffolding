namespace Scaffolding.Extensions.Logging;

public class LogAdditionalInfo
{
    internal static readonly string _logAdditionalInfoItemKey = "LogAdditionalInfo";

    public LogAdditionalInfo(IHttpContextAccessor httpContextAccessor)
    {
        httpContextAccessor.HttpContext.Items.Add(_logAdditionalInfoItemKey, this);
    }

    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
}
