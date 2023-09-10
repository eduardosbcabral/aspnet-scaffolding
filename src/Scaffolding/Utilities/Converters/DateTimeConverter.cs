using Newtonsoft.Json;
using TimeZoneConverter;

namespace Scaffolding.Utilities.Converters;

/// <summary>
///  DateTime and NullableDataTime converter for newtonsoft
///  Considering time zone and only date format
/// </summary>
internal class DateTimeConverter : JsonConverter
{
    public static TimeZoneInfo DefaultTimeZone = TZConvert.GetTimeZoneInfo("E. South America Standard Time");

    public Func<TimeZoneInfo> GetTimeZoneInfo { get; set; }

    public DateTimeConverter() { }

    public DateTimeConverter(Func<TimeZoneInfo> getTimeZoneInfo)
    {
        this.GetTimeZoneInfo = getTimeZoneInfo;
    }

    public override bool CanConvert(Type objectType)
    {
        return
            (typeof(DateTime).IsAssignableFrom(objectType)) ||
            (typeof(DateTime?).IsAssignableFrom(objectType));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (string.IsNullOrWhiteSpace(reader.Value?.ToString()))
        {
            return null;
        }

        var date = DateTime.Parse(reader.Value.ToString());

        date = TimeZoneInfo.ConvertTimeToUtc(date, GetCurrentTimeZoneInfoInvokingFunction());

        return date;
    }

    public override async void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        DateTime? convertedDate = null;

        if (value != null)
        {
            var originalDate = DateTime.Parse(value.ToString());

            convertedDate = TimeZoneInfo.ConvertTimeFromUtc(originalDate, GetCurrentTimeZoneInfoInvokingFunction());
        }

        await writer.WriteValueAsync(convertedDate);
    }

    public static TimeZoneInfo GetTimeZoneByAspNetHeader(IHttpContextAccessor httpContextAccessor, string headerName)
    {
        var httpContext = httpContextAccessor.HttpContext;

        try
        {
            var timezone = httpContext.Request.Headers[headerName];
            return TZConvert.GetTimeZoneInfo(timezone);
        }
        catch (Exception)
        {
            return DefaultTimeZone;
        }
    }

    private TimeZoneInfo GetCurrentTimeZoneInfoInvokingFunction()
    {
        return this.GetTimeZoneInfo?.Invoke() ?? DefaultTimeZone;
    }
}

internal class DateConverter : JsonConverter
{
    private readonly static string _defaultFormat = "yyyy-MM-dd";

    public string Format { get; set; }

    public DateConverter() { }

    public DateConverter(string format)
    {
        this.Format = format;
    }

    public override bool CanConvert(Type objectType)
    {
        return
            (typeof(DateTime).IsAssignableFrom(objectType)) ||
            (typeof(DateTime?).IsAssignableFrom(objectType));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (string.IsNullOrWhiteSpace(reader.Value?.ToString()))
        {
            return null;
        }

        var date = DateTime.Parse(reader.Value.ToString());

        return date.Date;
    }

    public override async void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        DateTime? convertedDate = null;

        if (value != null)
        {
            convertedDate = DateTime.Parse(value.ToString());
        }

        await writer.WriteValueAsync(convertedDate?.ToString(this.Format ?? _defaultFormat));
    }
}