using LLCStroyCom.Api.Requests.Projects;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.ResultPattern.Errors;
using LLCStroyCom.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LLCStroyCom.Api.Controllers;

[ApiController]
[Route("api/companies/{companyId:guid}/projects")]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetProjectAsync([FromRoute]Guid companyId, [FromRoute]Guid projectId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting project: {projectId}", projectId);
        
        var result = await _projectService.GetAsync(companyId, projectId, cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogWarning("Cannot get project: {projectId}\nReason: [{errorCode}]{errorMessage}", projectId,
                result.Error.ErrorCode, result.Error.Message);
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }
    
    [Authorize]    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedProjectListResponse>> ListAsync([FromRoute]Guid companyId, [FromQuery] ProjectsQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting projects: {query}", query);
            
            var result =
                await _projectService.ListAsync(companyId, query.PageToken, query.ProjectFilter, query.MaxPageSize, cancellationToken);
            return Ok(result);
        }
        catch (Exception e) when(e is PageTokenEncodingException or PageTokenDecodingException)
        {
            _logger.LogError("Error with encoding or decoding page token");
            return BadRequest(e.Message);
        }
    }
    
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProjectDto>> CreateProjectAsync([FromRoute] Guid companyId,
        ProjectCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to create project {projectName} in company {companyId}", request.Name, companyId);
        var result = await _projectService.CreateAsync(companyId, request, cancellationToken);

        if (result.Succeeded)
            return CreatedAtRoute("GetProject", new { companyId, projectId = result.Value.Id }, result.Value);
        
        _logger.LogError("Cannot create project in company {companyId}\nReason: [{errorCode}]{errorMessage}",
            companyId, result.Error.ErrorCode, result.Error.Message);
        return result.Error.ErrorCode switch
        {
            ErrorCode.NotFound => NotFound(result.Error),
            ErrorCode.AlreadyExists => Conflict(result.Error),
            _ => BadRequest(),
        };
    }

    [Authorize]
    [HttpPatch("{projectId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> UpdateProjectAsync([FromRoute]Guid companyId, [FromRoute] Guid projectId, JsonPatchDocument<ProjectPatchDto> patchDocument,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to update project {id}", projectId);
        var result = await _projectService.UpdateAsync(companyId, projectId, patchDocument, cancellationToken);

        if (result.Succeeded)
        {
            return NoContent();
        }

        var error = result.Error;
        _logger.LogError("Cannot update project. Reason [{errorCode}]{errorMessage}", error.ErrorCode, error.Message);

        return error.ErrorCode switch
        {
            ErrorCode.NotFound => NotFound(error.Message),
            ErrorCode.DbUpdateConcurrency or ErrorCode.AlreadyExists => Conflict(error.Message),
            _ => BadRequest(error.Message)
        };
    }

    [Authorize]
    [HttpDelete("{projectId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteProjectAsync([FromRoute] Guid companyId, [FromRoute] Guid projectId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to delete project {id}", projectId);
        
        var result = await _projectService.DeleteAsync(companyId, projectId, cancellationToken);

        return result.Succeeded ? NoContent() : NotFound(result.Error.Message);
    }
}