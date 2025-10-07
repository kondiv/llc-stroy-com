namespace LLCStroyCom.Domain.Models;

public class PaginationResult<T>
{
    public IEnumerable<T> Items { get; }
    public int Page { get; }
    public int MaxPageSize { get; }
    public int PageCount { get; }
    public int TotalCount { get; }
    public bool HasNextPage => Page < PageCount;
    public bool HasPreviousPage => Page > 1;

    public PaginationResult(
        IEnumerable<T> items,
        int page,
        int maxPageSize,
        int pageCount,
        int totalCount)
    {
        Items = items;
        Page = page;
        MaxPageSize = maxPageSize;
        PageCount = pageCount;
        TotalCount = totalCount;
    }
}