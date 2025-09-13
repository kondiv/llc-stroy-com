using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Repositories;

namespace LLCStroyCom.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly StroyComDbContext _context;

    public ProjectRepository(StroyComDbContext context)
    {
        _context = context;
    }
    
    public Task CreateAsync(Project project, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}