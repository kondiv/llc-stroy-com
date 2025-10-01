using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Models.Filters.Project;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.ResultPattern;
using Microsoft.AspNetCore.JsonPatch;

namespace LLCStroyCom.Domain.Services;

public interface IProjectService
{
    Task<Result<ProjectDto>> GetAsync(Guid companyId, Guid projectId, CancellationToken cancellationToken = default);

    Task<PaginatedProjectListResponse> ListAsync(string? plainPageToken, ProjectFilter filter, int maxPageSize,
        CancellationToken cancellationToken = default);

    Task<Result<ProjectDto>> CreateAsync(Guid companyId, ProjectCreateRequest request, 
        CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(Guid id, JsonPatchDocument<ProjectPatchDto> patchDocument,
        CancellationToken cancellationToken = default);
}