using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Response;

namespace LLCStroyCom.Infrastructure.Repositories;

public class DefectRepository : IDefectRepository
{
    private readonly StroyComDbContext _context;

    public DefectRepository(StroyComDbContext context)
    {
        _context = context;
    }
    
    public async Task<Defect> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Defects.FindAsync([id], cancellationToken) ??
               throw CouldNotFindDefect.WithId(id);
    }

    public async Task CreateAsync(Defect defect, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(defect);
        
        await _context.Defects.AddAsync(defect, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> UpdateAsync(Guid defectId, Status newStatus, CancellationToken cancellationToken = default)
    {
        var defect = await GetAsync(defectId, cancellationToken);

        if(defect.Status != newStatus)
        {
            defect.Status = newStatus;
        }
        
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var defect = await GetAsync(id, cancellationToken);

        _context.Defects.Remove(defect);
        await _context.SaveChangesAsync(cancellationToken);
    }
}