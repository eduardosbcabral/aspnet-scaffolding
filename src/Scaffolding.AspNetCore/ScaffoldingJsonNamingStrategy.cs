using Newtonsoft.Json.Serialization;

using System.Text.Json;

namespace Scaffolding.AspNetCore;

public class ScaffoldingJsonNamingStrategy
{
    public static NamingStrategy DEFAULT_STRATEGY { get; set; } = new SnakeCaseNamingStrategy();
    public static Type GET_DEFAULT_STRATEGY_TYPE => DEFAULT_STRATEGY.GetType();

}
