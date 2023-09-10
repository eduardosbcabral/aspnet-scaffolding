using Scaffolding.Extensions.Json;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace Scaffolding.Utilities.Extractors;

public static class HttpContextExtractor
{
    /// <summary>
    /// Get status code
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static int GetStatusCode(this HttpContext context, Exception exception)
    {
        if (exception != null)
        {
            return 500;
        }

        var statusCode = context?.Response?.StatusCode ?? 0;
        return statusCode;
    }

    /// <summary>
    /// Get status code family, like 1XX 2XX 3XX 4XX 5XX
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static string GetStatusCodeFamily(this HttpContext context, Exception exception)
    {
        var statusCode = context.GetStatusCode(exception);
        return statusCode.ToString()[0] + "XX";
    }

    /// <summary>
    /// Get query string
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static IDictionary<string, string> GetQueryString(this HttpContext context, string[] blacklist)
    {
        if (context?.Request?.Query == null)
        {
            return null;
        }

        var dic = new Dictionary<string, string>();
        foreach (var item in context.Request.Query)
        {
            var key = item.Key;
            var value = item.Value.ToString();
            dic[item.Key] = MaskField(key, value, blacklist);
        }

        return dic;
    }

    public static string GetRawQueryString(this HttpContext context, string[] blacklist)
    {
        if (context?.Request?.Query == null)
        {
            return string.Empty;
        }

        var queryString = HttpUtility.ParseQueryString(context.Request.QueryString.ToString());
        foreach (var qs in queryString.AllKeys)
        {
            var key = qs;
            var value = queryString[qs];
            queryString[qs] = MaskField(key, value, blacklist);
        }

        return queryString.HasKeys() ? $"?{queryString}" : "";
    }

    /// <summary>
    /// Get all request headers
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static IDictionary<string, string> GetRequestHeaders(this HttpContext context, string[] blacklist)
    {
        if (context?.Request?.Headers == null)
        {
            return null;
        }

        var dic = new Dictionary<string, string>();
        foreach (var item in context.Request.Headers)
        {
            var key = item.Key;
            var value = item.Value.ToString();
            dic[item.Key] = MaskField(key, value, blacklist);
        }

        return dic;
    }

    /// <summary>
    /// Get all response headers
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static IDictionary<string, string> GetResponseHeaders(this HttpContext context, string[] blacklist)
    {
        if (context?.Response?.Headers == null)
        {
            return null;
        }

        var dic = new Dictionary<string, string>();
        foreach (var item in context.Response.Headers)
        {
            var key = item.Key;
            var value = item.Value.ToString();
            dic[item.Key] = MaskField(key, value, blacklist);
        }

        return dic;
    }

    /// <summary>
    /// Get total execution time from X-Internal-Time header
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static object GetExecutionTime(this HttpContext context, string timeElapsedProperty)
    {
        long elapsedDefault = -1;
        object elapsedParsed = "-1";

        context?.Items?.TryGetValue(timeElapsedProperty, out elapsedParsed);
        if (Int64.TryParse(elapsedParsed.ToString(), out long elapsedLong) == true)
        {
            return elapsedLong;
        }

        return elapsedDefault;
    }

    /// <summary>
    /// Get request key from RequestKey Header
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetRequestKey(this HttpContext context, string requestKeyProperty)
    {
        if (string.IsNullOrWhiteSpace(requestKeyProperty))
        {
            return null;
        }

        if (context?.Items?.ContainsKey(requestKeyProperty) == true)
        {
            return context.Items[requestKeyProperty].ToString();
        }

        return null;
    }

    /// <summary>
    /// Get ip (X-Forwarded-For or original)
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetIp(this HttpContext context)
    {
        var defaultIp = "??";

        if (context?.Request?.Headers == null)
        {
            return defaultIp;
        }

        if (context.Request.Headers.Any(r => r.Key == "X-Forwarded-For") == true)
        {
            return context.Request.Headers["X-Forwarded-For"].First();
        }

        return context.Connection?.RemoteIpAddress?.ToString() ?? defaultIp;
    }

