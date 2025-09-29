using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Enums;

namespace LLCStroyCom.Domain.Repositories;

public interface IDefectRepository
{
    Task<Defect> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task CreateAsync(Defect defect, CancellationToken cancellationToken = default);
    Task<ResultPattern.Result> UpdateAsync(Guid defectId, Status newStatus, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}