using System.Collections;
using Microsoft.AspNetCore.Http;

namespace LLCStroyCom.Tests.Controllers;

public class RequestCookieCollection : IRequestCookieCollection
{
    private readonly Dictionary<string, string> _cookies;

    public RequestCookieCollection(Dictionary<string, string> cookies)
    {
        _cookies = cookies;
    }

    public string? this[string key] => _cookies.TryGetValue(key, out var value) ? value : null;

    public int Count => _cookies.Count;
    public ICollection<string> Keys => _cookies.Keys;

    public bool ContainsKey(string key) => _cookies.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _cookies.GetEnumerator();

    public bool TryGetValue(string key, out string? value)
    {
        var result = _cookies.TryGetValue(key, out var val);
        value = val;
        return result;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public record Tokens(string AccessToken, string RefreshToken);