using Ardalis.Specification.EntityFrameworkCore;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Models;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.ResultPattern;
using LLCStroyCom.Domain.ResultPattern.Errors;
using LLCStroyCom.Domain.Specifications.Defects;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LLCStroyCom.Infrastructure.Repositories;

public sealed class DefectRepository : IDefectRepository
{
    private readonly StroyComDbContext _context;

    public DefectRepository(StroyComDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Defect>> GetAsync(Guid projectId, Guid defectId, CancellationToken cancellationToken = default)
    {
        var defect = await _context.Defects.FindAsync([projectId, defectId], cancellationToken);

        return defect is null
            ? Result<Defect>.Failure(new NotFoundError($"Cannot find defect {defectId} for project {projectId}")) 
            : Result<Defect>.Success(defect);
    }

    public async Task<PaginationResult<Defect>> ListAsync(Guid projectId, DefectSpecification specification, int maxPageSize, int page,
        CancellationToken cancellationToken = default)
    {
        if (maxPageSize is <= 0 or > 45)
        {
            throw new ArgumentOutOfRangeException(nameof(maxPageSize), maxPageSize, "The MaxSageSize must be greater than zero.");
        }
        if (page <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), page, "The Page must be greater than zero.");
        }
        
        var query = _context.Defects.Where(d => d.ProjectId == projectId).AsQueryable();

        var filteredQuery = SpecificationEvaluator.Default.GetQuery(query, specification);

        var totalCount = await filteredQuery.CountAsync(cancellationToken);
        var pageCount = (int)Math.Ceiling(totalCount / (double)maxPageSize);
        
        var items = await filteredQuery
            .Skip((page - 1) * maxPageSize)
            .Take(maxPageSize)
            .ToListAsync(cancellationToken);
        
        return new PaginationResult<Defect>(items, page, maxPageSize, pageCount, totalCount);
    }

    public async Task<Result<Defect>> CreateAsync(Defect defect, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Defects.AddAsync(defect, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Defect>.Success(defect);
        }
        catch (DbUpdateException e)
            when (e.InnerException is NpgsqlException { SqlState: PostgresErrorCodes.ForeignKeyViolation })
        {
            return Result<Defect>.Failure(new DbUpdateError("Project id is missing or project does not exist"));
        }
        catch (DbUpdateException e)
            when (e.InnerException is NpgsqlException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            return Result<Defect>.Failure(new AlreadyExistsError("Defect already exists"));
        }
    }

    public async Task<Result> UpdateAsync(Defect defect, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Defects.Update(defect);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DbUpdateException e)
            when (e.InnerException is NpgsqlException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            return Result.Failure(new AlreadyExistsError("Defect with such parameters already exists"));
        }
        catch (DbUpdateConcurrencyException e)
        {
            return Result.Failure(new DbUpdateConcurrencyError("Defect has been updated by other person. Request updated defect and try again"), e);
        }
    }

    public async Task<Result> DeleteAsync(Guid projectId, Guid defectId, CancellationToken cancellationToken = default)
    {
        var defect = await _context.Defects.FindAsync([projectId, defectId], cancellationToken);

        if (defect is null)
        {
            return Result.Failure(new NotFoundError($"Cannot find defect {defectId} for project {projectId}"));
        }
        
        _context.Defects.Remove(defect);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}