using LLCStroyCom.Domain.Models.PageTokens;

namespace LLCStroyCom.Domain.Services;

public interface IPageTokenService
{
    string Encode<TToken>(TToken token) where TToken : IPageToken;
    TToken Decode<TToken>(string encodedToken) where TToken : IPageToken;
}