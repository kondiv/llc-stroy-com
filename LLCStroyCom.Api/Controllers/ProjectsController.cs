using Ardalis.Specification;
using LLCStroyCom.Api.Requests;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Specifications.Projects;
using Microsoft.AspNetCore.Mvc;

namespace LLCStroyCom.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectRepository _repository;

    public ProjectsController(IProjectRepository repository)
    {
        _repository = repository;
    }
    
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<Project>>> ListAsync([FromQuery] ProjectsQuery query,
        CancellationToken cancellationToken = default)
    {
        return Ok();
    }
}