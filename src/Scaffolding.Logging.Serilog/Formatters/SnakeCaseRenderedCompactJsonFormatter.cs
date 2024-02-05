using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;

using System.Text.Json;

namespace Scaffolding.Logging.Formatters;

public class SnakeCaseRenderedCompactJsonFormatter(SnakeCaseJsonValueFormatter? valueFormatter = null) : ITextFormatter
{
    private static readonly JsonNamingPolicy _jsonNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    private readonly SnakeCaseJsonValueFormatter _valueFormatter = valueFormatter ?? new SnakeCaseJsonValueFormatter(_jsonNamingPolicy, null);

    //
    // Summary:
    //     Format the log event into the output. Subsequent events will be newline-delimited.
    //
    //
    // Parameters:
    //   logEvent:
    //     The event to format.
    //
    //   output:
    //     The output.
    public void Format(LogEvent logEvent, TextWriter output)
    {
        FormatEvent(logEvent, output, _valueFormatter);
        output.WriteLine();
    }

    //
    // Summary:
    //     Format the log event into the output.
    //
    // Parameters:
    //   logEvent:
    //     The event to format.
    //
    //   output:
    //     The output.
    //
    //   valueFormatter:
    //     A value formatter for Serilog.Events.LogEventPropertyValues on the event.
    public static void FormatEvent(LogEvent logEvent, TextWriter output, SnakeCaseJsonValueFormatter valueFormatter)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(valueFormatter);

        output.Write("{\"@t\":\"");
        output.Write(logEvent.Timestamp.UtcDateTime.ToString("O"));
        output.Write("\",\"@m\":");
        JsonValueFormatter.WriteQuotedJsonString(logEvent.MessageTemplate.Render(logEvent.Properties), output);
        if (logEvent.Level != LogEventLevel.Information)
        {
            output.Write(",\"@l\":\"");
            output.Write(logEvent.Level);
            output.Write('"');
        }

        if (logEvent.Exception != null)
        {
            output.Write(",\"@x\":");
            JsonValueFormatter.WriteQuotedJsonString(logEvent.Exception.ToString(), output);
        }

        foreach (KeyValuePair<string, LogEventPropertyValue> property in logEvent.Properties)
        {
            var text = _jsonNamingPolicy.ConvertName(property.Key);
            if (text.Length > 0 && text[0] == '@')
            {
                text = "@" + text;
            }

            output.Write(',');
            JsonValueFormatter.WriteQuotedJsonString(text, output);
            output.Write(':');
            valueFormatter.Format(property.Value, output);
        }

        output.Write('}');
    }
}