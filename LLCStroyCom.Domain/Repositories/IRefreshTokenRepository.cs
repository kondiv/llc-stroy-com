using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetAsync(string token, CancellationToken cancellationToken = default);
    Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken = default);
}