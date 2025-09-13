using LLCStroyCom.Domain;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Infrastructure;
using LLCStroyCom.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Tests.Repositories;

public class ProjectRepositoryTests
{
    private readonly IProjectRepository _projectRepository;

    public ProjectRepositoryTests()
    {
        var context = GetInMemoryContext();
        _projectRepository = new ProjectRepository(context);
    }

    private StroyComDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<StroyComDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        return new StroyComDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_WhenProjectIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Project? project = null;
        
        // Act
        var act = () => _projectRepository.CreateAsync(project);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenProjectIsValidEntity_ShouldCreateProject()
    {
        // Arrange
        var context = GetInMemoryContext();
        IProjectRepository projectRepository = new ProjectRepository(context);
        
        var project = new Project()
        {
            City = "Moscow",
            CompanyId = Guid.NewGuid(),
            Name = "Жилищный комплекс",
            Status = Status.New
        };
        
        // Act
        await projectRepository.CreateAsync(project);
        
        // Assert
        Assert.Equal(1, context.Projects.Count());
    }
}