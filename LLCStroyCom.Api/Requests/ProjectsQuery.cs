using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Specifications.Projects;

namespace LLCStroyCom.Api.Requests;

public sealed class ProjectsQuery
{
    public string? PageToken { get; set; }
    public string? NameFilter { get; set; }
    public string? CityFilter { get; set; }
    public Status? StatusFilter { get; set; }
    public Guid? CompanyFilter { get; set; }
    public int MaxPageSize { get; set; } = 30;
    public OrderBy OrderBy { get; set; } = OrderBy.CreatedAtAsc;
}