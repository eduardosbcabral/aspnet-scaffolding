using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Scaffolding.AspNetCore.RequestKey;

namespace Scaffolding.AspNetCore.Extensions;

public static class RequestKeyExtensions
{
    public static string RequestKeyHeaderName = "RequestKey";

    public static IServiceCollection AddRequestKey(this IServiceCollection services, string customHeaderName = "")
    {
        if (string.IsNullOrWhiteSpace(customHeaderName) == false)
        {
            RequestKeyHeaderName = customHeaderName;
        }

        services.AddScoped<RequestKeyMiddleware>();
        services.AddScoped(obj => new RequestKey.RequestKey());
        return services;
    }

    public static IApplicationBuilder UseRequestKey(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestKeyMiddleware>();
    }
}
