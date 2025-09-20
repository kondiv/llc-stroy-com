using System.Text;
using System.Text.Json;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Models.PageTokens;
using LLCStroyCom.Domain.Services;

namespace LLCStroyCom.Application.Services;

public sealed class PageTokenService : IPageTokenService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    
    public PageTokenService()
    {
        _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }
    
    public string Encode<TToken>(TToken token) where TToken : IPageToken
    {
        ArgumentNullException.ThrowIfNull(token, nameof(token));

        try
        {
            var pageTokenWrapper = new PageTokenWrapper<TToken>()
            {
                Type = typeof(TToken).Name,
                Token = token
            };
            
            var json = JsonSerializer.Serialize(pageTokenWrapper, _jsonSerializerOptions);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(jsonBytes);
        }
        catch (Exception)
        {
            throw PageTokenEncodingException.ForToken(typeof(TToken).Name);
        }
    }

    public TToken Decode<TToken>(string encodedToken) where TToken : IPageToken
    {
        ArgumentNullException.ThrowIfNull(encodedToken, nameof(encodedToken));
        ArgumentException.ThrowIfNullOrEmpty(encodedToken, nameof(encodedToken));
        ArgumentException.ThrowIfNullOrWhiteSpace(encodedToken, nameof(encodedToken));

        try
        {
            var jsonBytes = Convert.FromBase64String(encodedToken);
            var json = Encoding.UTF8.GetString(jsonBytes);
            var wrapper = JsonSerializer.Deserialize<PageTokenWrapper<TToken>>(json, _jsonSerializerOptions);

            if (wrapper is null || wrapper.Type != typeof(TToken).Name)
            {
                throw PageTokenDecodingException.ForToken(typeof(TToken).Name);
            }

            return wrapper.Token;
        }
        catch (Exception)
        {
            throw PageTokenDecodingException.ForToken(typeof(TToken).Name);
        }
    }
}