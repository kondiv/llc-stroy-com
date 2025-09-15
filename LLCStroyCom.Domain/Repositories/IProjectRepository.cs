using Ardalis.Specification;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Models.PageTokens;

namespace LLCStroyCom.Domain.Repositories;

public interface IProjectRepository
{
    Task CreateAsync(Project project, CancellationToken cancellationToken = default);
    Task<Project> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> ListAsync(List<ISpecification<Project>> specifications, int maxPageSize, ProjectPageToken pageToken,
        CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(Guid id, Status status, CancellationToken cancellationToken = default);
}