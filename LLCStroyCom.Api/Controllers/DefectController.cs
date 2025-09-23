using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LLCStroyCom.Api.Controllers;

[ApiController]
[Route("api/defects")]
public sealed class DefectController : Controller
{
    private readonly IDefectService _defectService;

    public DefectController(IDefectService defectService)
    {
        _defectService = defectService;
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DefectDto>> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var defect = await _defectService.GetAsync(id, cancellationToken);
            return Ok(defect);
        }
        catch (CouldNotFindDefect e)
        {
            return NotFound(e.Message);
        }
    }
}