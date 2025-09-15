namespace LLCStroyCom.Domain.Models.PageTokens;

public class PageTokenWrapper<TToken> where TToken : IPageToken
{
    public string Type { get; set; } = null!;
    public TToken Token { get; set; }
}