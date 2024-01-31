using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace Scaffolding.Logging.Formatter;

public sealed class CustomJsonConsoleFormatter : ConsoleFormatter, IDisposable
{
    private readonly IDisposable? _optionsReloadToken;

    private CustomJsonConsoleFormatterOptions _formatterOptions;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public CustomJsonConsoleFormatter(IOptionsMonitor<CustomJsonConsoleFormatterOptions> options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        : base(nameof(CustomJsonConsoleFormatter))
    {
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        _formatterOptions = options.CurrentValue;
    }

    private void ReloadLoggerOptions(CustomJsonConsoleFormatterOptions options) =>
        _formatterOptions = options;

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (logEntry.Exception == null && message == null)
        {
            return;
        }

        var namingPolicy = _formatterOptions.GetJsonNamingPolicy();

        LogLevel logLevel = logEntry.LogLevel;
        string category = logEntry.Category;
        int eventId = logEntry.EventId.Id;
        Exception? exception = logEntry.Exception;
        const int DefaultBufferSize = 1024;
        using (var output = new PooledByteBufferWriter(DefaultBufferSize))
        {
            using (var writer = new Utf8JsonWriter(output, _formatterOptions.JsonWriterOptions))
            {
                writer.WriteStartObject();
                var timestampFormat = _formatterOptions.TimestampFormat;
                if (timestampFormat != null)
                {
                    DateTimeOffset dateTimeOffset = _formatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
                    writer.WriteString(namingPolicy.ConvertName("Timestamp"), dateTimeOffset.ToString(timestampFormat));
                }
                writer.WriteNumber(namingPolicy.ConvertName(nameof(logEntry.EventId)), eventId);
                writer.WriteString(namingPolicy.ConvertName(nameof(logEntry.LogLevel)), GetLogLevelString(logLevel));
                writer.WriteString(namingPolicy.ConvertName(nameof(logEntry.Category)), category);
                writer.WriteString(namingPolicy.ConvertName("Message"), message);

                if (exception != null)
                {
                    writer.WriteString(namingPolicy.ConvertName(nameof(Exception)), exception.ToString());
                }

                if (logEntry.State != null)
                {
                    writer.WriteStartObject(nameof(logEntry.State));
                    writer.WriteString(namingPolicy.ConvertName("Message"), logEntry.State.ToString());
                    if (logEntry.State is IReadOnlyCollection<KeyValuePair<string, object>> stateProperties)
                    {
                        foreach (KeyValuePair<string, object> item in stateProperties)
                        {
                            WriteItem(writer, item, namingPolicy);
                        }
                    }
                    writer.WriteEndObject();
                }
                WriteScopeInformation(writer, scopeProvider, namingPolicy);
                writer.WriteEndObject();
                writer.Flush();
            }
#if NETCOREAPP
            textWriter.Write(Encoding.UTF8.GetString(output.WrittenMemory.Span));
#else
                textWriter.Write(Encoding.UTF8.GetString(output.WrittenMemory.Span.ToArray()));
#endif
        }
        textWriter.Write(Environment.NewLine);
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "Trace",
            LogLevel.Debug => "Debug",
            LogLevel.Information => "Information",
            LogLevel.Warning => "Warning",
            LogLevel.Error => "Error",
            LogLevel.Critical => "Critical",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }

    private void WriteScopeInformation(Utf8JsonWriter writer, IExternalScopeProvider? scopeProvider, JsonNamingPolicy namingPolicy)
    {
        if (_formatterOptions.IncludeScopes && scopeProvider != null)
        {
            writer.WriteStartArray(namingPolicy.ConvertName("Scopes"));
            scopeProvider.ForEachScope((scope, state) =>
            {
                if (scope is IEnumerable<KeyValuePair<string, object>> scopeItems)
                {
                    state.WriteStartObject();
                    state.WriteString(namingPolicy.ConvertName("Message"), scope.ToString());
                    foreach (KeyValuePair<string, object> item in scopeItems)
                    {
                        WriteItem(state, item, namingPolicy);
                    }
                    state.WriteEndObject();
                }
                else
                {
                    state.WriteStringValue(ToInvariantString(scope));
                }
            }, writer);
            writer.WriteEndArray();
        }
    }

