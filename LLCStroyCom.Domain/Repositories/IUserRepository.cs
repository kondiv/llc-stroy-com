using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Domain.Repositories;

public interface IUserRepository
{
    Task CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}