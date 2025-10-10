using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Models;
using LLCStroyCom.Domain.Services;
using LLCStroyCom.Domain.Specifications.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Api.Controllers;

[ApiController]
[Route("api/companies/{companyId}/employees")]
public class CompanyEmployeesController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<CompanyEmployeesController> _logger;

    public CompanyEmployeesController(ICompanyService companyService, ILogger<CompanyEmployeesController> logger)
    {
        _companyService = companyService;
        _logger = logger;
    }
    
    [Authorize(Policy = "CompanyEmployee", Roles = "manager,observer")]
    [HttpGet("{employeeId:guid}", Name = "GetEmployee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EmployeeDto>> GetAsync([FromRoute] Guid companyId, [FromRoute] Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Request for employee: {id}", companyId);
            var employee = await _companyService.GetEmployeeAsync(companyId, employeeId, cancellationToken);
            return Ok(employee);
        }
        catch (CouldNotFind e)
        {
            _logger.LogWarning(e, e.Message);
            return NotFound(e.Message);
        }
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PaginationResult<EmployeeDto>>> ListAsync([FromRoute] Guid companyId,
        [FromQuery]ApplicationUserFilter filter,
        [FromQuery]int maxPageSize = 20, [FromQuery]int page = 1, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Requesting list of employees from company {companyId}\nMax page size: {maxPageSize}\nPage: {page}",
            companyId, maxPageSize, page);
        try
        {
            var specification = new ApplicationUserSpecification(filter);
        
            var result = await _companyService.ListCompanyEmployeesAsync(companyId, specification, maxPageSize, page, cancellationToken);

            return Ok(result);
        }
        catch (ArgumentOutOfRangeException e)
        {
            _logger.LogError(e, e.Message);
            return BadRequest(e.Message);
        }
    }
    
    [Authorize(Policy = "CompanyEmployee", Roles = "manager,observer")]
    [HttpPost("{employeeId:guid}:hire")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Guid>> HireAsync([FromRoute] Guid companyId, [FromRoute] Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Request for adding employee: {employeeId} to company: {id}", employeeId, companyId);
            var result = await _companyService.AddEmployeeAsync(companyId, employeeId, cancellationToken);
            return CreatedAtRoute("GetEmployee", new {companyId, employeeId = result}, result);
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

    [Authorize(Policy = "CompanyEmployee", Roles = "manager,observer")]
    [HttpDelete("{employeeId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveAsync([FromRoute] Guid companyId, [FromRoute] Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Request for removing employee: {employeeId} from company: {id}", employeeId, companyId);
            await _companyService.RemoveEmployeeAsync(companyId, employeeId, cancellationToken);
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

    [Authorize(Policy = "CompanyEmployee", Roles = "manager,observer")]
    [HttpPatch("{employeeId:guid}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    public ActionResult UpdateAsync([FromRoute] Guid companyId, [FromRoute] Guid employeeId, JsonPatchDocument patchDocument,
        CancellationToken cancellationToken = default)
    {
        return StatusCode(StatusCodes.Status405MethodNotAllowed);
    }

    [Authorize(Policy = "CompanyEmployee", Roles = "manager,observer")]
    [HttpPut("{employeeId:guid}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    public ActionResult ReplaceEmployeeAsync([FromRoute] Guid companyId, [FromRoute] Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return StatusCode(StatusCodes.Status405MethodNotAllowed);
    }
}