    /// <summary>
    /// Get user
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetUser(this HttpContext context)
    {
        return context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Get request body
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static async Task<object> GetRequestBody(this HttpContext context, string[] blacklist)
    {
        var request = context?.Request;

        if (request?.Body == null)
        {
            return null;
        }

        var body = "";
        request.EnableBuffering();
        using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
        {
            body = await reader.ReadToEndAsync();
        }
        request.Body.Position = 0;

        var contentType = (context.Request.Headers.ContainsKey("Content-Type") == true)
            ? string.Join(";", context.Request.Headers["Content-Type"])
            : string.Empty;

        var isJson = contentType.Contains("json") == true;

        if (isJson)
        {
            return GetContentAsObjectByContentTypeJson(body, true, blacklist);
        }
        else
        {
            return new Dictionary<string, string> { { "raw_body", body } };
        }
    }

    /// <summary>
    /// Get host
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static object GetHost(this HttpContext context)
    {
        return context?.Request?.Host.ToString().Split(':').FirstOrDefault();
    }

    /// <summary>
    /// Get port
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static object GetPort(this HttpContext context)
    {
        var parts = context?.Request?.Host.ToString().Split(':');

        if (parts.Count() > 1)
        {
            return parts.LastOrDefault();
        }

        if (context?.Request?.Protocol == "http")
        {
            return 80;
        }

        if (context?.Request?.Protocol == "https")
        {
            return 443;
        }

        return 0;
    }

    /// <summary>
    /// Get port
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static object GetFullUrl(this HttpContext context, string[] queryBlacklist, string[] httpContextBlackList)
    {
        var absoluteUri = string.Concat(
                   context?.Request?.Scheme,
                   "://",
                   context?.Request?.Host.ToUriComponent(),
                   context?.GetPathBase(httpContextBlackList),
                   context?.GetPath(httpContextBlackList),
                   context.GetRawQueryString(queryBlacklist));

        return absoluteUri;
    }

    /// <summary>
    /// Get Path
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static object GetPath(this HttpContext context, string[] blacklist)
    {

        if (blacklist?.Any() == true && blacklist.Contains("Path"))
        {
            return @"/******";
        }

        return context.Request.Path.ToUriComponent();

    }

    /// <summary>
    /// Get PathBase
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static object GetPathBase(this HttpContext context, string[] blacklist)
    {

        if (blacklist?.Any() == true && blacklist.Contains("PathBase"))
        {
            return @"/******";
        }

        return context.Request.PathBase.ToUriComponent();

    }

    /// <summary>
    /// Get response content
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static async Task<object> GetResponseContent(this HttpContext context, string[] blacklist)
    {
        if (context?.Response?.Body?.CanRead == false)
        {
            return null;
        }

        MemoryStream stream = new();

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        context.Response.Body.CopyTo(stream);

        stream.Seek(0, SeekOrigin.Begin);
        var body = string.Empty;
        using (StreamReader reader = new(stream))
        {
            body = await reader.ReadToEndAsync();
        }

        if (string.IsNullOrWhiteSpace(body) == false &&
            context.Response.ContentType.Contains("json") == true)
        {
            return GetContentAsObjectByContentTypeJson(body, true, blacklist);
        }
        else
        {
            return new Dictionary<string, string> { { "raw_content", body } };
        }
    }

    /// <summary>
    /// Get content length
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static long GetResponseLength(this HttpContext context)
    {
        return context?.Response?.Body?.Length ?? 0;
    }

    /// <summary>
    /// Get content as object by content type
    /// </summary>
    /// <param name="content"></param>
    /// <param name="contentType"></param>
    internal static object GetContentAsObjectByContentTypeJson(string content, bool maskJson, string[] backlist)
    {
        try
        {
            if (maskJson == true && backlist?.Any() == true)
            {
                content = content.MaskFields(backlist, "******");
            }

            return content.DeserializeAsObject();
        }
        catch (Exception) { }

        return content;
    }

    internal static string MaskField(string key, string value, string[] blacklist)
    {
        if (blacklist?.Any() == true && blacklist.Contains(key))
        {
            return "******";
        }

        return value;
    }
}
