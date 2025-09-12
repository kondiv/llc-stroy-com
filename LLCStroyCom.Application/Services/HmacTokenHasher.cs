using System.Security.Cryptography;
using System.Text;
using LLCStroyCom.Domain.Configs;
using LLCStroyCom.Domain.Services;
using Microsoft.Extensions.Options;

namespace LLCStroyCom.Application.Services;

public class HmacTokenHasher : ITokenHasher
{
    private readonly byte[] _keyBytes;

    public HmacTokenHasher(string secret)
    {
        _keyBytes = Encoding.UTF8.GetBytes(secret);
    }
    
    public string HashToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentException.ThrowIfNullOrEmpty(token);
        
        using var hmac = new HMACSHA256(_keyBytes);
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = hmac.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}