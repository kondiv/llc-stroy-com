using Ardalis.Specification;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Models;
using LLCStroyCom.Domain.Models.PageTokens;
using LLCStroyCom.Domain.Specifications.Companies;

namespace LLCStroyCom.Domain.Repositories;

public interface ICompanyRepository
{
    Task<PaginationResult<Company>> ListAsync(CompanySpecification specification, int maxPageSize, int page,
        CancellationToken cancellationToken = default);
    Task<Company> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Company> GetExtendedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Company> CreateAsync(Company company, CancellationToken cancellationToken = default);
    Task UpdateAsync(Company company, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}