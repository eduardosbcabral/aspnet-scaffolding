namespace Scaffolding.Extensions.Cors;

public static class CorsMiddlewareExtension
{
    public static void AllowCors(this IApplicationBuilder app)
    {
        app.UseCors(CorsServiceExtension.CorsName);
    }
}