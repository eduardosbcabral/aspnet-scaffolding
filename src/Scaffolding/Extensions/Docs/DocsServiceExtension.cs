using Microsoft.OpenApi.Models;
using Scaffolding.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using static Scaffolding.Utilities.SwaggerUtilities;

namespace Scaffolding.Extensions.Docs;

public static class DocsServiceExtension
{
    public static DocsSettings DocsSettings { get; private set; }
    private const string DEFAULT_VERSION_IF_NOT_FILLED = "v1";

    public static void SetupSwaggerDocs(this WebApplicationBuilder builder)
    {
        var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>();
        var docsSettings = builder.Configuration.GetSection("DocsSettings").Get<DocsSettings>();

        if (docsSettings?.Enabled == true)
        {
            try
            {
                GenerateSwaggerUrl(docsSettings, apiSettings);

                builder.Services.AddSwaggerGen(options =>
                {
                    var readme = string.Empty;
                    try
                    {
                        if (string.IsNullOrWhiteSpace(docsSettings.PathToReadme) == false)
                        {
                            readme = File.ReadAllText(docsSettings.PathToReadme);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"[ERROR] Swagger markdown ({docsSettings.PathToReadme}) could not be loaded.");
                    }

                    options.SchemaFilter<OriginalEnumSchemaFilter>();
                    SwaggerEnum.Enums = docsSettings.IgnoredEnums;

                    options.CustomSchemaIds(x => x.FullName);
                    options.CustomOperationIds(apiDesc =>
                    {
                        return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)
                           ? methodInfo.Name : null;
                    });

                    options.IgnoreObsoleteActions();
                    options.IgnoreObsoleteProperties();

                    options.SchemaFilter<SwaggerExcludeFilter>();
                    options.OperationFilter<QueryAndPathCaseOperationFilter>();
                    var version = string.IsNullOrEmpty(apiSettings.Version)
                            ? DEFAULT_VERSION_IF_NOT_FILLED
                            : apiSettings.Version;
                    options.SwaggerDoc(version, new OpenApiInfo
                    {
                        Title = docsSettings.Title,
                        Version = version,
                        Description = readme,
                        Contact = new OpenApiContact
                        {
                            Name = docsSettings.AuthorName,
                            Email = docsSettings.AuthorEmail
                        }
                    });
                });
                DocsSettings = docsSettings;
                builder.Services.AddSingleton(docsSettings);
                builder.Services.AddSwaggerGenNewtonsoftSupport();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] Swagger exception: {e.Message}");
            }
        }
    }

    private static void GenerateSwaggerUrl(DocsSettings docsSettings, ApiSettings apiSettings)
    {
        var version = string.IsNullOrEmpty(apiSettings.Version) ? DEFAULT_VERSION_IF_NOT_FILLED : apiSettings.Version;

        docsSettings.SwaggerJsonTemplateUrl = "/swagger/{documentName}/swagger.json";
        docsSettings.SwaggerJsonUrl = $"/swagger/{version}/swagger.json";
        docsSettings.RedocUrl = apiSettings.GetFullPath().Trim('/') + "/docs";
    }
}
