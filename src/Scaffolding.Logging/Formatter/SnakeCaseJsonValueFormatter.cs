using Serilog.Events;
using Serilog.Formatting.Json;

using System.Text.Json;

namespace Scaffolding.Logging.Formatter;

internal class SnakeCaseJsonValueFormatter(JsonNamingPolicy jsonPolicy, string typeTagName = "_typeTag") : JsonValueFormatter(typeTagName)
{
    private readonly string _typeTagName = typeTagName;
    private readonly JsonNamingPolicy _jsonPolicy = jsonPolicy;

    protected override bool VisitStructureValue(TextWriter state, StructureValue structure)
    {
        state.Write('{');
        string value = "";
        for (int i = 0; i < structure.Properties.Count; i++)
        {
            state.Write(value);
            value = ",";
            LogEventProperty logEventProperty = structure.Properties[i];
            WriteQuotedJsonString(_jsonPolicy.ConvertName(logEventProperty.Name), state);
            state.Write(':');
            Visit(state, logEventProperty.Value);
        }

        if (_typeTagName != null && structure.TypeTag != null)
        {
            state.Write(value);
            WriteQuotedJsonString(_typeTagName, state);
            state.Write(':');
            WriteQuotedJsonString(structure.TypeTag, state);
        }

        state.Write('}');
        return false;
    }
}
