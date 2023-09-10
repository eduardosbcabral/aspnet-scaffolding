using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Scaffolding.Extensions.QueryFormatter;
using Scaffolding.Extensions.RoutePrefix;
using Scaffolding.Models;
using Scaffolding.Utilities;
using Scaffolding.Utilities.Converters;

namespace Scaffolding.Extensions.Json;

public static class JsonSerializerServiceExtension
{
    public static JsonSerializerSettings JsonSerializerSettings { get; set; }

    public static JsonSerializer JsonSerializer { get; set; }

    public static void ConfigureJsonSettings(this WebApplicationBuilder builder)
    {
        var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>();

        var jsonSerializerMode = apiSettings.JsonSerializer;
        CaseUtility.JsonSerializerMode = jsonSerializerMode;

        JsonSerializerSettings = null;
        JsonSerializer = null;

        switch (jsonSerializerMode)
        {
            case JsonSerializerEnum.Camelcase:
                JsonSerializer = JsonUtility.CamelCaseJsonSerializer;
                JsonSerializerSettings = JsonUtility.CamelCaseJsonSerializerSettings;
                break;
            case JsonSerializerEnum.Lowercase:
                JsonSerializer = JsonUtility.LowerCaseJsonSerializer;
                JsonSerializerSettings = JsonUtility.LowerCaseJsonSerializerSettings;
                break;
            case JsonSerializerEnum.Snakecase:
                JsonSerializer = JsonUtility.SnakeCaseJsonSerializer;
                JsonSerializerSettings = JsonUtility.SnakeCaseJsonSerializerSettings;
                break;
            default:
                break;
        }

        JsonConvert.DefaultSettings = () => JsonSerializerSettings;

        builder.Services.AddSingleton(x => JsonSerializer);
        builder.Services.AddSingleton(x => JsonSerializerSettings);

        DateTimeConverter.DefaultTimeZone = apiSettings.TimezoneDefaultInfo;
    }

    public static void SetupScaffoldingJsonSettings(this IMvcBuilder mvc, ApiSettings apiSettings, IHttpContextAccessor httpContextAccessor)
    {
        mvc.AddMvcOptions(options =>
            {
                // Configure query string formatter by json serializer mode
                options.AddQueryFormatter(CaseUtility.JsonSerializerMode);
                // Configure json property formatter by json serializer mode
                options.AddPathFormatter(CaseUtility.JsonSerializerMode);
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = JsonSerializerSettings.ContractResolver;
                options.SerializerSettings.NullValueHandling = JsonSerializerSettings.NullValueHandling;
                options.SerializerSettings.Converters.Add(new DateTimeConverter(() =>
                {
                    return DateTimeConverter.GetTimeZoneByAspNetHeader(httpContextAccessor, apiSettings.TimezoneHeader);
                }));

                foreach (var converter in JsonSerializerSettings.Converters)
                {
                    options.SerializerSettings.Converters.Add(converter);
                }
            });
    }
}
