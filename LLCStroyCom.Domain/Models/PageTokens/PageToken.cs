namespace LLCStroyCom.Domain.Models.PageTokens;

public class PageToken : IPageToken
{
    public bool HasNextPage { get; set; }
    public string OrderBy { get; set; } = null!;
    public bool Descending { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}