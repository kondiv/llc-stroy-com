using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Api.Controllers;

[ApiController]
[Route("api/companies")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<CompanyController> _logger;

    public CompanyController(ICompanyService companyService, ILogger<CompanyController> logger)
    {
        _companyService = companyService;
        _logger = logger;
    }

    [Authorize]
    [HttpGet("{id:guid}", Name = "GetCompany")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CompanyDto>> GetAsync([FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Requesting company: {id}", id);
            var company = await _companyService.GetAsync(id, cancellationToken);
            return Ok(company);
        }
        catch (CouldNotFindCompany e)
        {
            _logger.LogWarning(e, e.Message);
            return NotFound(e.Message);
        }
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CompanyDto>> CreateAsync([FromBody] CompanyCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Request for creating company");
            var result = await _companyService.CreateAsync(request, cancellationToken);
            return CreatedAtRoute("GetCompany", new { id = result.Id }, result);
        }
        catch (DbUpdateException e)
        {
            _logger.LogWarning(e, e.Message);
            return Conflict(e.InnerException?.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Request for deleting company: {id}", id);
            await _companyService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (CouldNotFindCompany e)
        {
            _logger.LogWarning(e, e.Message);
            return NotFound(e.Message);
        }
    }

    [Authorize]
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> PatchAsync([FromRoute] Guid id, JsonPatchDocument<CompanyPatchDto> patch,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _companyService.UpdateAsync(id, patch, cancellationToken);

            return NoContent();
        }
        catch (CouldNotFindCompany e)
        {
            _logger.LogWarning(e, e.Message);
            return NotFound();
        }
    }

    [Authorize]
    [HttpGet("{id:guid}/employees/{employeeId:guid}", Name = "GetEmployee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EmployeeDto>> GetEmployeeAsync([FromRoute] Guid id, [FromRoute] Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Request for employee: {id}", id);
            var employee = await _companyService.GetEmployeeAsync(id, employeeId, cancellationToken);
            return Ok(employee);
        }
        catch (CouldNotFind e)
        {
            _logger.LogWarning(e, e.Message);
            return NotFound(e.Message);
        }
    }
    

    [Authorize]
    [HttpPost("{id:guid}/employees/{employeeId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Guid>> AddEmployeeAsync([FromRoute] Guid id, [FromRoute] Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Request for adding employee: {employeeId} to company: {id}", employeeId, id);
            var result = await _companyService.AddEmployeeAsync(id, employeeId, cancellationToken);
            return CreatedAtRoute("GetEmployee", result, result);
        }
        catch (AlreadyWorks e)
        {
            _logger.LogWarning(e, e.Message);
            return Conflict(e.Message);
        }
        catch (CouldNotFind e)
        {
            _logger.LogWarning(e, e.Message);
            return NotFound(e.Message);
        }
        catch (DbUpdateConcurrencyException e)
        {
            _logger.LogWarning(e, e.Message);
            return Conflict(e.InnerException?.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}/employees/{employeeId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveEmployeeAsync([FromRoute] Guid id, [FromRoute] Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Request for removing employee: {employeeId} from company: {id}", employeeId, id);
            await _companyService.RemoveEmployeeAsync(id, employeeId, cancellationToken);
            return NoContent();
        }
        catch (CouldNotFind e)
        {
            _logger.LogWarning(e, e.Message);
            return NotFound();
        }
        catch (DbUpdateConcurrencyException e)
        {
            _logger.LogWarning(e, e.Message);
            return Conflict(e.InnerException?.Message);
        }
    }

    [Authorize]
    [HttpPatch("{id:guid}/employees/{employeeId:guid}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    public ActionResult UpdateEmployeeAsync([FromRoute] Guid id, [FromRoute] Guid employeeId, JsonPatchDocument patchDocument,
        CancellationToken cancellationToken = default)
    {
        return StatusCode(StatusCodes.Status405MethodNotAllowed);
    }

    [Authorize]
    [HttpPut("{id:guid}/employees/{employeeId:guid}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    public ActionResult ReplaceEmployeeAsync([FromRoute] Guid id, [FromRoute] Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return StatusCode(StatusCodes.Status405MethodNotAllowed);
    }
}
