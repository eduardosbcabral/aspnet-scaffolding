using AspNetCoreRateLimit;
using Microsoft.Extensions.Caching.Memory;

namespace Scaffolding.Extensions.RateLimiting;

public static class RateLimitingServiceExtension
{
    public static void SetupRateLimiting(this WebApplicationBuilder builder, RateLimitConfiguration customRateLimitingConfiguration = null)
    {
        var rateLimitingSettings = builder.Configuration.GetSection("RateLimitingSettings").Get<RateLimitingSettings>();

        if (rateLimitingSettings?.Enabled == false) { return; }

        builder.Services.AddSingleton(rateLimitingSettings);

        if (IpRateLimitingIsConfigured())
        {
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("RateLimitingSettings:IpRateLimiting"));
            builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("RateLimitingSettings:IpRateLimitPolicies"));
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        if (ClientRateLimitingIsConfigured())
        {
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("RateLimitingSettings:ClientRateLimiting"));
            builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("RateLimitingSettings:ClientRateLimitingPolicies"));
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        if (rateLimitingSettings.Storage == RateLimitingStorageType.Distributed)
        {
            builder.Services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

            if (IpRateLimitingIsConfigured())
            {
                builder.Services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            }

            if (ClientRateLimitingIsConfigured())
            {
                builder.Services.AddSingleton<IClientPolicyStore, DistributedCacheClientPolicyStore>();
            }
        }
        else if (rateLimitingSettings.Storage == RateLimitingStorageType.Memory)
        {
            builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            if (IpRateLimitingIsConfigured())
            {
                builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            }

            if (ClientRateLimitingIsConfigured())
            {
                builder.Services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            }
        }

        builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

        if (customRateLimitingConfiguration != null)
        {
            builder.Services.AddSingleton<IRateLimitConfiguration>(customRateLimitingConfiguration);
        }

        bool IpRateLimitingIsConfigured()
            => rateLimitingSettings.IpRateLimiting is not null;

        bool ClientRateLimitingIsConfigured()
            => rateLimitingSettings.ClientRateLimiting is not null;
    }

    public static void UseScaffoldingRateLimiting(this WebApplication app)
    {
        var rateLimitingSettings = app.Services.GetService<RateLimitingSettings>();

        if (rateLimitingSettings?.Enabled == true)
        {
            if (rateLimitingSettings.IpRateLimiting is not null)
            {
                app.UseIpRateLimiting();
            }

            if (rateLimitingSettings.ClientRateLimiting is not null)
            {
                app.UseClientRateLimiting();
            }
        }
    }
}
