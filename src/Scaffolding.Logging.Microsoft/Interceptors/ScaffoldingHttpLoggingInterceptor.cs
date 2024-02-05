using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Primitives;

using Scaffolding.Configuration;
using Scaffolding.Configuration.Settings;
using Scaffolding.Configuration.Settings.Implementations;

namespace Scaffolding.Logging.Microsoft.Interceptors;

public class ScaffoldingHttpLoggingInterceptor(IScaffoldingConfiguration configuration) : IHttpLoggingInterceptor
{
    private readonly IApplicationSettings _applicationSettings = configuration.GetSettings<ApplicationSettings>();
    private readonly HttpLoggingOptions httpLoggingOptions = new();

    private readonly StringValues REDACTED_VALUE = "******";

    public ValueTask OnRequestAsync(HttpLoggingInterceptorContext logContext)
    {
        // Don't enrich if we're not going to log any part of the request
        if (!logContext.IsAnyEnabled(HttpLoggingFields.Request))
        {
            return default;
        }

        if (logContext.TryDisable(HttpLoggingFields.RequestHeaders))
        {
            RedactRequestHeaders(logContext);
        }

        RedactRequestBody(logContext);
        EnrichRequest(logContext);

        return default;
    }

    public ValueTask OnResponseAsync(HttpLoggingInterceptorContext logContext)
    {
        // Don't enrich if we're not going to log any part of the response
        if (!logContext.IsAnyEnabled(HttpLoggingFields.Response))
        {
            return default;
        }

        if (logContext.TryDisable(HttpLoggingFields.ResponseHeaders))
        {
            RedactResponseHeaders(logContext);
        }

        RedactResponseBody(logContext);

        return default;
    }

    private void RedactRequestBody(HttpLoggingInterceptorContext logContext)
    {
        //using (var streamReader = new HttpRequestStreamReader(logContext.HttpContext.Request.Body, Encoding.UTF8))
        //{
        //    var redacted = JsonSerializer.Serialize(streamReader, new JsonSerializerOptions().UseSensitive());
        //    logContext.AddParameter("RequestBody", redacted);
        //}
    }

    private void RedactRequestHeaders(HttpLoggingInterceptorContext logContext)
    {
        logContext.AddParameter("RequestHeaders", RedactHeaders(logContext.HttpContext.Request.Headers));
    }

    private void EnrichRequest(HttpLoggingInterceptorContext logContext)
    {
        logContext.AddParameter("Application", _applicationSettings.Name);
        logContext.AddParameter("Domain", _applicationSettings.Domain);
    }

    private void RedactResponseHeaders(HttpLoggingInterceptorContext logContext)
    {
        logContext.AddParameter("ResponseHeaders", RedactHeaders(logContext.HttpContext.Response.Headers));
    }

    private void RedactResponseBody(HttpLoggingInterceptorContext logContext)
    {
        //logContext.AddParameter("ResponseBody", body);
    }

    private HeaderDictionary RedactHeaders(IHeaderDictionary headers)
    {
        var redactedHeaders = new HeaderDictionary();
        foreach (var item in headers)
        {
            var isNotRedactedHeader = httpLoggingOptions.RequestHeaders.Contains(item.Key);
            var redactedHeader = isNotRedactedHeader ? item.Value : REDACTED_VALUE;
            redactedHeaders.Add(item.Key, redactedHeader);
        }
        return redactedHeaders;
    }
}
