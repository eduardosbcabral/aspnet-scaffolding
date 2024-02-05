using Serilog.Core;
using Serilog.Events;
namespace Scaffolding.Logging.Serilog.Enrichers;

public class RemovePropertiesEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent le, ILogEventPropertyFactory lepf)
    {
        le.RemovePropertyIfPresent("SourceContext");
        le.RemovePropertyIfPresent("RequestId");
        le.RemovePropertyIfPresent("RequestPath");
        le.RemovePropertyIfPresent("Scheme");
        le.RemovePropertyIfPresent("ConnectionId");
    }
}