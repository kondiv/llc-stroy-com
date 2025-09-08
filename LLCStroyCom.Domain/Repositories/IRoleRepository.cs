using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Domain.Repositories;

public interface IRoleRepository
{
    Task<ApplicationRole> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}