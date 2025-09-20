using LLCStroyCom.Api.Requests.Projects;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LLCStroyCom.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }
    
    [Authorize]    
    [HttpGet("list")]
    public async Task<ActionResult<PaginatedProjectListResponse>> ListAsync([FromQuery] ProjectsQuery query,
        CancellationToken cancellationToken = default)
    {
        var result =
            await _projectService.ListAsync(query.PageToken, query.ProjectFilter, query.MaxPageSize, cancellationToken);

        return Ok(result);
    }
}