namespace Scaffolding.Extensions.Docs;

internal static class DocsMiddlewareExtension
{
    public static void UseScaffoldingDocumentation(this IApplicationBuilder app)
    {
        var docsSettings = app.ApplicationServices.GetService<DocsSettings>();

        if (docsSettings?.Enabled == true)
        {
            var title = docsSettings?.Title ?? "API Reference";

            app.UseStaticFiles();
            app.UseDirectoryBrowser();
            app.UseSwagger(c =>
            {
                c.RouteTemplate = docsSettings.SwaggerJsonTemplateUrl.TrimStart('/');
            });

            app.UseReDoc(c =>
            {
                c.RoutePrefix = docsSettings.RedocUrl.TrimStart('/');
                c.SpecUrl = docsSettings.SwaggerJsonUrl;
                c.DocumentTitle = title;
            });
        }
    }
}
