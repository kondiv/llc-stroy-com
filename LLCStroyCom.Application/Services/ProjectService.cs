using AutoMapper;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Models.Filters.Project;
using LLCStroyCom.Domain.Models.PageTokens;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.Services;
using LLCStroyCom.Domain.Specifications.Projects;

namespace LLCStroyCom.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IPageTokenService _pageTokenService;
    private readonly IProjectRepository _projectRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;

    public ProjectService(
        IPageTokenService pageTokenService,
        IProjectRepository projectRepository,
        ICompanyRepository companyRepository,
        IMapper mapper)
    {
        _pageTokenService = pageTokenService;
        _projectRepository = projectRepository;
        _companyRepository = companyRepository;
        _mapper = mapper;
    }

    public async Task<ProjectDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetAsync(id, cancellationToken);
        
        return _mapper.Map<ProjectDto>(project);
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
                Projects = projectsList.Select(p => _mapper.Map<ProjectDto>(p))
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
            Projects = projectsList.Select(p => _mapper.Map<ProjectDto>(p))
                .ToList(),
            PageToken = encodedToken
        };
    }

    public async Task<ProjectDto> CreateAsync(Guid companyId, ProjectCreateRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}