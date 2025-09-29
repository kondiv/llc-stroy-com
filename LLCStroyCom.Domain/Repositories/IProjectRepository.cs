using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Specifications.Projects;

namespace LLCStroyCom.Domain.Repositories;

public interface IProjectRepository
{
    Task CreateAsync(Project project, CancellationToken cancellationToken = default);
    Task<Project> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> ListAsync(ProjectSpecification specification, CancellationToken cancellationToken = default);
}