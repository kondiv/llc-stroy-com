using AutoMapper;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Models.Filters.Project;
using LLCStroyCom.Domain.Models.PageTokens;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.ResultPattern;
using LLCStroyCom.Domain.ResultPattern.Errors;
using LLCStroyCom.Domain.Services;
using LLCStroyCom.Domain.Specifications.Projects;
using Microsoft.AspNetCore.JsonPatch;

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

    public async Task<Result<ProjectDto>> GetAsync(Guid companyId, Guid projectId, CancellationToken cancellationToken = default)
    {
        var projectResult = await _projectRepository.GetAsync(projectId, cancellationToken);

        if (projectResult.IsFailure)
        {
            return Result<ProjectDto>.Failure(projectResult.Error, projectResult.InnerException);
        }

        if (projectResult.Value.CompanyId != companyId)
        {
            return Result<ProjectDto>.Failure(new NotFoundError($"Project {projectId} does not exist in company {companyId}"));
        }
        
        var projectDto = _mapper.Map<ProjectDto>(projectResult.Value);

        return Result<ProjectDto>.Success(projectDto);
    }

    public async Task<PaginatedProjectListResponse> ListAsync(Guid companyId, string? plainPageToken, ProjectFilter filter, int maxPageSize, 
        CancellationToken cancellationToken = default)
    {
        ProjectPageToken? token = null;
        
        if (!string.IsNullOrEmpty(plainPageToken))
        {
            token = _pageTokenService.Decode<ProjectPageToken>(plainPageToken);
        }
        
        var specification = new ProjectSpecification(companyId, filter, token, maxPageSize);

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

    public async Task<Result<ProjectDto>> CreateAsync(Guid companyId, ProjectCreateRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _ = await _companyRepository.GetAsync(companyId, cancellationToken);
        }
        catch (CouldNotFindCompany e)
        {
            return Result<ProjectDto>.Failure(new NotFoundError("Company was not found"), e);
        }

        var project = new Project()
        {
            Name = request.Name,
            City = request.City,
            CompanyId = companyId,
        };
        
        var result = await _projectRepository.CreateAsync(project, cancellationToken);

        if (result.IsFailure)
        {
            return Result<ProjectDto>.Failure(result.Error, result.InnerException);
        }
        
        var projectDto = _mapper.Map<ProjectDto>(result.Value);
        return Result<ProjectDto>.Success(projectDto);
    }

    public async Task<Result> UpdateAsync(Guid companyId, Guid projectId, JsonPatchDocument<ProjectPatchDto> patchDocument,
        CancellationToken cancellationToken = default)
    {
        var getProject = await _projectRepository.GetAsync(projectId, cancellationToken);

        if (getProject.IsFailure)
        {
            return Result.Failure(getProject.Error);
        }

        var existingProject = getProject.Value;

        if (!BelongsToCompany(existingProject, companyId))
        {
            return Result.Failure(new NotFoundError($"Cannot find project {projectId} in company {companyId}"));
        }
        
        var projectDto = _mapper.Map<ProjectPatchDto>(existingProject);
        
        patchDocument.ApplyTo(projectDto);

        var project = _mapper.Map(projectDto, existingProject);

        return await _projectRepository.UpdateAsync(project, cancellationToken);
    }

    public async Task<Result> DeleteAsync(Guid companyId, Guid projectId, CancellationToken cancellationToken = default)
    {
        if (!await BelongsToCompanyAsync(projectId, companyId, cancellationToken))
        {
            return Result.Failure(new NotFoundError($"Cannot find project {projectId} in company {companyId}"));
        }
        
        return await _projectRepository.DeleteAsync(projectId, cancellationToken);
    }

    private async Task<bool> BelongsToCompanyAsync(Guid projectId, Guid companyId, CancellationToken cancellationToken = default)
    {
        var getProjectResult = await _projectRepository.GetAsync(projectId, cancellationToken);

        if (getProjectResult.IsFailure)
        {
            return false;
        }

        return getProjectResult.Value.CompanyId == companyId;
    }
    private bool BelongsToCompany(Project project, Guid companyId)
    {
        return project.CompanyId == companyId;
    }
}