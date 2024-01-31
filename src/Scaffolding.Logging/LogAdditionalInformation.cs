using Microsoft.AspNetCore.Http;

namespace Scaffolding.Extensions.Logging;

public class LogAdditionalInformation
{
    internal static readonly string KEY = "LogAdditionalProperty";

    public LogAdditionalInformation(IHttpContextAccessor httpContextAccessor)
    {
        httpContextAccessor.HttpContext.Items.Add(KEY, this);
    }

    public Dictionary<string, object> Data { get; set; } = [];
}
