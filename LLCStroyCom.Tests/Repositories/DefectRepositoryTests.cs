using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Infrastructure;
using LLCStroyCom.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Tests.Repositories;

public class DefectRepositoryTests
{
    private readonly IDefectRepository _defectRepository;

    public DefectRepositoryTests()
    {
        var context = GetInMemoryContext();
        _defectRepository = new DefectRepository(context);
    }
    
    private static StroyComDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<StroyComDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        return new StroyComDbContext(options);
    }

    [Fact]
    public async Task GetAsync_WhenDefectNotFound_ShouldThrowCouldNotFindDefect()
    {
        // Arrange
        
        // Act
        var act = () => _defectRepository.GetAsync(Guid.NewGuid());
        
        // Assert
        await Assert.ThrowsAsync<CouldNotFindDefect>(act);
    }

    [Fact]
    public async Task GetAsync_WhenDefectFound_ShouldReturnDefect()
    {
        // Arrange
        var context = GetInMemoryContext();
        var defectId = Guid.NewGuid();
        var defect = new Defect()
        {
            Id = defectId,
            Name = "Трещина",
            Description = "Трещина",
            ProjectId = Guid.NewGuid(),
        };
        var project = new Project()
        {
            Id = defect.ProjectId,
            City = "Moscow",
            CreatedAt = DateTimeOffset.UtcNow,
            Name = "Project",
            Status = Status.InProgress,
            Defects = new List<Defect>() { defect }
        };
        
        await context.Projects.AddAsync(project);

        await context.Defects.AddAsync(defect);
        await context.SaveChangesAsync();
        
        var defectRepository = new DefectRepository(context);
        
        // Act
        var result = await defectRepository.GetAsync(defectId);
        
        // Assert
        Assert.Equal(defect.Id, result.Id);
        Assert.Equal(defect.Name, result.Name);
        Assert.Equal(defect.Description, result.Description);
        Assert.Equal(defect.ProjectId, result.ProjectId);
    }

    [Fact]
    public async Task GetAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        
        // Act
        var act = () => _defectRepository.GetAsync(Guid.NewGuid(), cancellationToken);
        
        // Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenAlreadyExists_ShouldThrowArgumentException()
    {
        // Arrange
        var defect = new Defect()
        {
            Id = Guid.NewGuid(),
            Name = "Трещина",
            Description = "Трещина",
            ProjectId = Guid.NewGuid(),
        };

        var context = GetInMemoryContext();
        await context.Defects.AddAsync(defect);
        await context.SaveChangesAsync();
        
        var defectRepository = new DefectRepository(context);
        
        // Act
        var act = () => defectRepository.CreateAsync(defect);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenInvalidModel_ShouldThrowDbUpdateException()
    {
        // Arrange
        var defect = new Defect();
        
        // Act
        var act = () => _defectRepository.CreateAsync(defect);
        
        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenEverythingIsOk_ShouldCreateNewDefectInDb()
    {
        // Arrange
        var context = GetInMemoryContext();
        var defectRepository = new DefectRepository(context);
        var defect = new Defect()
        {
            Id = Guid.NewGuid(),
            Name = "Трещина",
            Description = "Трещина",
            ProjectId = Guid.NewGuid(),
        };
        
        // Act
        await defectRepository.CreateAsync(defect);
        
        // Assert
        Assert.Equal(1, await context.Defects.CountAsync());
        Assert.Equal(Status.New, defect.Status);
    }

    [Fact]
    public async Task CreateAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        var defect = new Defect()
        {
            Id = Guid.NewGuid(),
            Name = "Трещина",
            Description = "Трещина",
            ProjectId = Guid.NewGuid(),
        };
        
        // Act
        var act = () => _defectRepository.CreateAsync(defect, cancellationToken);
        
        // Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task UpdateAsync_WhenDefectNotFound_ShouldThrowCouldNotFindDefect()
    {
        // Arrange
        
        // Act
        var act = () => _defectRepository.UpdateAsync(Guid.NewGuid(), Status.Completed);
        
        // Assert
        await Assert.ThrowsAsync<CouldNotFindDefect>(act);
    }

    [Fact]
    public async Task UpdateAsync_WhenEverythingIsOk_ShouldUpdateDefectInDb()
    {
        // Arrange
        var defectId = Guid.NewGuid();
        var defect = new Defect()
        {
            Id = defectId,
            Name = "Трещина",
            Description = "Трещина",
            ProjectId = Guid.NewGuid(),
        };
        var project = new Project()
        {
            Id = defect.ProjectId,
            City = "Moscow",
            CreatedAt = DateTimeOffset.UtcNow,
            Name = "Project",
            Status = Status.InProgress,
            Defects = new List<Defect>() { defect }
        };
        
        var context = GetInMemoryContext();
        await context.Projects.AddAsync(project);
        await context.Defects.AddAsync(defect);
        await context.SaveChangesAsync();
        
        var defectRepository = new DefectRepository(context);
        
        // Act
        var updateResult = await defectRepository.UpdateAsync(defectId, Status.Completed);
        
        // Assert
        Assert.True(updateResult.Succeeded);
        Assert.Equal(Status.Completed, defect.Status);
    }

    [Fact]
    public async Task UpdateAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        
        // Act
        var act = () => _defectRepository.UpdateAsync(Guid.NewGuid(), Status.Completed, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ShouldThrowCouldNotFindDefect()
    {
        // Arrange
        
        // Act
        var act = () =>  _defectRepository.DeleteAsync(Guid.NewGuid());
        
        // Assert
        await Assert.ThrowsAsync<CouldNotFindDefect>(act);
    }

    [Fact]
    public async Task DeleteAsync_WhenFound_ShouldDeleteDefectFromDb()
    {
        // Arrange
        var context = GetInMemoryContext();
        var defectId = Guid.NewGuid();
        var defect = new Defect()
        {
            Id = defectId,
            Name = "Трещина",
            Description = "Трещина",
            ProjectId = Guid.NewGuid(),
        };
        var project = new Project()
        {
            Id = defect.ProjectId,
            City = "Moscow",
            CreatedAt = DateTimeOffset.UtcNow,
            Name = "Project",
            Status = Status.InProgress,
            Defects = new List<Defect>() { defect }
        };
        
        await context.Projects.AddAsync(project);
        await context.Defects.AddAsync(defect);
        await context.SaveChangesAsync();
        
        var defectRepository = new DefectRepository(context);
        
        // Act
        await defectRepository.DeleteAsync(defectId);
        
        // Assert
        Assert.Equal(0, await context.Defects.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var context = GetInMemoryContext();
        var defectId = Guid.NewGuid();
        var defect = new Defect()
        {
            Id = defectId,
            Name = "Трещина",
            Description = "Трещина",
            ProjectId = Guid.NewGuid(),
        };
        var project = new Project()
        {
            Id = defect.ProjectId,
            City = "Moscow",
            CreatedAt = DateTimeOffset.UtcNow,
            Name = "Project",
            Status = Status.InProgress,
            Defects = new List<Defect>() { defect }
        };
        
        await context.Projects.AddAsync(project);
        await context.Defects.AddAsync(defect);
        await context.SaveChangesAsync();
        
        var defectRepository = new DefectRepository(context);
        
        var cancellationToken = new CancellationToken(canceled: true);
        
        // Act
        var act = () => defectRepository.DeleteAsync(defectId, cancellationToken);
        
        // Assert
        Assert.Equal(1, await context.Defects.CountAsync());
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }
}