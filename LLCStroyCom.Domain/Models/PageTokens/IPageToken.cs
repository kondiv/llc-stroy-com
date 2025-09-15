using LLCStroyCom.Domain.Enums;

namespace LLCStroyCom.Domain.Models.PageTokens;

public interface IPageToken
{
    public OrderBy OrderBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}