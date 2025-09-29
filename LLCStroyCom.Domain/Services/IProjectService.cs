using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Models.Filters.Project;
using LLCStroyCom.Domain.Requests;

namespace LLCStroyCom.Domain.Services;

public interface IProjectService
{
    Task<ProjectDto> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PaginatedProjectListResponse> ListAsync(string? plainPageToken, ProjectFilter filter, int maxPageSize,
        CancellationToken cancellationToken = default);

    Task<ProjectDto> CreateAsync(Guid companyId, ProjectCreateRequest request, CancellationToken cancellationToken = default);
}