    private static void WriteItem(Utf8JsonWriter writer, KeyValuePair<string, object> item, JsonNamingPolicy namingPolicy)
    {
        var key = namingPolicy.ConvertName(item.Key);
        switch (item.Value)
        {
            case bool boolValue:
                writer.WriteBoolean(key, boolValue);
                break;
            case byte byteValue:
                writer.WriteNumber(key, byteValue);
                break;
            case sbyte sbyteValue:
                writer.WriteNumber(key, sbyteValue);
                break;
            case char charValue:
#if NETCOREAPP
                writer.WriteString(key, MemoryMarshal.CreateSpan(ref charValue, 1));
#else
                    writer.WriteString(key, charValue.ToString());
#endif
                break;
            case decimal decimalValue:
                writer.WriteNumber(key, decimalValue);
                break;
            case double doubleValue:
                writer.WriteNumber(key, doubleValue);
                break;
            case float floatValue:
                writer.WriteNumber(key, floatValue);
                break;
            case int intValue:
                writer.WriteNumber(key, intValue);
                break;
            case uint uintValue:
                writer.WriteNumber(key, uintValue);
                break;
            case long longValue:
                writer.WriteNumber(key, longValue);
                break;
            case ulong ulongValue:
                writer.WriteNumber(key, ulongValue);
                break;
            case short shortValue:
                writer.WriteNumber(key, shortValue);
                break;
            case ushort ushortValue:
                writer.WriteNumber(key, ushortValue);
                break;
            case null:
                writer.WriteNull(key);
                break;
            default:
                writer.WriteString(key, ToInvariantString(item.Value));
                break;
        }
    }

    private static string? ToInvariantString(object? obj) => Convert.ToString(obj, CultureInfo.InvariantCulture);

    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }
}

public sealed class CustomJsonConsoleFormatterOptions : ConsoleFormatterOptions
{
    public string NamingPolicy { get; set; } = "SnakeCaseLower";

    public JsonNamingPolicy GetJsonNamingPolicy()
        => this.NamingPolicy switch
        {
            "CamelCase" => JsonNamingPolicy.CamelCase,
            "SnakeCaseLower" => JsonNamingPolicy.SnakeCaseLower,
            "SnakeCaseUpper" => JsonNamingPolicy.SnakeCaseUpper,
            "KebabCaseLower" => JsonNamingPolicy.KebabCaseLower,
            "KebabCaseUpper" => JsonNamingPolicy.KebabCaseUpper,
            _ => throw new KeyNotFoundException()
        };

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonConsoleFormatterOptions"/> class.
    /// </summary>
    public CustomJsonConsoleFormatterOptions() { }

    /// <summary>
    /// Gets or sets JsonWriterOptions.
    /// </summary>
    public JsonWriterOptions JsonWriterOptions { get; set; }
}

internal sealed class PooledByteBufferWriter : IBufferWriter<byte>, IDisposable
{
    // This class allows two possible configurations: if rentedBuffer is not null then
    // it can be used as an IBufferWriter and holds a buffer that should eventually be
    // returned to the shared pool. If rentedBuffer is null, then the instance is in a
    // cleared/disposed state and it must re-rent a buffer before it can be used again.
    private byte[]? _rentedBuffer;
    private int _index;

    private const int MinimumBufferSize = 256;

    // Value copied from Array.MaxLength in System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Array.cs.
    public const int MaximumBufferSize = 0X7FFFFFC7;

    private PooledByteBufferWriter()
    {
#if NETCOREAPP
        // Ensure we are in sync with the Array.MaxLength implementation.
        Debug.Assert(MaximumBufferSize == Array.MaxLength);
#endif
    }

    public PooledByteBufferWriter(int initialCapacity) : this()
    {
        Debug.Assert(initialCapacity > 0);

        _rentedBuffer = ArrayPool<byte>.Shared.Rent(initialCapacity);
        _index = 0;
    }

    public ReadOnlyMemory<byte> WrittenMemory
    {
        get
        {
            Debug.Assert(_rentedBuffer != null);
            Debug.Assert(_index <= _rentedBuffer.Length);
            return _rentedBuffer.AsMemory(0, _index);
        }
    }

    public int WrittenCount
    {
        get
        {
            Debug.Assert(_rentedBuffer != null);
            return _index;
        }
    }

    public int Capacity
    {
        get
        {
            Debug.Assert(_rentedBuffer != null);
            return _rentedBuffer.Length;
        }
    }

    public int FreeCapacity
    {
        get
        {
            Debug.Assert(_rentedBuffer != null);
            return _rentedBuffer.Length - _index;
        }
    }

