namespace LLCStroyCom.Domain.Exceptions;

public class PageTokenDecodingException : Exception
{
    private PageTokenDecodingException(string message)
        : base(message)
    {
        
    }

    public static PageTokenDecodingException ForToken(string token)
    {
        return new PageTokenDecodingException("Error while decoding token: " + token);
    }
}