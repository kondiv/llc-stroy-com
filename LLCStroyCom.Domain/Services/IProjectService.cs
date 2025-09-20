using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Models.Filters.Project;

namespace LLCStroyCom.Domain.Services;

public interface IProjectService
{
    Task<PaginatedProjectListResponse> ListAsync(string? plainPageToken, ProjectFilter filter, int maxPageSize,
        CancellationToken cancellationToken = default);

    Task<ProjectDto> GetAsync(Guid id, CancellationToken cancellationToken = default);
}