    public void Clear()
    {
        ClearHelper();
    }

    public void ClearAndReturnBuffers()
    {
        Debug.Assert(_rentedBuffer != null);

        ClearHelper();
        byte[] toReturn = _rentedBuffer;
        _rentedBuffer = null;
        ArrayPool<byte>.Shared.Return(toReturn);
    }

    private void ClearHelper()
    {
        Debug.Assert(_rentedBuffer != null);
        Debug.Assert(_index <= _rentedBuffer.Length);

        _rentedBuffer.AsSpan(0, _index).Clear();
        _index = 0;
    }

    // Returns the rented buffer back to the pool
    public void Dispose()
    {
        if (_rentedBuffer == null)
        {
            return;
        }

        ClearHelper();
        byte[] toReturn = _rentedBuffer;
        _rentedBuffer = null;
        ArrayPool<byte>.Shared.Return(toReturn);
    }

    public void InitializeEmptyInstance(int initialCapacity)
    {
        Debug.Assert(initialCapacity > 0);
        Debug.Assert(_rentedBuffer is null);

        _rentedBuffer = ArrayPool<byte>.Shared.Rent(initialCapacity);
        _index = 0;
    }

    public static PooledByteBufferWriter CreateEmptyInstanceForCaching() => new PooledByteBufferWriter();

    public void Advance(int count)
    {
        Debug.Assert(_rentedBuffer != null);
        Debug.Assert(count >= 0);
        Debug.Assert(_index <= _rentedBuffer.Length - count);
        _index += count;
    }

    public Memory<byte> GetMemory(int sizeHint = MinimumBufferSize)
    {
        CheckAndResizeBuffer(sizeHint);
        return _rentedBuffer.AsMemory(_index);
    }

    public Span<byte> GetSpan(int sizeHint = MinimumBufferSize)
    {
        CheckAndResizeBuffer(sizeHint);
        return _rentedBuffer.AsSpan(_index);
    }

#if NETCOREAPP
    internal ValueTask WriteToStreamAsync(Stream destination, CancellationToken cancellationToken)
    {
        return destination.WriteAsync(WrittenMemory, cancellationToken);
    }

    internal void WriteToStream(Stream destination)
    {
        destination.Write(WrittenMemory.Span);
    }
#else
        internal Task WriteToStreamAsync(Stream destination, CancellationToken cancellationToken)
        {
            Debug.Assert(_rentedBuffer != null);
            return destination.WriteAsync(_rentedBuffer, 0, _index, cancellationToken);
        }

        internal void WriteToStream(Stream destination)
        {
            Debug.Assert(_rentedBuffer != null);
            destination.Write(_rentedBuffer, 0, _index);
        }
#endif

    private void CheckAndResizeBuffer(int sizeHint)
    {
        Debug.Assert(_rentedBuffer != null);
        Debug.Assert(sizeHint > 0);

        int currentLength = _rentedBuffer.Length;
        int availableSpace = currentLength - _index;

        // If we've reached ~1GB written, grow to the maximum buffer
        // length to avoid incessant minimal growths causing perf issues.
        if (_index >= MaximumBufferSize / 2)
        {
            sizeHint = Math.Max(sizeHint, MaximumBufferSize - currentLength);
        }

        if (sizeHint > availableSpace)
        {
            int growBy = Math.Max(sizeHint, currentLength);

            int newSize = currentLength + growBy;

            if ((uint)newSize > MaximumBufferSize)
            {
                newSize = currentLength + sizeHint;
                if ((uint)newSize > MaximumBufferSize)
                {
                    throw new OutOfMemoryException();
                }
            }

            byte[] oldBuffer = _rentedBuffer;

            _rentedBuffer = ArrayPool<byte>.Shared.Rent(newSize);

            Debug.Assert(oldBuffer.Length >= _index);
            Debug.Assert(_rentedBuffer.Length >= _index);

            Span<byte> oldBufferAsSpan = oldBuffer.AsSpan(0, _index);
            oldBufferAsSpan.CopyTo(_rentedBuffer);
            oldBufferAsSpan.Clear();
            ArrayPool<byte>.Shared.Return(oldBuffer);
        }

        Debug.Assert(_rentedBuffer.Length - _index > 0);
        Debug.Assert(_rentedBuffer.Length - _index >= sizeHint);
    }
}