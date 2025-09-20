using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Domain.Response;

public class ProjectsListResponse
{
    public string? PageToken { get; set; }
    public IEnumerable<Project> Projects { get; set; } = new List<Project>();
}