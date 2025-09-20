using Ardalis.Specification.EntityFrameworkCore;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Models.PageTokens;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Specifications.Projects;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Infrastructure.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly StroyComDbContext _context;

    public ProjectRepository(StroyComDbContext context)
    {
        _context = context;
    }
    
    public async Task CreateAsync(Project project, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        
        await _context.Projects.AddAsync(project, cancellationToken); 
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Project> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects.FindAsync([id], cancellationToken);
        
        return project ?? throw CouldNotFindProject.WithId(id);
    }

    public async Task<IEnumerable<Project>> ListAsync(ProjectSpecification specification, CancellationToken cancellationToken = default)
    {
        var query = _context.Projects.AsQueryable();
        
        query = SpecificationEvaluator.Default.GetQuery(query, specification);
        
        return await query.ToListAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(Guid id, Status status, CancellationToken cancellationToken = default)
    {
        var project = await GetAsync(id, cancellationToken);

        if (project.Status != status)
        {
            project.Status = status;
        }
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}