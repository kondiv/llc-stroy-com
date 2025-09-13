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
    
    public async Task CreateAsync(Project project, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);
        
        await _context.Projects.AddAsync(project, cancellationToken); 
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<Project> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}