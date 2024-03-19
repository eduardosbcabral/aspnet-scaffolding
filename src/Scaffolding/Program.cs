using Destructurama.Attributed;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json.Serialization;

using Scaffolding.AspNetCore.Filters;
using Scaffolding.Configuration.Builders;
using Scaffolding.Configuration.Settings.Implementations;
using Scaffolding.HealthCheck.Extensions;
using Scaffolding.Logging.Settings.Implementations;

var builder = WebApplication.CreateBuilder(args);

var scaffoldingConfiguration = new ScaffoldingConfigurationBuilder(builder.Configuration)
    .With<ApplicationSettings>()
    .With<LogSettings>()
    .Build();

builder.Configuration.AddUserSecrets<Program>();

builder.Services
    .AddScaffoldingSettings(scaffoldingConfiguration)
    .ConfigureJson(new SnakeCaseNamingStrategy());

builder.Services.AddSerilogRequestResponseLogging();

var app = builder.Build();

app.UseSerilogLogging()
    .UseScaffoldingHealthCheck();

app.MapGet("/", () => "Application Running");
app.MapPost("/post", (Response req) => new Response
{
    Email = "a@a.com",
    Password = "123456",
}).AddEndpointFilter<RequestResponseLoggingEndpointFilter>();

await app.RunAsync();

class Response
{
    public string Email { get; set; }
    [LogMasked]
    public string Password { get; set; }
}