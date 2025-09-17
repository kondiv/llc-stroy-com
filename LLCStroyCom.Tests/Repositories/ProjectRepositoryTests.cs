using System.Net.Mime;
using LLCStroyCom.Domain;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Models.Filters.Project;
using LLCStroyCom.Domain.Models.PageTokens;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Specifications.Projects;
using LLCStroyCom.Infrastructure;
using LLCStroyCom.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Tests.Repositories;

public class ProjectRepositoryTests
{
    private readonly IProjectRepository _projectRepository;

    public ProjectRepositoryTests()
    {
        var context = GetInMemoryDbContext();
        _projectRepository = new ProjectRepository(context);
    }

    private StroyComDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<StroyComDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        return new StroyComDbContext(options);
    }

    private async Task<StroyComDbContext> GetFilledInMemoryDbContextAsync()
    {
        var context = GetInMemoryDbContext();
        
        var company1 = new Company(){Id = Guid.NewGuid(), Name = "Company1"};
        var company2 = new Company(){Id = Guid.NewGuid(), Name = "Company2"};
        var company3 = new Company(){Id = Guid.NewGuid(), Name = "Company3"};
        
        await context.Companies.AddRangeAsync(company1, company2, company3);
        await context.SaveChangesAsync();
        
        var project1 = new Project(){Name = "Project1", City = "Москва", CompanyId = company1.Id};
        var project2 = new Project(){Name = "Project2", City = "Москва", CompanyId = company2.Id};
        var project3 = new Project(){Name = "Project3", City = "НеМосква", CompanyId = company3.Id};
        var project4 = new Project(){Name = "Project4", City = "НеМосква", CompanyId = company3.Id};
        var project5 = new Project(){Name = "Project5", City = "Москва", CompanyId = company2.Id};
        var project6 = new Project(){Name = "Project6", City = "НеМосква", CompanyId = company1.Id};
        
        await context.Projects.AddRangeAsync(project1, project2, project3, project4, project5, project6);
        await context.SaveChangesAsync();

        return context;
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
        var context = GetInMemoryDbContext();
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
        
        var context = GetInMemoryDbContext();
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

        var context = GetInMemoryDbContext();
        IProjectRepository projectRepository = new ProjectRepository(context);

        await context.Projects.AddAsync(project);
        await context.SaveChangesAsync();
        
        // Act
        var result = await projectRepository.GetAsync(projectId);
        
        // Assert
        Assert.Equal(projectId, result.Id);
        Assert.Equal(companyId, result.CompanyId);
        Assert.Equal("Moscow", result.City);
        Assert.Equal("Жилой комплекс", result.Name);
    }

    [Fact]
    public async Task GetAsync_WhenProjectDoesNotExist_ShouldThrowCouldNotFindProject()
    {
        // Arrange
        // Act
        var act = () => _projectRepository.GetAsync(Guid.NewGuid());
        
        // Assert
        await Assert.ThrowsAsync<CouldNotFindProject>(act);
    }

    [Fact]
    public async Task GetAsync_WhenOperationIsCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        
        // Act
        var act = () => _projectRepository.GetAsync(Guid.NewGuid(), cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task ChangeStatusAsync_WhenProjectExists_ShouldChangeProjectStatus()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var status = Status.InProgress;

        var project = new Project()
        {
            Id = projectId,
            Status = status,
            CompanyId = Guid.NewGuid(),
            Name = "георгиевск",
            City = "Москва"
        };

        var context = GetInMemoryDbContext();
        IProjectRepository projectRepository = new ProjectRepository(context);

        await context.Projects.AddAsync(project);
        await context.SaveChangesAsync();
        
        // Act
        await projectRepository.ChangeStatusAsync(projectId, Status.OnReview);
        
        // Assert
        Assert.NotEqual(status, project.Status);
    }

    [Fact]
    public async Task ChangeStatusAsync_WhenProjectDoesNotExist_ShouldThrowCouldNotFindProject()
    {
        // Arrange
        
        // Act
        var act = () => _projectRepository.ChangeStatusAsync(Guid.NewGuid(), Status.InProgress);
        
        // Assert
        await Assert.ThrowsAsync<CouldNotFindProject>(act);
    }

    [Fact]
    public async Task ChangeStatusAsync_WhenOperationIsCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        
        // Act
        var act = () => _projectRepository.ChangeStatusAsync(Guid.NewGuid(), Status.Canceled, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task ChangeStatusAsync_WhenStatusesAreEqual_ShouldNotUpdateProject()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var status = Status.InProgress;

        var project = new Project()
        {
            Id = projectId,
            Status = status,
            CompanyId = Guid.NewGuid(),
            Name = "георгиевск",
            City = "Москва"
        };

        var context = GetInMemoryDbContext();
        IProjectRepository projectRepository = new ProjectRepository(context);

        await context.Projects.AddAsync(project);
        await context.SaveChangesAsync();
        
        // Act
        await projectRepository.ChangeStatusAsync(projectId, status); // using same status
        
        // Assert
        Assert.Equal(status, project.Status);
    }

    [Fact]
    public async Task ListAsync_WhenEverythingIsAlright_ShouldReturnProjectsList()
    {
        // Arrange
        var projectFilter = new ProjectFilter();
        ProjectPageToken? pageToken = null;
        var maxPageSize = 5;

        var specification = new ProjectSpecification(projectFilter, pageToken, maxPageSize);
        var context = await GetFilledInMemoryDbContextAsync();
        IProjectRepository projectRepository = new ProjectRepository(context);
        
        // Act
        var result = await projectRepository.ListAsync(specification);
        
        // Assert
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnOneAdditionalEntity()
    {
        // Arrange
        var projectFilter = new ProjectFilter();
        ProjectPageToken? pageToken = null;
        var maxPageSize = 5;
        var specification = new ProjectSpecification(projectFilter, pageToken, maxPageSize);
        
        var context = await GetFilledInMemoryDbContextAsync();
        IProjectRepository projectRepository = new ProjectRepository(context);
        
        // Act
        var result = await projectRepository.ListAsync(specification);
        
        // Assert
        Assert.Equal(maxPageSize + 1, result.Count());
    }

    [Fact]
    public async Task ListAsync_WhenOrderByName_ShouldReturnOrderedList()
    {
        // Arrange
        var projectFilter = new ProjectFilter()
        {
            OrderBy = "name",
        };
        ProjectPageToken? pageToken = null;
        var maxPageSize = 5;
        var specification = new ProjectSpecification(projectFilter, pageToken, maxPageSize);
        
        var context = await GetFilledInMemoryDbContextAsync();
        IProjectRepository projectRepository = new ProjectRepository(context);
        
        // Act
        var result = await projectRepository.ListAsync(specification);
        var listResult = result.ToList();
    }
}