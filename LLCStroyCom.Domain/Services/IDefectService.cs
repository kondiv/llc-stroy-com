using LLCStroyCom.Domain.Dto;

namespace LLCStroyCom.Domain.Services;

public interface IDefectService
{
    Task<DefectDto> GetAsync(Guid id, CancellationToken cancellationToken = default);
}