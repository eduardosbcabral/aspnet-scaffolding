using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

using Scaffolding.Configuration.Builders;
using Scaffolding.Configuration.Settings.Implementations;
using Scaffolding.HealthCheck.Extensions;
using Scaffolding.Logging.Formatter;
using Scaffolding.Logging.Settings.Implementations;

using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var scaffoldingConfiguration = new ScaffoldingConfigurationBuilder(builder.Configuration)
    .With<ApplicationSettings>()
    .With<LogSettings>("Log")
    .Build();

var applicationSettings = scaffoldingConfiguration.GetSettings<ApplicationSettings>();
var logSettings = scaffoldingConfiguration.GetSettings<LogSettings>();

//var logger = LoggerBuilder.Instance()
//    .WithDefaultConfiguration()
//    .WithProperty("Application", applicationSettings.Name)
//    .WithProperty("Domain", applicationSettings.Domain)
//    .WriteToConsoleSnakeCase()
//    .Build();

var services = builder.Services;

services.AddSingletonScaffoldingServices(scaffoldingConfiguration);

//services.AddScoped(x => logger);

services.AddScaffoldingHealthChecks();

builder.Logging
    .AddConsole(x => x.FormatterName = nameof(CustomJsonConsoleFormatter))
    .AddConsoleFormatter<CustomJsonConsoleFormatter, CustomJsonConsoleFormatterOptions>();

builder.Services.AddHttpLogging(x =>
{
    x.CombineLogs = true;
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

var app = builder.Build();

app.UseScaffoldingHealthCheck();

app.UseHttpLogging();

app.MapGet("/", () => "Application Running");

var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Teste Template ILogger Microsoft Scaffolding");

await app.RunAsync();

//var builder = ScaffoldingApi.Initialize(args);

//builder.UseLogging();

//var healthcheckBuilder = builder.SetupHealthcheck();

//// Example
//void AddHealthchecks()
//{
//    // var externalService (Service instance after configuration and injection)
//    // healthcheckBuilder.AddUrlGroup(new Uri(externalService.Name), name: "External Service Name");

//    healthcheckBuilder.AddUrlGroup(new Uri("https://www.google.com.br"), name: "Google", tags: new[] { "external" });
//}

//AddHealthchecks();

//builder.Services.AddMemoryCache();

//var app = builder.Build();

//app.UseDefaultConfiguration();

//await ScaffoldingApi.RunAsync(app);