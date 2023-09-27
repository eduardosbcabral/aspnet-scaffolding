using Scaffolding;
using Scaffolding.Extensions.Healthcheck;

var builder = Api.Initialize(args);

builder.UseLogging();

var healthcheckBuilder = builder.SetupHealthcheck();

// Example
void AddHealthchecks()
{
    // var externalService (Service instance after configuration and injection)
    // healthcheckBuilder.AddUrlGroup(new Uri(externalService.Name), name: "External Service Name");

    healthcheckBuilder.AddUrlGroup(new Uri("https://www.google.com.br"), name: "Google", tags: new[] { "external" });
}

AddHealthchecks();

builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseDefaultConfiguration();

await Api.RunAsync(app);