using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Services;
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

    // TODO Write tests
    public async Task<ApplicationUser> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
                   .Include(u => u.RefreshTokens)
                   .Include(u => u.Role)
                   .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
               ?? throw UserCouldNotBeFound.WithId(id);
    }

    public async Task<ApplicationUser> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentException.ThrowIfNullOrEmpty(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        return await _context.Users
                   .Include(u => u.Role)
                   .FirstOrDefaultAsync(u => u.Email == email, cancellationToken)
               ?? throw UserCouldNotBeFound.WithEmail(email);
    }

    public async Task AssignNewAndRevokeOldRefreshTokenAsync(Guid userId, RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        var user = await GetAsync(userId, cancellationToken);

        var lastActiveToken = user.RefreshTokens
            .OrderByDescending(rt => rt.CreatedAt)
            .FirstOrDefault(rt => rt.IsActive);

        if (lastActiveToken != null)
        {
            lastActiveToken.RevokedAt = DateTimeOffset.UtcNow;
        }
        
        user.RefreshTokens.Add(refreshToken);
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await GetAsync(id, cancellationToken);
            
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