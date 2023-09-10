using Microsoft.AspNetCore.Mvc;

namespace Scaffolding.Extensions.Logging;

public static class DisableLoggingExtension
{
    internal const string ITEM_NAME = "DisableLogging";

    public static void DisableLogging(this HttpContext context)
    {
        context?.Items.Add("DisableLogging", true);
    }

    public static void DisableLogging(this ControllerBase controller)
    {
        controller?.HttpContext?.Items?.Add("DisableLogging", true);
    }
}
