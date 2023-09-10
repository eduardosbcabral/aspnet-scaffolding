using Microsoft.AspNetCore.Localization;
using Scaffolding.Models;

namespace Scaffolding.Extensions.CultureInfo;

public static class CultureInfoMiddlewareExtension
{
    public static void UseScaffoldingRequestLocalization(this WebApplication app)
    {
        var apiSettings = app.Services.GetService<ApiSettings>();

        if (apiSettings.SupportedCultures?.Any() == true)
        {
            app.UseRequestLocalization(options =>
            {
                options.AddSupportedCultures(apiSettings.SupportedCultures);
                options.AddSupportedUICultures(apiSettings.SupportedCultures);
                options.SetDefaultCulture(apiSettings.SupportedCultures.FirstOrDefault());
                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new AcceptLanguageHeaderRequestCultureProvider { Options = options }
                };
            });
        }
    }
}