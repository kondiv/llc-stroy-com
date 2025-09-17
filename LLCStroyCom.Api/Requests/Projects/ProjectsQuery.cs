using LLCStroyCom.Domain.Models.Filters.Project;

namespace LLCStroyCom.Api.Requests.Projects;

public sealed class ProjectsQuery
{
    public ProjectFilter ProjectFilter { get; set; } = new ProjectFilter();
    public string? PageToken { get; set; }
    public int MaxPageSize { get; set; } = 10;
}