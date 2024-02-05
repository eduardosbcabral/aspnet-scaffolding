using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Scaffolding.Configuration;
using Scaffolding.Configuration.Settings.Implementations;
using Scaffolding.Logging.Settings.Implementations;

namespace Scaffolding.Documentation.Extensions;

public static class DocumentationExtensions
{
    public static IServiceCollection AddScaffoldingDocumentation(this IServiceCollection services, IScaffoldingConfiguration configuration)
    {
        var settings = configuration.GetSettings<DocumentationSettings>();
        var applicationSettings = configuration.GetSettings<ApplicationSettings>();
        if (settings?.Enabled == true)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc(applicationSettings.Version, new()
                {
                    Version = applicationSettings.Version,
                    Title = applicationSettings.Name
                });
            }).AddSwaggerGenNewtonsoftSupport();
        }
        return services;
    }

    public static IApplicationBuilder UseScaffoldingDocumentation(this IApplicationBuilder app, IScaffoldingConfiguration configuration, string basePath = "")
    {
        var settings = configuration.GetSettings<DocumentationSettings>();
        var applicationSettings = configuration.GetSettings<ApplicationSettings>();
        if (settings?.Enabled == true)
        {
            var path = basePath.TrimStart('/');
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = $"{path}/swagger";
                options.SwaggerEndpoint($"/{path}/swagger/{applicationSettings.Version}/swagger.json", applicationSettings.Name);
            });
            app.UseSwagger(c =>
            {
                c.RouteTemplate = path + "/swagger/{documentname}/swagger.json";
            });
        }
        return app;
    }
}
