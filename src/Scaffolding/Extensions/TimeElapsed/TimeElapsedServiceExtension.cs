namespace Scaffolding.Extensions.TimeElapsed;

internal static class TimeElapsedServiceExtension
{
    internal static string TimeElapsedHeaderName = "X-Internal-Time";

    public static void SetupTimeElapsed(this WebApplicationBuilder _, string headerName = null)
    {
        if (string.IsNullOrWhiteSpace(headerName) == false)
        {
            TimeElapsedHeaderName = headerName;
        }
    }
}
