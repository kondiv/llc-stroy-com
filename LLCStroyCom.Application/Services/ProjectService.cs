using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Models.Filters.Project;
using LLCStroyCom.Domain.Models.PageTokens;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Services;
using LLCStroyCom.Domain.Specifications.Projects;

namespace LLCStroyCom.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IPageTokenService _pageTokenService;
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IPageTokenService pageTokenService, IProjectRepository projectRepository)
    {
        _pageTokenService = pageTokenService;
        _projectRepository = projectRepository;
    }
    
    public async Task<PaginatedProjectListResponse> ListAsync(string? plainPageToken, ProjectFilter filter, int maxPageSize, 
        CancellationToken cancellationToken = default)
    {
        ProjectPageToken? token = null;
        
        if (!string.IsNullOrEmpty(plainPageToken))
        {
            token = _pageTokenService.Decode<ProjectPageToken>(plainPageToken);
        }
        
        var specification = new ProjectSpecification(filter, token, maxPageSize);

        var projects = await _projectRepository.ListAsync(specification, cancellationToken);
        
        
        var projectsList = projects.ToList();
        if (projectsList.Count == 0)
        {
            return new PaginatedProjectListResponse()
            {
                Projects = new List<ProjectDto>(),
                PageToken = null
            };
        }
        
        var hasNextPage = false;
        if (projectsList.Count > maxPageSize)
        {
            hasNextPage = true;
            projectsList.Remove(projectsList.Last());
        }

        if (!hasNextPage)
        {
            return new PaginatedProjectListResponse()
            {
                Projects = projectsList.Select(p => new ProjectDto(p.Name, p.City, p.CompanyId, p.Status, p.CreatedAt))
                    .ToList(),
                PageToken = null
            };
        }

        var currentToken = new ProjectPageToken()
        {
            CreatedAt = DateTimeOffset.UtcNow,
            Descending = token?.Descending ?? filter.Descending,
            HasNextPage = hasNextPage,
            OrderBy = token?.OrderBy ?? filter.OrderBy,
            ProjectId = projectsList.Last().Id,
            ProjectName = projectsList.Last().Name,
            ProjectCreatedAt = projectsList.Last().CreatedAt
        };

        var encodedToken = _pageTokenService.Encode(currentToken);

        return new PaginatedProjectListResponse()
        {
            Projects = projectsList.Select(p => new ProjectDto(p.Name, p.City, p.CompanyId, p.Status, p.CreatedAt))
                .ToList(),
            PageToken = encodedToken
        };
    }
}