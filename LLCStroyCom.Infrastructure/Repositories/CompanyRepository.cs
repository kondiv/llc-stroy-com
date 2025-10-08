using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Models;
using LLCStroyCom.Domain.Models.PageTokens;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Specifications.Companies;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Infrastructure.Repositories;

public sealed class CompanyRepository : ICompanyRepository
{
    private readonly StroyComDbContext _context;

    public CompanyRepository(StroyComDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResult<Company>> ListAsync(CompanySpecification specification, int maxPageSize, int page,
        CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator.Default.GetQuery(_context.Companies.AsQueryable(), specification);

        var totalCount = await query.CountAsync(cancellationToken);
        var pageCount = (int)Math.Ceiling(totalCount / (double)maxPageSize);
        
        var items = await query
            .Skip((page - 1) * maxPageSize)
            .Take(maxPageSize)
            .ToListAsync(cancellationToken);
        
        return new PaginationResult<Company>(items, page, maxPageSize, pageCount, totalCount);
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