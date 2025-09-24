using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Requests;

namespace LLCStroyCom.Domain.Services;

public interface ICompanyService
{
    Task CreateAsync(CreateCompanyRequest request);
    Task UpdateAsync();
    Task<CompanyDto> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}