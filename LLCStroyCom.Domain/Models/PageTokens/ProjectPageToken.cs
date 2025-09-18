using LLCStroyCom.Domain.Enums;

namespace LLCStroyCom.Domain.Models.PageTokens;

public class ProjectPageToken : PageToken
{
    public Guid ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public DateTimeOffset? ProjectCreatedAt { get; set; }
    public override string OrderBy { get; set; } = "name";
}