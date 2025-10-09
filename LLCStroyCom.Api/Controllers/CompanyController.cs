using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Models;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.Services;
using LLCStroyCom.Domain.Specifications.Companies;
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
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PaginationResult<CompanyDto>>> ListAsync(
        [FromQuery] CompanyFilter filter,
        [FromQuery] int maxPageSize = 20, [FromQuery] int page = 1, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Requesting list of companies.\nMax page size: {maxPageSize}\nPage: {page}",
            maxPageSize, page);
        try
        {
            var specification = new CompanySpecification(filter);
        
            var result = await _companyService.ListAsync(specification, maxPageSize, page, cancellationToken);
        
            return Ok(result);
        }
        catch (ArgumentOutOfRangeException e)
        {
            _logger.LogError(e, e.Message);
            return BadRequest(e.Message);
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
}
