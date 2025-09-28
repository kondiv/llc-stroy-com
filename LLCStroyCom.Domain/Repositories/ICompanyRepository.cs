using Ardalis.Specification;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Models.PageTokens;

namespace LLCStroyCom.Domain.Repositories;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> ListAsync(List<ISpecification<Company>> specifications, int maxPageSize, IPageToken pageToken,
        CancellationToken cancellationToken = default);
    Task<Company> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Company> GetExtendedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Company> CreateAsync(Company company, CancellationToken cancellationToken = default);
    Task UpdateAsync(Company company, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}