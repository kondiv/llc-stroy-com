using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.ResultPattern;
using Microsoft.AspNetCore.JsonPatch;

namespace LLCStroyCom.Domain.Services;

public interface IDefectService
{
    Task<Result<DefectDto>> GetAsync(Guid projectId, Guid defectId, CancellationToken cancellationToken = default);
    Task<Result<DefectDto>> CreateAsync(Guid projectId, DefectCreateRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Guid projectId, Guid defectId, JsonPatchDocument<DefectPatchDto> patchDocument,
        CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid projectId, Guid defectId, CancellationToken cancellationToken = default);
}