using LLCStroyCom.Domain.Enums;

namespace LLCStroyCom.Domain.Models.PageTokens;

public class PageToken : IPageToken
{
    public OrderBy OrderBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}