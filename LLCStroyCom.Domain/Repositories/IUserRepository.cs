using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Domain.Repositories;

public interface IUserRepository
{
    Task<Guid> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task<ApplicationUser> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AssignRefreshTokenAsync(Guid userId, RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}