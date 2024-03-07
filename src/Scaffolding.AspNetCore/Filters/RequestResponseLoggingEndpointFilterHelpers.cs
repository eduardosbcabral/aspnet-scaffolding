using Microsoft.AspNetCore.Http;

internal static class RequestResponseLoggingEndpointFilterHelpers
{
    internal static Dictionary<string, string> GetHeaders(IHeaderDictionary headers, string[] headersToLog)
    {
        var dic = new Dictionary<string, string>();
        foreach (var item in headers.Where(x => headersToLog.Contains(x.Key)))
        {
            var value = item.Value.ToString();
            dic[item.Key] = value;
        }

        return dic;
    }
}