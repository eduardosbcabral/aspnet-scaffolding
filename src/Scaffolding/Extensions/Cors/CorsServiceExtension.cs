namespace Scaffolding.Extensions.Cors;

public static class CorsServiceExtension
{
    public const string CorsName = "EnableAll";

    public static void SetupAllowCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(o => o.AddPolicy(CorsName, builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }));
    }
}