using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.ResultPattern.Errors;
using LLCStroyCom.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LLCStroyCom.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/defects")]
public class ProjectDefectsController : ControllerBase
{
    private readonly IDefectService _defectService;
    private readonly ILogger<ProjectDefectsController> _logger;
    
    public ProjectDefectsController(IDefectService defectService, ILogger<ProjectDefectsController> logger)
    {
        _defectService = defectService;
        _logger = logger;
    }

    [Authorize]
    [HttpGet("{defectId:guid}", Name = "Get")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DefectDto>> GetAsync([FromRoute] Guid projectId, [FromRoute] Guid defectId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Get defect {defectId}", defectId);
        var result = await _defectService.GetAsync(projectId, defectId, cancellationToken);

        if (result.Succeeded)
        {
            return Ok(result.Value);
        }

        _logger.LogError("Cannot get {defectId}. Reason:[{errorCode}]{errorMessage}", defectId, result.Error.ErrorCode, result.Error.Message);
        return result.Error.ErrorCode switch
        {
            ErrorCode.NotFound => NotFound(result.Error.Message),
            _ => BadRequest(result.Error.Message)
        };
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DefectDto>> CreateAsync([FromRoute]Guid projectId, [FromBody]DefectCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Create defect");
        if (!ModelState.IsValid)
        {
            _logger.LogError("Cannot create defect. Invalid request state");
            return BadRequest(ModelState.ErrorCount);
        }

        var result = await _defectService.CreateAsync(projectId, request, cancellationToken);

        if (result.Succeeded)
        {
            return CreatedAtRoute("Get", new {projectId, defectId = result.Value.Id}, result.Value);
        }

        _logger.LogError("Cannot create defect. Reason:[{errorCode}]{errorMessage}", result.Error.ErrorCode, result.Error.Message);
        return result.Error.ErrorCode switch
        {
            ErrorCode.AlreadyExists => Conflict(result.Error.Message),
            _ => BadRequest(result.Error.Message)
        };
    }

    [Authorize]
    [HttpPatch("{defectId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> UpdateAsync([FromRoute] Guid projectId, [FromRoute] Guid defectId,
        JsonPatchDocument<DefectPatchDto> patchDocument,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Update defect {defectId} from project {projectId}", defectId, projectId);
        var result = await _defectService.UpdateAsync(projectId, defectId, patchDocument, cancellationToken);

        if (result.Succeeded)
        {
            return NoContent();
        }

        _logger.LogError("Cannot update defect. Reason:[{errorCode}]{errorMessage}]", result.Error.ErrorCode, result.Error.Message);
        return result.Error.ErrorCode switch
        {
            ErrorCode.DbUpdateConcurrency => Conflict(result.Error.Message),
            _ => BadRequest(result.Error.Message)
        };
    }

    [Authorize]
    [HttpDelete("{defectId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync([FromRoute]Guid projectId, [FromRoute]Guid defectId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting defect {defectId} from project {projectId}", defectId, projectId);
        var result = await _defectService.DeleteAsync(projectId, defectId, cancellationToken);

        if (result.Succeeded)
        {
            return NoContent();
        }

        _logger.LogError("Cannot delete defect. Reason:[{errorCode}]{errorMessage}]", result.Error.ErrorCode, result.Error.Message);
        return result.Error.ErrorCode switch
        {
            ErrorCode.NotFound => NotFound(result.Error.Message),
            _ => BadRequest(result.Error.Message)
        };
    }
}