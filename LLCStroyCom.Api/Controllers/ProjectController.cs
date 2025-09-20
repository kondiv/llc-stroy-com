using LLCStroyCom.Api.Requests.Projects;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Exceptions;
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
        try
        {
            var result =
                await _projectService.ListAsync(query.PageToken, query.ProjectFilter, query.MaxPageSize, cancellationToken);
            return Ok(result);
        }
        catch (Exception e) when(e is PageTokenEncodingException or PageTokenDecodingException)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetAsync([FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _projectService.GetAsync(id, cancellationToken);

            return Ok(project);
        }
        catch (CouldNotFindProject e)
        {
            return NotFound(e.Message);
        }
    }
}