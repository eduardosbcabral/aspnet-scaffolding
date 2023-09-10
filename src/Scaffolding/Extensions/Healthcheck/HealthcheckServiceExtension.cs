﻿namespace Scaffolding.Extensions.Healthcheck;

public static class HealthcheckServiceExtension
{
    public static IHealthChecksBuilder SetupHealthcheck(this WebApplicationBuilder builder)
    {
        var healthcheckSettings = builder.Configuration.GetSection("HealthcheckSettings").Get<HealthcheckSettings>();
        builder.Services.AddSingleton(healthcheckSettings);

        if (healthcheckSettings?.Enabled == true)
        {
            return builder.Services.AddHealthChecks();
        }

        return null;
    }
}
