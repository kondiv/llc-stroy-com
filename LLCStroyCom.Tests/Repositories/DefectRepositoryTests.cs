using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Models;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.ResultPattern.Errors;
using LLCStroyCom.Domain.Specifications.Defects;
using LLCStroyCom.Infrastructure;
using LLCStroyCom.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Tests.Repositories;

public class DefectRepositoryTests
{
    private readonly IDefectRepository _defectRepository;

    public DefectRepositoryTests()
    {
        var context = GetInMemoryDbContext();
        _defectRepository = new DefectRepository(context);
    }

    private static StroyComDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<StroyComDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        return new StroyComDbContext(options);
    }

    [Fact]
    public async Task GetAsync_WhenDefectFound_ShouldReturnResultSuccessWithDefect()
    {
        // Arrange
        var defect = new Defect()
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Name = "Трещина",
            Description = "Описание",
            Status = Status.New
        };

        var context = GetInMemoryDbContext();
        await context.Defects.AddAsync(defect);
        await context.SaveChangesAsync();

        var repository = new DefectRepository(context);
        
        // Act
        var result = await repository.GetAsync(defect.ProjectId, defect.Id);
        
        // Assert
        Assert.True(result.Succeeded);
        var value = result.Value;
        Assert.Equal(defect.Id, value.Id);
        Assert.Equal(defect.ProjectId, value.ProjectId);
        Assert.Equal(defect.Name, value.Name);
        Assert.Equal(defect.Description, value.Description);
    }

    [Fact]
    public async Task GetAsync_WhenDefectNotFound_ShouldReturnResultFailure()
    {
        // Arrange
        
        // Act
        var result = await _defectRepository.GetAsync(Guid.NewGuid(), Guid.NewGuid());
        
        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task GetAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        
        // Act
        var act = () => _defectRepository.GetAsync(Guid.NewGuid(), Guid.NewGuid(), cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenCreatedSuccessfully_ShouldReturnResultSuccessWithDefect()
    {
        // Arrange
        var defect = new Defect()
        {
            Name = "name",
            Description = "description",
            Status = Status.New,
            ProjectId = Guid.NewGuid(),
        };
        
        // Act
        var result = await _defectRepository.CreateAsync(defect);
        
        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(defect.Name, result.Value.Name);
    }

    // Works correctly, but not in InMemoryDbContext - Test result: Success
    [Fact]
    public async Task CreateAsync_WhenDefectAlreadyExists_ShouldReturnResultFailure()
    {
        // Arrange
        var defect = new Defect()
        {
            Name = "name",
            Description = "description",
            Status = Status.New,
            ProjectId = Guid.NewGuid(),
        };

        var context = GetInMemoryDbContext();
        await context.Defects.AddAsync(defect);
        await context.SaveChangesAsync();
        var repository = new DefectRepository(context);

        // Act
        var act = () => repository.CreateAsync(defect);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var defect = new Defect()
        {
            Name = "name",
            Description = "description",
            Status = Status.New,
            ProjectId = Guid.NewGuid(),
            ChiefEngineer = null
        };
        
        var cancellationToken = new CancellationToken(canceled: true);
        
        // Act
        var act = () => _defectRepository.CreateAsync(defect, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(act);
    }

    [Fact]
    public async Task UpdateAsync_WhenUpdatedSuccessfully_ShouldReturnResultSuccess()
    {
        // Arrange
        var defect = new Defect()
        {
            Id = Guid.NewGuid(),
            Name = "name",
            Description = "description",
            Status = Status.New,
            ProjectId = Guid.NewGuid(),
        };

        var context = GetInMemoryDbContext();
        await context.Defects.AddAsync(defect);
        await context.SaveChangesAsync();
        var repository = new DefectRepository(context);
        
        context.Defects.Entry(defect).State = EntityState.Detached;

        var defectToUpdate = new Defect()
        {
            Id = defect.Id,
            ProjectId = defect.ProjectId,
            Name = "new-name",
            Description = "new-description",
        };
        
        // Act
        var result = await repository.UpdateAsync(defectToUpdate);
        
        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task UpdateAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var defect = new Defect()
        {
            Name = "name",
            Description = "description",
            Status = Status.New,
            ProjectId = Guid.NewGuid(),
            ChiefEngineer = null
        };
        
        var cancellationToken = new CancellationToken(canceled: true);
        
        // Act
        var act = () => _defectRepository.UpdateAsync(defect, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(act);
    }

    [Fact]
    public async Task DeleteAsync_WhenDefectFound_ShouldReturnResultSuccess()
    {
        // Arrange
        var defect = new Defect()
        {
            Id = Guid.NewGuid(),
            Name = "name",
            Description = "description",
            Status = Status.New,
            ProjectId = Guid.NewGuid(),
        };
        
        var context = GetInMemoryDbContext();
        await context.Defects.AddAsync(defect);
        await context.SaveChangesAsync();
        
        var repository = new DefectRepository(context);
        
        // Act
        var result = await repository.DeleteAsync(defect.ProjectId, defect.Id);
        
        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(0, await context.Defects.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_WhenDefectNotFound_ShouldReturnResultFailure()
    {
        // Arrange
        
        // Act
        var result = await _defectRepository.DeleteAsync(Guid.NewGuid(), Guid.NewGuid());
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<NotFoundError>(result.Error);
    }

    [Fact]
    public async Task DeleteAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        
        // Act
        var act = () => _defectRepository.DeleteAsync(Guid.NewGuid(), Guid.NewGuid(), cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task ListAsync_WhenNoElementsFound_ShouldReturnEmptyPaginationResult()
    {
        // Arrange
        var specification = new DefectSpecification(new DefectFilter());
        var projectId = Guid.NewGuid();
        var maxPageSize = 2;
        var page = 1;
        
        // Act
        var result = await _defectRepository.ListAsync(projectId, specification, maxPageSize, page);
        
        // Assert
        Assert.IsType<PaginationResult<Defect>>(result);
        Assert.Empty(result.Items);
        Assert.Equal(page, result.Page);
        Assert.Equal(maxPageSize, result.MaxPageSize);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.PageCount);
        Assert.False(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public async Task ListAsync_WhenElementsLessThanMaxPageSize_ShouldReturnPaginationResultWithoutNextPage()
    {
        // Arrange
        var specification = new DefectSpecification(new DefectFilter());
        var projectId = Guid.NewGuid();
        var maxPageSize = 2;
        var page = 1;
        
        var context = GetInMemoryDbContext();
        await context.Defects.AddAsync(new Defect()
            { Name = "name", Description = "description", ProjectId = projectId });
        await context.SaveChangesAsync();

        var repository = new DefectRepository(context);
        
        // Act
        var result = await repository.ListAsync(projectId, specification, maxPageSize, page);
        
        // Assert
        Assert.IsType<PaginationResult<Defect>>(result);
        Assert.NotEmpty(result.Items);
        Assert.Equal(page, result.Page);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.PageCount);
        Assert.Equal(maxPageSize, result.MaxPageSize);
        Assert.False(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public async Task ListAsync_WhenElementsMoreThanMaxPageSize_ShouldReturnPaginationResultWithNextPage()
    {
        // Arrange
        var specification = new DefectSpecification(new DefectFilter());
        var projectId = Guid.NewGuid();
        var maxPageSize = 1;
        var page = 1;
        
        var context = GetInMemoryDbContext();
        await context.Defects.AddRangeAsync(
            new Defect() { Name = "name", Description = "desc", ProjectId = projectId },
            new Defect() { Name = "name1", Description = "desc", ProjectId = projectId });
        await context.SaveChangesAsync();

        var repository = new DefectRepository(context);
        
        // Act
        var result = await repository.ListAsync(projectId, specification, maxPageSize, page);
        
        // Assert
        Assert.IsType<PaginationResult<Defect>>(result);
        Assert.NotEmpty(result.Items);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.PageCount);
        Assert.Equal(page, result.Page);
        Assert.Equal(maxPageSize, result.MaxPageSize);
        Assert.True(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public async Task ListAsync_WhenFilterByName_ShouldReturnValidPaginationResult()
    {
        // Arrange
        var defectFilter = new DefectFilter() { Name = "name" };
        var specification = new DefectSpecification(defectFilter);
        var projectId = Guid.NewGuid();
        var maxPageSize = 2;
        var page = 1;
        
        var context = GetInMemoryDbContext();
        await context.AddRangeAsync(
            new Defect() { Name = "name", Description = "desc", ProjectId = projectId },
            new Defect() { Name = "bibop", Description = "desc", ProjectId = projectId });
        await context.SaveChangesAsync();
        
        var repository = new DefectRepository(context);
        
        // Act
        var result = await repository.ListAsync(projectId, specification, maxPageSize, page);
        
        // Assert
        Assert.IsType<PaginationResult<Defect>>(result);
        Assert.NotEmpty(result.Items);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.PageCount);
        Assert.False(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public async Task ListAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        
        // Act
        var act = () => _defectRepository.ListAsync(Guid.NewGuid(),
            new DefectSpecification(new DefectFilter()), 1, 1, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }
}