using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Infrastructure.Repositories;

public sealed class RoleRepository : IRoleRepository
{
    private readonly StroyComDbContext _context;

    public RoleRepository(StroyComDbContext context)
    {
        _context = context;
    }
    
    public async Task<ApplicationRole> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Type == name, cancellationToken)
                   ?? throw CouldNotFindRole.WithName(name);

        return role;
    }
}