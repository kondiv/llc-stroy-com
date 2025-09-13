using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Domain.Repositories;

public interface IProjectRepository
{
    Task CreateAsync(Project project, CancellationToken cancellationToken = default);
    Task GetAsync(Guid id, CancellationToken cancellationToken = default);
}