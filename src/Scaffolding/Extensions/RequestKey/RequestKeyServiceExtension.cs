namespace Scaffolding.Extensions.RequestKey;

internal static class RequestKeyServiceExtension
{
    internal static string RequestKeyHeaderName = "RequestKey";

    public static void SetupRequestKey(this WebApplicationBuilder builder, string headerName = null)
    {
        if (string.IsNullOrWhiteSpace(headerName) == false)
        {
            RequestKeyHeaderName = headerName;
        }

        builder.Services.AddScoped<RequestKeyMiddleware>();
        builder.Services.AddScoped(x => new RequestKey());
    }
}
