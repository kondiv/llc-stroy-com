using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Specifications;

namespace LLCStroyCom.Domain.Models.PageTokens;

public interface IPageToken
{
    public bool HasNextPage { get; }
    public string OrderBy { get; set; }
    public bool Descending { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}