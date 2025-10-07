using AutoMapper;
using LLCStroyCom.Application.MapperProfiles;
using LLCStroyCom.Application.Services;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.ResultPattern;
using LLCStroyCom.Domain.ResultPattern.Errors;
using LLCStroyCom.Domain.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Moq;

namespace LLCStroyCom.Tests.Services;

public class DefectServiceTests
{
    private readonly Defect _validDefect;
    private readonly IDefectService _defectService;
    private readonly Mock<IDefectRepository> _defectRepositoryMock;

    public DefectServiceTests()
    {
        _validDefect = new Defect()
        {
            Name = "name",
            Description = "description",
            Status = Status.New,
            Project = new Project()
            {
              Name = "project-name",
              City = "city-name",
              CompanyId = Guid.NewGuid(),
              CreatedAt = DateTimeOffset.UtcNow,
              Id = Guid.NewGuid(),
              Status = Status.InProgress
            },
            ChiefEngineerId = null,
        };
        
        _defectRepositoryMock = new Mock<IDefectRepository>();
        
        var loggerFactory = new LoggerFactory();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserProfile>();
            cfg.AddProfile<DefectProfile>();
            cfg.AddProfile<CompanyProfile>();
            cfg.AddProfile<ProjectProfile>();
        }, loggerFactory);
        _defectService = new DefectService(_defectRepositoryMock.Object, mapperConfig.CreateMapper());
    }

    [Fact]
    public async Task GetAsync_WhenDefectFound_ShouldReturnResultSuccessWithDefectDto()
    {
        // Arrange
        _defectRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Defect>.Success(_validDefect));
        
        // Act
        var result = await _defectService.GetAsync(_validDefect.ProjectId, _validDefect.Id);
        
        // Assert
        Assert.True(result.Succeeded);
        Assert.IsType<DefectDto>(result.Value);
    }

    [Fact]
    public async Task GetAsync_WhenDefectNotFound_ShouldReturnResulFailureWithNotFoundError()
    {
        // Arrange
        _defectRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Defect>.Failure(new NotFoundError("")));
        
        // Act
        var result = await _defectService.GetAsync(_validDefect.ProjectId, _validDefect.Id);
        
        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task GetAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        _defectRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());
        
        // Act
        var act = () => _defectService.GetAsync(_validDefect.ProjectId, _validDefect.Id);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenCreatedSuccessfully_ShouldReturnResultSuccessWithDefectDto()
    {
        // Arrange
        _defectRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Defect>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Defect>.Success(_validDefect));

        var request = new DefectCreateRequest()
        {
            Name = "name",
            Description = "description",
        };
        
        // Act
        var result = await _defectService.CreateAsync(_validDefect.ProjectId, request);
        
        // Assert
        Assert.True(result.Succeeded);
        Assert.IsType<DefectDto>(result.Value);
    }

    [Fact]
    public async Task CreateAsync_WhenDefectAlreadyExists_ShouldReturnResultFailureWithAlreadyExistsError()
    {
        // Arrange
        _defectRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Defect>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Defect>.Failure(new AlreadyExistsError("")));

        var request = new DefectCreateRequest()
        {
            Name = "name",
            Description = "description",
        };
        
        // Act
        var result = await _defectService.CreateAsync(_validDefect.ProjectId, request);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<AlreadyExistsError>(result.Error);
    }

    [Fact]
    public async Task CreateAsync_WhenProjectNotFound_ShouldReturnResultFailureWithNotFoundError()
    {
        // Arrange
        _defectRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Defect>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Defect>.Failure(new NotFoundError("")));

        var request = new DefectCreateRequest()
        {
            Name = "name",
            Description = "description",
        };
        
        // Act
        var result = await _defectService.CreateAsync(_validDefect.ProjectId, request);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<NotFoundError>(result.Error);
    }

    [Fact]
    public async Task CreateAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        
        _defectRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Defect>(), cancellationToken))
            .ThrowsAsync(new OperationCanceledException(cancellationToken));

        var request = new DefectCreateRequest()
        {
            Name = "name",
            Description = "description",
        };
        
        // Act
        var act = () => _defectService.CreateAsync(_validDefect.ProjectId, request, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task UpdateAsync_WhenUpdatedSuccessfully_ShouldReturnResultSuccess()
    {
        // Arrange
        _defectRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Defect>.Success(_validDefect));

        _defectRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Defect>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var patchDocument = new JsonPatchDocument<DefectPatchDto>();
        
        // Act
        var result = await _defectService.UpdateAsync(_validDefect.ProjectId, _validDefect.Id, patchDocument);
        
        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task UpdateAsync_WhenDefectNotFound_ShouldReturnResultFailureWithNotFoundError()
    {
        // Arrange
        _defectRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Defect>.Failure(new NotFoundError("")));
        
        var patchDocument = new JsonPatchDocument<DefectPatchDto>();
        
        // Act
        var result = await _defectService.UpdateAsync(_validDefect.ProjectId, _validDefect.Id, patchDocument);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<NotFoundError>(result.Error);
    }

    [Fact]
    public async Task UpdateAsync_WhenDefectAlreadyExists_ShouldReturnResultFailureWithAlreadyExistsError()
    {
        // Arrange
        _defectRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Defect>.Success(_validDefect));
        
        _defectRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Defect>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new AlreadyExistsError("")));
        
        var patchDocument = new JsonPatchDocument<DefectPatchDto>();
        
        // Act
        var result = await _defectService.UpdateAsync(_validDefect.ProjectId, _validDefect.Id, patchDocument);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<AlreadyExistsError>(result.Error);
    }

    [Fact]
    public async Task UpdateAsync_WhenRaceCondition_ShouldReturnResultFailureWithConflictError()
    {
        // Arrange
        _defectRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Defect>.Success(_validDefect));
        
        _defectRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Defect>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new DbUpdateConcurrencyError("")));
        
        var patchDocument = new JsonPatchDocument<DefectPatchDto>();
        
        // Act
        var result = await _defectService.UpdateAsync(_validDefect.ProjectId, _validDefect.Id, patchDocument);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<DbUpdateConcurrencyError>(result.Error);
    }

    [Fact]
    public async Task UpdateAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        
        _defectRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), cancellationToken))
            .ThrowsAsync(new OperationCanceledException(cancellationToken));
        
        var patchDocument = new JsonPatchDocument<DefectPatchDto>();
        
        // Act
        var act = () => _defectService.UpdateAsync(_validDefect.ProjectId, _validDefect.Id, patchDocument,
            cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task DeleteAsync_WhenDeletedSuccessfully_ShouldReturnResultSuccess()
    {
        // Arrange
        _defectRepositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        
        // Act
        var result = await _defectService.DeleteAsync(_validDefect.ProjectId, _validDefect.Id);
        
        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task DeleteAsync_WhenDefectNotFound_ShouldReturnResultFailureWithNotFoundError()
    {
        // Arrange
        _defectRepositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new NotFoundError("")));

        // Act
        var result = await _defectService.DeleteAsync(_validDefect.ProjectId, _validDefect.Id);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<NotFoundError>(result.Error);
    }

    [Fact]
    public async Task DeleteAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        
        _defectRepositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException(cancellationToken));
        
        // Act
        var act = () => _defectService.DeleteAsync(_validDefect.ProjectId, _validDefect.Id, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }
}