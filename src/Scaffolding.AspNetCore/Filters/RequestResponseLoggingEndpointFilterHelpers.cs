using Microsoft.AspNetCore.Http;

internal static class RequestResponseLoggingEndpointFilterHelpers
{

    private static IDictionary<string, string> GetHeaders(IHeaderDictionary headers)
    {
        var dic = new Dictionary<string, string>();
        foreach (var item in headers)
        {
            var value = item.Value.ToString();
            dic[item.Key] = value;
        }

        return dic;
    }
}