namespace LLCStroyCom.Domain.Models.PageTokens;

public abstract class PageToken : IPageToken
{
    public bool HasNextPage { get; set; }
    public abstract string OrderBy { get; set; }
    public bool Descending { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}