using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Response;

namespace LLCStroyCom.Domain.Repositories;

public interface IDefectRepository
{
    Task<Defect> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task CreateAsync(Defect defect, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Guid defectId, Status newStatus, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}