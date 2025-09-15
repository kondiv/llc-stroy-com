using LLCStroyCom.Domain.Models.PageTokens;

namespace LLCStroyCom.Domain.Exceptions;

public class PageTokenEncodingException : Exception
{
    private PageTokenEncodingException(string message)
        : base(message)
    {
        
    }

    public static PageTokenEncodingException ForToken(string tokenType)
    {
        return new PageTokenEncodingException($"Error while encoding page token: {tokenType}");
    }
}