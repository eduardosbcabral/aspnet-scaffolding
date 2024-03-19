using Microsoft.AspNetCore.Http;

namespace Scaffolding.Logging;

public static class DisableLoggingExtension
{
    internal const string KEY = "DisableLogging";

    public static void DisableLogging(this IHttpContextAccessor httpContextAccessor)
    {
        httpContextAccessor.HttpContext.Items.Add(KEY, true);
    }
}

