namespace LLCStroyCom.Domain.Dto;

public class PaginatedProjectListResponse
{
    public ICollection<ProjectDto> Projects { get; set; } = [];
    public string? PageToken { get; set; }
}