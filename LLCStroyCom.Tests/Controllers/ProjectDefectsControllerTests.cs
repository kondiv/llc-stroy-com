using LLCStroyCom.Api.Controllers;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Models;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.ResultPattern;
using LLCStroyCom.Domain.ResultPattern.Errors;
using LLCStroyCom.Domain.Services;
using LLCStroyCom.Domain.Specifications.Defects;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LLCStroyCom.Tests.Controllers;

public class ProjectDefectsControllerTests
{
    private readonly Mock<IDefectService> _defectServiceMock;
    private readonly ProjectDefectsController _controller;

    public ProjectDefectsControllerTests()
    {
        _defectServiceMock = new Mock<IDefectService>();
        var logger = new Mock<ILogger<ProjectDefectsController>>();
        _controller = new ProjectDefectsController(_defectServiceMock.Object, logger.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenCreated_ShouldReturnCreatedAtRoute()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new DefectCreateRequest()
        {
            Description = "Description",
            Name = "Name"
        };
        var defectDto = new DefectDto(Guid.NewGuid(), request.Name, request.Description, Status.New, new ProjectDto(projectId,
                "project-name", "city", Guid.NewGuid(), Status.InProgress, DateTimeOffset.UtcNow),
            null);
        
        _defectServiceMock
            .Setup(x => x.CreateAsync(projectId, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DefectDto>.Success(defectDto));
        
        // Act
        var result = await _controller.CreateAsync(projectId, request);
        
        // Assert
        var actualResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        Assert.Equal(defectDto, actualResult.Value);
    }

    [Fact]
    public async Task CreateAsync_WhenNotCreated_WhenAlreadyExists_ShouldReturnConflict()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new DefectCreateRequest();

        _defectServiceMock
            .Setup(x => x.CreateAsync(projectId, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DefectDto>.Failure(new AlreadyExistsError("")));
        
        // Act
        var result = await _controller.CreateAsync(projectId, request);
        
        // Assert
        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateAsync_WhenNotCreated_WhenProjectNotFound_ShouldReturnBadRequest()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new DefectCreateRequest();
        
        _defectServiceMock
            .Setup(x => x.CreateAsync(projectId, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DefectDto>.Failure(new DbUpdateError("")));
        
        // Act
        var result = await _controller.CreateAsync(projectId, request);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAsync_WhenFound_ShouldReturnOkObjectResultWithDto()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var defectId = Guid.NewGuid();
        
        var defectDto = new DefectDto(Guid.NewGuid(), "name", "desc", Status.New, new ProjectDto(projectId,
                "project-name", "city", Guid.NewGuid(), Status.InProgress, DateTimeOffset.UtcNow),
            null);
        
        _defectServiceMock
            .Setup(x => x.GetAsync(projectId, defectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DefectDto>.Success(defectDto));
        
        // Act
        var result = await _controller.GetAsync(projectId, defectId);
        
        // Assert
        var actualResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(defectDto, actualResult.Value);
    }

    [Fact]
    public async Task GetAsync_WhenNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var defectId = Guid.NewGuid();
        
        _defectServiceMock
            .Setup(x => x.GetAsync(projectId, defectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<DefectDto>.Failure(new NotFoundError("")));
        
        // Act
        var result = await _controller.GetAsync(projectId, defectId);
        
        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateAsync_WhenUpdated_ShouldReturnUpdatedAtRoute()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var defectId = Guid.NewGuid();
        var patchDocument = new JsonPatchDocument<DefectPatchDto>();

        _defectServiceMock
            .Setup(x => x.UpdateAsync(projectId, defectId, patchDocument, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        
        // Act
        var result = await _controller.UpdateAsync(projectId, defectId, patchDocument);
        
        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotUpdated_WhenProjectNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var defectId = Guid.NewGuid();
        var patchDocument = new JsonPatchDocument<DefectPatchDto>();
        
        _defectServiceMock
            .Setup(x => x.UpdateAsync(projectId, defectId, patchDocument, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new NotFoundError("")));
        
        // Act
        var result = await _controller.UpdateAsync(projectId, defectId, patchDocument);
        
        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotUpdated_WhenRaceCondition_ShouldReturnConflictObjectResult()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var defectId = Guid.NewGuid();
        var patchDocument = new JsonPatchDocument<DefectPatchDto>();

        _defectServiceMock
            .Setup(x => x.UpdateAsync(projectId, defectId, patchDocument, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new DbUpdateConcurrencyError("")));
        
        // Act
        var result = await _controller.UpdateAsync(projectId, defectId, patchDocument);
        
        // Assert
        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotUpdated_WhenSameDefectExists_ShouldReturnConflictObjectResult()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var defectId = Guid.NewGuid();
        var patchDocument = new JsonPatchDocument<DefectPatchDto>();
        
        _defectServiceMock
            .Setup(x => x.UpdateAsync(projectId, defectId, patchDocument, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new AlreadyExistsError("")));
        
        // Act
        var result = await _controller.UpdateAsync(projectId, defectId, patchDocument);
        
        // Assert
        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenDeleted_ShouldReturnNoContent()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var defectId = Guid.NewGuid();

        _defectServiceMock
            .Setup(x => x.DeleteAsync(projectId, defectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        
        // Act
        var result = await _controller.DeleteAsync(projectId, defectId);
        
        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotDeleted_WhenNotFound_ShouldReturnNotFoundObjectResult()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var defectId = Guid.NewGuid();
        
        _defectServiceMock
            .Setup(x => x.DeleteAsync(projectId, defectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new NotFoundError("")));
        
        // Act
        var result = await _controller.DeleteAsync(projectId, defectId);
        
        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ListAsync_WhenPaginationResultEmpty_ShouldReturnOkObjectResult()
    {
        // Arrange
        var emptyPaginationResult = new PaginationResult<DefectDto>([], 1, 1, 0, 0);
        _defectServiceMock
            .Setup(x => x.ListAsync(It.IsAny<Guid>(), It.IsAny<DefectSpecification>(),
                1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyPaginationResult);
        
        // Act
        var result = await _controller.ListAsync(Guid.NewGuid(), new DefectFilter(), 1, 1);
        
        // Assert
        var actualResult = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<PaginationResult<DefectDto>>(actualResult.Value);
        Assert.Empty(value.Items);
    }
    
    [Fact]
    public async Task ListAsync_WhenPaginationResultNotEmpty_ShouldReturnOkObjectResult()
    {
        // Arrange
        var paginationResult = new PaginationResult<DefectDto>([new DefectDto(Guid.NewGuid(), "name", "desc",
            Status.New, new ProjectDto(Guid.NewGuid(), "name", "city", Guid.NewGuid(),
                Status.InProgress, DateTimeOffset.UtcNow), null)], 1, 1, 0, 0);
        _defectServiceMock
            .Setup(x => x.ListAsync(It.IsAny<Guid>(), It.IsAny<DefectSpecification>(),
                1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginationResult);
        
        // Act
        var result = await _controller.ListAsync(Guid.NewGuid(), new DefectFilter(), 1, 1);
        
        // Assert
        var actualResult = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<PaginationResult<DefectDto>>(actualResult.Value);
        Assert.NotEmpty(value.Items);
    }

    [Fact]
    public async Task ListAsync_WhenArgumentOutOfRange_ShouldReturnBadRequestObjectResult()
    {
        // Arrange
        _defectServiceMock
            .Setup(x => x.ListAsync(It.IsAny<Guid>(), It.IsAny<DefectSpecification>(),
                -23, 1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentOutOfRangeException());
        
        // Act
        var result = await _controller.ListAsync(Guid.NewGuid(), new DefectFilter(), -23, 1);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}