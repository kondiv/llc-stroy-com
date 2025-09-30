using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LLCStroyCom.Api.Controllers;

[ApiController]
[Route("api/companies/{companyId}/projects")]
public class CompanyProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<CompanyProjectsController> _logger;

    public CompanyProjectsController(IProjectService projectService, ILogger<CompanyProjectsController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }
    
    [Authorize]
    [HttpGet("{projectId:guid}", Name = "GetProject")]
    public async Task<ActionResult<ProjectDto>> GetProjectAsync([FromRoute]Guid companyId, [FromRoute]Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var result = await _projectService.GetAsync(companyId, projectId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }
    
    [Authorize]
    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProjectDto>> CreateProjectAsync([FromRoute] Guid companyId,
        ProjectCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _projectService.CreateAsync(companyId, request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Errors);
        }
        
        return CreatedAtRoute("GetProject",new {companyId = companyId, projectId = result.Value.Id}, result.Value);
    }
}