using Ardalis.Specification.EntityFrameworkCore;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.ResultPattern;
using LLCStroyCom.Domain.ResultPattern.Errors;
using LLCStroyCom.Domain.Specifications.Projects;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LLCStroyCom.Infrastructure.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly StroyComDbContext _context;

    public ProjectRepository(StroyComDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<Project>> CreateAsync(Project project, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Projects.AddAsync(project, cancellationToken); 
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Project>.Success(project);
        }
        catch (DbUpdateException ex) 
            when (ex.InnerException is NpgsqlException {SqlState: PostgresErrorCodes.UniqueViolation})
        {
            return Result<Project>.Failure(new AlreadyExistsError("Project already exists"));
        }
    }

    public async Task<Result<Project>> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects.FindAsync([id], cancellationToken);

        return project is null ? Result<Project>.Failure(new NotFoundError("Cannot find project: " + id))
            : Result<Project>.Success(project);
    }

    public async Task<IEnumerable<Project>> ListAsync(ProjectSpecification specification, CancellationToken cancellationToken = default)
    {
        var query = _context.Projects.AsQueryable();
        
        query = SpecificationEvaluator.Default.GetQuery(query, specification);
        
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Result> UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DbUpdateException e)
            when (e.InnerException is NpgsqlException {SqlState: PostgresErrorCodes.UniqueViolation})
        {
            return Result.Failure(new AlreadyExistsError("Project with such parameters already exists"));
        }
        catch (DbUpdateConcurrencyException e)
        {
            return Result.Failure(new DbUpdateConcurrencyError("Project has been updated. Request updated project and try again"));
        }
    }
}