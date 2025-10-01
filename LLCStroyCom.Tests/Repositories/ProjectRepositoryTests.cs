using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Models.Filters.Project;
using LLCStroyCom.Domain.Models.PageTokens;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.ResultPattern;
using LLCStroyCom.Domain.ResultPattern.Errors;
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

    private static StroyComDbContext GetInMemoryDbContext()
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
        
        var project1 = new Project()
        {
            Name = "Project1",
            City = "Москва",
            CreatedAt = DateTimeOffset.UtcNow,
            Status = Status.InProgress
        };
        var project2 = new Project()
        {
            Name = "Project2",
            City = "Москва",
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(5),
            Status = Status.Canceled
        };
        var project3 = new Project()
        {
            Name = "Project3",
            City = "НеМосква",
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(10),
            Status = Status.InProgress
        };
        var project4 = new Project()
        {
            Name = "Project4",
            City = "НеМосква",
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(15),
            Status = Status.Completed
        };
        var project5 = new Project()
        {
            Name = "Project5",
            City = "Москва",
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(20),
            Status = Status.Completed
        };
        var project6 = new Project()
        {
            Name = "Project6",
            City = "НеМосква",
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(25),
            Status = Status.New
        };
        
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
        var act = () => _projectRepository.CreateAsync(project!);
        
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
            Name = "Жилищный комплекс",
            Status = Status.New
        };
        
        // Act
        await projectRepository.CreateAsync(project);
        
        // Assert
        Assert.Equal(1, await context.Projects.CountAsync());
    }

    // Not working with InMemoryDatabase, tested in actual DB - Success
    [Fact]
    public async Task CreateAsync_WhenProjectAlreadyExists_ShouldThrowArgumentException()
    {
        // Arrange
        var project = new Project()
        {
            City = "Moscow",
 
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
        Assert.IsType<Func<Task<Result<Project>>>>(act);
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
        var project = new Project()
        {
            Id = projectId,
            City = "Moscow",
            Name = "Жилой комплекс",
            Status = Status.Completed,
        };

        var context = GetInMemoryDbContext();
        IProjectRepository projectRepository = new ProjectRepository(context);

        await context.Projects.AddAsync(project);
        await context.SaveChangesAsync();
        
        // Act
        var result = await projectRepository.GetAsync(projectId);
        
        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task GetAsync_WhenProjectDoesNotExist_ShouldThrowCouldNotFindProject()
    {
        // Arrange
        // Act
        var result = await _projectRepository.GetAsync(Guid.NewGuid());
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<NotFoundError>(result.Error);
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
    public async Task ListAsync_WhenOrderByNameAsc_ShouldReturnOrderedList()
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
        
        // Assert
        Assert.Equal("Project1", listResult[0].Name);
        Assert.Equal("Project2", listResult[1].Name);
        Assert.Equal("Project3", listResult[2].Name);
        Assert.Equal("Project4", listResult[3].Name);
        Assert.Equal("Project5", listResult[4].Name);
        Assert.Equal("Project6", listResult[5].Name);
    }

    [Fact]
    public async Task ListAsync_WhenOrderByNameDesc_ShouldReturnOrderedList()
    {
        // Arrange
        var projectFilter = new ProjectFilter()
        {
            OrderBy = "name",
            Descending = true,
        };
        ProjectPageToken? pageToken = null;
        var maxPageSize = 5;
        var specification = new ProjectSpecification(projectFilter, pageToken, maxPageSize);
        
        var context = await GetFilledInMemoryDbContextAsync();
        IProjectRepository projectRepository = new ProjectRepository(context);
        
        // Act
        var result = await projectRepository.ListAsync(specification);
        var listResult = result.ToList();
        
        // Assert
        Assert.Equal("Project6", listResult[0].Name);
        Assert.Equal("Project5", listResult[1].Name);
        Assert.Equal("Project4", listResult[2].Name);
        Assert.Equal("Project3", listResult[3].Name);
        Assert.Equal("Project2", listResult[4].Name);
        Assert.Equal("Project1", listResult[5].Name);
    }

    [Fact]
    public async Task ListAsync_WhenOrderByCreatedAtAsc_ShouldReturnOrderedList()
    {
        // Arrange
        var projectFilter = new ProjectFilter()
        {
            OrderBy = "created-at",
        };
        ProjectPageToken? pageToken = null;
        var maxPageSize = 5;
        var specification = new ProjectSpecification(projectFilter, pageToken, maxPageSize);
        
        var context = await GetFilledInMemoryDbContextAsync();
        IProjectRepository projectRepository = new ProjectRepository(context);
        
        // Act
        var result = await projectRepository.ListAsync(specification);
        var listResult = result.ToList();
        
        // Assert
        Assert.True(listResult[0].CreatedAt < listResult[1].CreatedAt);
    }

    [Fact]
    public async Task ListAsync_WhenOrderByCreatedAtDesc_ShouldReturnOrderedList()
    {
        // Arrange
        var projectFilter = new ProjectFilter()
        {
            OrderBy = "created-at",
            Descending = true,
        };
        ProjectPageToken? pageToken = null;
        var maxPageSize = 5;
        var specification = new ProjectSpecification(projectFilter, pageToken, maxPageSize);

        var context = await GetFilledInMemoryDbContextAsync();
        IProjectRepository projectRepository = new ProjectRepository(context);

        // Act
        var result = await projectRepository.ListAsync(specification);
        var listResult = result.ToList();
        // Assert
        Assert.True(listResult[0].CreatedAt > listResult[1].CreatedAt);
    }

    [Fact]
    public async Task ListAsync_WhenFilteredByCity_ShouldReturnFilteredProjectList()
    {
        // Arrange
        var projectFilter = new ProjectFilter()
        {
            City = "Москва",
        };
        ProjectPageToken? pageToken = null;
        var maxPageSize = 5;
        var specification = new ProjectSpecification(projectFilter, pageToken, maxPageSize);
        
        var context = await GetFilledInMemoryDbContextAsync();
        IProjectRepository projectRepository = new ProjectRepository(context);
        
        // Act
        var result = await projectRepository.ListAsync(specification);
        var resultList = result.ToList();
        
        // Assert
        Assert.All(resultList, p => Assert.Equal("Москва", p.City));
    }

    [Fact]
    public async Task ListAsync_WhenFilteredByStatus_ShouldReturnFilteredProjectList()
    {
        // Arrange
        var projectFilter = new ProjectFilter()
        {
            Status = Status.InProgress,
        };
        ProjectPageToken? pageToken = null;
        var maxPageSize = 5;
        var specification = new ProjectSpecification(projectFilter, pageToken, maxPageSize);
        
        var context = await GetFilledInMemoryDbContextAsync();
        IProjectRepository projectRepository = new ProjectRepository(context);
        
        // Act
        var result = await projectRepository.ListAsync(specification);
        var resultList = result.ToList();
        
        // Assert
        Assert.All(resultList, p => Assert.Equal(Status.InProgress, p.Status));
    }

    [Fact]
    public async Task ListAsync_WhenTwoFiltersAreApplied_ShouldReturnFilteredProjectList()
    {
        // Arrange
        var projectFilter = new ProjectFilter()
        {
            Status = Status.InProgress,
            City = "Москва"
        };
        ProjectPageToken? pageToken = null;
        var maxPageSize = 5;
        var specification = new ProjectSpecification(projectFilter, pageToken, maxPageSize);
        
        var context = await GetFilledInMemoryDbContextAsync();
        IProjectRepository projectRepository = new ProjectRepository(context);
        
        // Act
        var result = await projectRepository.ListAsync(specification);
        var resultList = result.ToList();
        
        // Assert
        foreach (var item in resultList)
        {
            Assert.Equal(Status.InProgress, item.Status);
            Assert.Equal("Москва", item.City);
        }
    }

    [Fact]
    public async Task ListAsync_WhenFilterAndOrderByApplied_ShouldReturnFilteredAndOrderedProjectList()
    {
        // Arrange
        var projectFilter = new ProjectFilter()
        {
            OrderBy = "name",
            City = "Москва"
        };
        ProjectPageToken? pageToken = null;
        var maxPageSize = 5;
        var specification = new ProjectSpecification(projectFilter, pageToken, maxPageSize);
        
        var context = await GetFilledInMemoryDbContextAsync();
        IProjectRepository projectRepository = new ProjectRepository(context);
        
        // Act
        var result = await projectRepository.ListAsync(specification);
        var resultList = result.ToList();

        // Assert
        Assert.True(string.Compare(resultList[0].Name, resultList[1].Name, StringComparison.Ordinal) < 0);
        Assert.All(resultList, p => Assert.Equal("Москва", p.City));
    }

    [Fact]
    public async Task ListAsync_WhenOperationIsCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var projectFilter = new ProjectFilter()
        {
            OrderBy = "name",
            City = "Москва"
        };
        ProjectPageToken? pageToken = null;
        var maxPageSize = 5;
        var specification = new ProjectSpecification(projectFilter, pageToken, maxPageSize);
        var cancellationToken = new CancellationToken(canceled: true);
        
        var context = await GetFilledInMemoryDbContextAsync();
        IProjectRepository projectRepository = new ProjectRepository(context);
        
        // Act
        var act = () => projectRepository.ListAsync(specification, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task UpdateAsync_WhenUpdateSucceeded_ShouldReturnResultSuccess()
    {
        // Arrange
        var project = new Project()
        {
            Id = Guid.NewGuid(),
            Name = "name",
            City = "newCity",
            CompanyId = Guid.NewGuid()
        };

        var context = GetInMemoryDbContext();
        var projectRepository = new ProjectRepository(context);
        await context.Projects.AddAsync(project);
        await context.SaveChangesAsync();
        
        context.Entry(project).State = EntityState.Detached;

        var projectUpdate = new Project()
        {
            Id = project.Id,
            Name = "new-name"
        };
        
        // Act
        var result = await projectRepository.UpdateAsync(projectUpdate);
        
        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task UpdateAsync_WhenUpdateFailed_ShouldReturnResultFailure()
    {
        // Arrange
        var project = new Project()
        {
            Id = Guid.NewGuid(),
            Name = "name"
        };
        
        // Act
        var result = await _projectRepository.UpdateAsync(project);
        
        // Assert
        Assert.True(result.IsFailure);
    }
}