namespace Scaffolding.Extensions.Healthcheck;

public static class HealthcheckServiceExtension
{
    public static IHealthChecksBuilder SetupHealthcheck(this WebApplicationBuilder builder)
    {
        var healthcheckSettings = builder.Configuration.GetSection("HealthcheckSettings").Get<HealthcheckSettings>();
        
        if (healthcheckSettings == null) return null;
        
        if (healthcheckSettings?.Enabled == true)
        {
            builder.Services.AddSingleton(healthcheckSettings);
            return builder.Services.AddHealthChecks();
        }

        return null;
    }
}
