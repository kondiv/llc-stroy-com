using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Models;
using LLCStroyCom.Domain.ResultPattern;
using LLCStroyCom.Domain.Specifications.Defects;

namespace LLCStroyCom.Domain.Repositories;

public interface IDefectRepository
{
    Task<Result<Defect>> GetAsync(Guid projectId, Guid defectId, CancellationToken cancellationToken = default);
    Task<PaginationResult<Defect>> ListAsync(Guid projectId, DefectSpecification specification, int maxPageSize, int page,
        CancellationToken cancellationToken = default);
    Task<Result<Defect>> CreateAsync(Defect defect, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Defect defect, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid projectId, Guid defectId, CancellationToken cancellationToken = default);
}