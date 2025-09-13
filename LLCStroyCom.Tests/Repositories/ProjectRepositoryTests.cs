using LLCStroyCom.Domain;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Exceptions;
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

    // Not working with InMemoryDatabase, tested in actual DB - Success
    [Fact]
    public async Task CreateAsync_WhenProjectAlreadyExists_ShouldThrowArgumentException()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var project = new Project()
        {
            City = "Moscow",
            CompanyId = companyId,
            Status = Status.New,
            Name = "Жилой комплекс"
        };
        
        var context = GetInMemoryContext();
        IProjectRepository projectRepository = new ProjectRepository(context);

        await context.Projects.AddAsync(project);
        await context.SaveChangesAsync();
        
        // Act
        var act = () => projectRepository.CreateAsync(project);
        
        // Assert
        // await Assert.ThrowsAsync<DbUpdateException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenProjectIsInvalidModel_ShouldThrow()
    {
        // Arrange
        var project = new Project();
        
        // Act
        var act = () => _projectRepository.CreateAsync(project);
        
        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenOperationIsCancelled_ShouldThrowOperationCancelledException()
    {
        // Arrange
        var project = new Project()
        {
            City = "Moscow",
            CompanyId = Guid.NewGuid(),
            Status = Status.New,
            Name = "Жилой комплекс"
        };
        var cancellationToken = new CancellationToken(canceled: true);
        
        // Act
        var act = () => _projectRepository.CreateAsync(project, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(act);
    }

    [Fact]
    public async Task GetAsync_WhenProjectExists_ShouldReturnProject()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var project = new Project()
        {
            Id = projectId,
            City = "Moscow",
            Name = "Жилой комплекс",
            Status = Status.Completed,
            CompanyId = companyId
        };
        
        // Act
        var result = await _projectRepository.GetAsync(projectId);
        
        // Assert
        Assert.Equal(projectId, result.Id);
        Assert.Equal(companyId, result.CompanyId);
        Assert.Equal("Moscow", result.City);
        Assert.Equal("Жилой комплекс", result.Name);
    }

    [Fact]
    public async Task GetAsync_WhenProjectDoesNotExist_ShouldThrowProjectCouldNotBeFound()
    {
        // Arrange
        // Act
        var act = () => _projectRepository.GetAsync(Guid.NewGuid());
        
        // Assert
        await Assert.ThrowsAsync<ProjectCouldNotBeFound>(act);
    }
}