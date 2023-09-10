using AspNetCoreRateLimit;

namespace Scaffolding.Extensions.RateLimiting;

internal class RateLimitingSettings
{
    public bool Enabled { get; set; }
    public RateLimitingStorageType Storage { get; set; } = RateLimitingStorageType.Memory;
    public IpRateLimitOptions IpRateLimiting { get; set; }
    public ClientRateLimitOptions ClientRateLimiting { get; set; }
}
