namespace Scaffolding.AspNetCore.RequestKey;

public class RequestKey
{
    public string Value { get; set; } = string.Empty;

    public RequestKey()
    {
    }

    public RequestKey(string value)
    {
        Value = value;
    }

    public static implicit operator string(RequestKey requestKey)
    {
        return requestKey.Value;
    }

    public static implicit operator RequestKey(string requestKey)
    {
        return new(requestKey);
    }
}
