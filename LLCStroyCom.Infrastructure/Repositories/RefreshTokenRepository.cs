using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly StroyComDbContext _context;
    private readonly ITokenHasher _tokenHasher;

    public RefreshTokenRepository(StroyComDbContext context, ITokenHasher tokenHasher)
    {
        _context = context;
        _tokenHasher = tokenHasher;
    }
    
    public async Task<RefreshToken?> GetAsync(string token, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(token);
        ArgumentException.ThrowIfNullOrEmpty(token);
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var hashedToken = _tokenHasher.HashToken(token);
        
        return await _context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hashedToken, cancellationToken);
    }

    public async Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync(cancellationToken);
    }
}