using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly StroyComDbContext _context;

    public UserRepository(StroyComDbContext context)
    {
        _context = context;
    }
    
    public async Task<Guid> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        try
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return user.Id;
        }
        catch (Exception ex)
        {
            if (ex.InnerException is not OperationCanceledException ||
                ex.InnerException is not TaskCanceledException) throw;
            
            throw new OperationCanceledException("User creating canceled");
        }
    }

    public async Task<ApplicationUser> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentException.ThrowIfNullOrEmpty(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken)
               ?? throw UserCouldNotBeFound.WithEmail(email);
    }

    public async Task AssignRefreshTokenAsync(Guid userId, RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);
        
        var user = await _context.Users.FindAsync([userId], cancellationToken)
            ?? throw UserCouldNotBeFound.WithId(userId);
        
        user.RefreshTokens.Add(refreshToken);
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Users.FindAsync(new object?[] { id, cancellationToken }, cancellationToken)
                       ?? throw UserCouldNotBeFound.WithId(id);
            
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.InnerException is not OperationCanceledException ||
                ex.InnerException is not TaskCanceledException) throw;
            
            throw new OperationCanceledException("User deleting canceled");
        }
    }
}