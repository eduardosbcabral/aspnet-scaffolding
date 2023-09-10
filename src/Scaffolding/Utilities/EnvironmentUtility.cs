namespace Scaffolding.Utilities;

public class EnvironmentUtility
{
    public static string GetCurrentEnvironment()
        => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

    public static bool IsDevelopment()
    {
        var env = GetCurrentEnvironment();
        return env.Contains("dev", StringComparison.InvariantCultureIgnoreCase);
    }
}
