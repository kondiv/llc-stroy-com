using LLCStroyCom.Api.Requests.Projects;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace LLCStroyCom.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }
    
    [HttpGet("list")]
    public async Task<ActionResult<PaginatedProjectListResponse>> ListAsync([FromQuery] ProjectsQuery query,
        CancellationToken cancellationToken = default)
    {
        var result =
            await _projectService.ListAsync(query.PageToken, query.ProjectFilter, query.MaxPageSize, cancellationToken);

        return Ok(result);
    }
}