using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.ResultPattern;
using LLCStroyCom.Domain.Specifications.Projects;

namespace LLCStroyCom.Domain.Repositories;

public interface IProjectRepository
{
    Task<Result<Project>> CreateAsync(Project project, CancellationToken cancellationToken = default);
    Task<Result<Project>> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> ListAsync(ProjectSpecification specification, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Project project, CancellationToken cancellationToken = default);
}