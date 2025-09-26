using Ardalis.Specification;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Models.PageTokens;
using LLCStroyCom.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Infrastructure.Repositories;

public sealed class CompanyRepository : ICompanyRepository
{
    private readonly StroyComDbContext _context;

    public CompanyRepository(StroyComDbContext context)
    {
        _context = context;
    }
    
    public Task<IEnumerable<Company>> ListAsync(List<ISpecification<Company>> specifications, int maxPageSize, IPageToken pageToken,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Company> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Companies.FindAsync([id], cancellationToken) ??
               throw CouldNotFindCompany.WithId(id);
    }

    public async Task<Company> GetExtendedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Companies
            .Include(c => c.Employees)
            .Include(c => c.Projects)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw CouldNotFindCompany.WithId(id);
    }

    public async Task<Company> CreateAsync(Company company, CancellationToken cancellationToken = default)
    {
        await _context.Companies.AddAsync(company, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return company;
    }

    public async Task UpdateAsync(Company company, CancellationToken cancellationToken = default)
    {
        _context.Companies.Update(company);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var company = await GetAsync(id, cancellationToken);
        _context.Companies.Remove(company);
        await _context.SaveChangesAsync(cancellationToken);
    }
}