using LLCStroyCom.Api.Controllers;
using LLCStroyCom.Api.Requests.Projects;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Models.Filters.Project;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.ResultPattern;
using LLCStroyCom.Domain.ResultPattern.Errors;
using LLCStroyCom.Domain.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LLCStroyCom.Tests.Controllers;

public class CompanyProjectsControllerTests
{
    private readonly Mock<IProjectService> _projectServiceMock;
    private readonly CompanyProjectsController _controller;

    public CompanyProjectsControllerTests()
    {
        _projectServiceMock = new Mock<IProjectService>();
        var logger = new Mock<ILogger<CompanyProjectsController>>();
        _controller = new CompanyProjectsController(_projectServiceMock.Object, logger.Object);
    }
    
    [Fact]
    public async Task ListAsync_WhenSomeProjectsLeft_ShouldReturnOkWithProjectsCollectionAndPageToken()
    {
        // Arrange
        _projectServiceMock
            .Setup(s => s.ListAsync(null, It.IsAny<ProjectFilter>(), It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedProjectListResponse()
            {
                PageToken = "encoded-page-token",
                Projects = new List<ProjectDto>()
                {
                    new ProjectDto(Guid.NewGuid(), "name", "city", Guid.NewGuid(), Status.Completed, DateTimeOffset.UtcNow)
                }
            });

        var query = new ProjectsQuery();
        
        // Act
        var result = await _controller.ListAsync(query);
        
        // Assert
        Assert.IsType<ActionResult<PaginatedProjectListResponse>>(result);
        Assert.NotNull(result.Result);

        var actualResult = result.Result as OkObjectResult;
        Assert.NotNull(actualResult);
        Assert.Equal(200, actualResult.StatusCode);
        
        Assert.IsType<PaginatedProjectListResponse>(actualResult.Value);
        
        var value = actualResult.Value as PaginatedProjectListResponse;
        Assert.NotNull(value);
        Assert.NotNull(value.PageToken);
        Assert.NotEmpty(value.Projects);
    }

    [Fact]
    public async Task ListAsync_WhenNoProjectsLeft_ShouldReturnProjectsAndNullPageToken()
    {
        // Arrange
        _projectServiceMock
            .Setup(s => s.ListAsync(It.IsAny<string>(), It.IsAny<ProjectFilter>(), It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedProjectListResponse()
            {
                PageToken = null,
                Projects = new List<ProjectDto>()
                {
                    new ProjectDto(Guid.NewGuid(),"name", "city", Guid.NewGuid(), Status.Completed, DateTimeOffset.UtcNow)
                }
            });
        
        var query = new ProjectsQuery();
        
        // Act
        var result = await _controller.ListAsync(query);
        
        // Assert
        Assert.IsType<ActionResult<PaginatedProjectListResponse>>(result);
        Assert.NotNull(result.Result);
        
        var actualResult = result.Result as OkObjectResult;
        Assert.NotNull(actualResult);
        Assert.Equal(200, actualResult.StatusCode);
        
        var value = actualResult.Value as PaginatedProjectListResponse;
        Assert.NotNull(value);
        Assert.Null(value.PageToken);
        Assert.NotEmpty(value.Projects);
    }

    [Fact]
    public async Task ListAsync_WhenEncodingExceptionWasThrown_ShouldReturnBadRequest()
    {
        // Arrange
        _projectServiceMock
            .Setup(s => s.ListAsync(It.IsAny<string>(), It.IsAny<ProjectFilter>(), It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(PageTokenEncodingException.ForToken("sd"));
        
        var query = new ProjectsQuery();
        
        // Act
        var result = await _controller.ListAsync(query);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task ListAsync_WhenDecodingExceptionWasThrown_ShouldReturnBadRequest()
    {
        // Arrange
        _projectServiceMock
            .Setup(s => s.ListAsync(It.IsAny<string>(), It.IsAny<ProjectFilter>(), It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(PageTokenDecodingException.ForToken("sd"));
        
        var query = new ProjectsQuery();
        
        // Act
        var result = await _controller.ListAsync(query);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task ListAsync_WhenNoProjects_ShouldReturnEmptyListAndNullPageToken()
    {
        // Arrange
        _projectServiceMock
            .Setup(s => s.ListAsync(It.IsAny<string>(), It.IsAny<ProjectFilter>(), It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedProjectListResponse() { Projects = new List<ProjectDto>(), PageToken = null });
        
        var query = new ProjectsQuery();
        
        // Act
        var result = await _controller.ListAsync(query);
        
        // Assert
        Assert.IsType<ActionResult<PaginatedProjectListResponse>>(result);
        Assert.NotNull(result.Result);
        
        var actualResult = result.Result as OkObjectResult;
        Assert.NotNull(actualResult);
        Assert.Equal(200, actualResult.StatusCode);
        
        var value = actualResult.Value as PaginatedProjectListResponse;
        Assert.NotNull(value);
        Assert.Empty(value.Projects);
        Assert.Null(value.PageToken);
    }

    [Fact]
    public async Task GetAsync_WhenProjectFound_ShouldReturnOkObjectResult()
    {
        // Arrange
        var projectDto = new ProjectDto(Guid.NewGuid(), "name", "city", Guid.NewGuid(), Status.Completed, DateTimeOffset.UtcNow);
        
        _projectServiceMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectDto>.Success(projectDto));
        
        // Act
        var result = await _controller.GetProjectAsync(Guid.NewGuid(), Guid.NewGuid());
        
        // Assert
        var actualResult = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<ProjectDto>(actualResult.Value);
        Assert.NotNull(value);
    }

    [Fact]
    public async Task GetAsync_WhenProjectNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _projectServiceMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectDto>.Failure(new NotFoundError("")));
        
        // Act
        var result = await _controller.GetProjectAsync(Guid.NewGuid(), Guid.NewGuid());
        
        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateAsync_WhenProjectCreated_ShouldReturnCreatedAtRoute()
    {
        // Arrange
        var createRequest = new ProjectCreateRequest()
        {
            City = "city",
            Name = "name",
        };
        var companyId = Guid.NewGuid();
        var projectDto = new ProjectDto(Guid.NewGuid(), createRequest.Name, createRequest.City, companyId, Status.Completed, DateTimeOffset.UtcNow);

        _projectServiceMock
            .Setup(x => x.CreateAsync(companyId, createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectDto>.Success(projectDto));
        
        // Act
        var result = await _controller.CreateProjectAsync(companyId, createRequest);
        
        // Assert
        var actualResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        var value = Assert.IsType<ProjectDto>(actualResult.Value);
        Assert.Equal(companyId, value.CompanyId);
    }

    [Fact]
    public async Task CreateAsync_WhenProjectAlreadyExists_ShouldReturnConflict()
    {
        // Arrange
        var projectDto = new ProjectDto(Guid.NewGuid(), "name", "city", Guid.NewGuid(), Status.Completed, DateTimeOffset.UtcNow);
        var createRequest = new ProjectCreateRequest()
        {
            City = "city",
            Name = "name",
        };
        
        _projectServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<Guid>(), createRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectDto>.Failure(new AlreadyExistsError("")));
        
        // Act
        var result = await _controller.CreateProjectAsync(Guid.NewGuid(), createRequest);
        
        // Assert
        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateAsync_WhenCompanyNotFound_ShouldReturnResultFailure()
    {
        // Arrange
        _projectServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<Guid>(), It.IsAny<ProjectCreateRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectDto>.Failure(new NotFoundError("")));
        
        // Act
        var result = await _controller.CreateProjectAsync(Guid.NewGuid(), new ProjectCreateRequest());
        
        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateAsync_WhenUpdatedSuccessfully_ShouldReturnNoContent()
    {
        // Arrange
        var patchDocument = new JsonPatchDocument<ProjectPatchDto>();
        
        _projectServiceMock
            .Setup(x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), patchDocument, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        
        // Act
        var result = await _controller.UpdateProjectAsync(Guid.NewGuid(), Guid.NewGuid(), patchDocument);
        
        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenTryingToUpdateNotExistingProperty_ShouldThrowJsonPatchException()
    {
        // Arrange
        var patchDocument = new JsonPatchDocument<ProjectPatchDto>();
        
        _projectServiceMock
            .Setup(x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), patchDocument,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new JsonPatchException());
        
        // Act
        var act = () => _controller.UpdateProjectAsync(Guid.NewGuid(), Guid.NewGuid(), patchDocument);
        
        // Assert
        await Assert.ThrowsAsync<JsonPatchException>(act);
    }

    [Fact]
    public async Task UpdateAsync_WhenRaceCondition_ShouldReturnConflict()
    {
        // Arrange
        var patchDocument = new JsonPatchDocument<ProjectPatchDto>();

        _projectServiceMock
            .Setup(x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), patchDocument, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new DbUpdateConcurrencyError("")));
        
        // Act
        var result = await _controller.UpdateProjectAsync(Guid.NewGuid(), Guid.NewGuid(), patchDocument);
        
        // Assert
        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenProjectNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var patchDocument = new JsonPatchDocument<ProjectPatchDto>();
        
        _projectServiceMock
            .Setup(x => x.UpdateAsync(It.IsAny<Guid>(),It.IsAny<Guid>(), patchDocument, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new NotFoundError("")));
        
        // Act
        var result = await _controller.UpdateProjectAsync(Guid.NewGuid(), Guid.NewGuid(), patchDocument);
        
        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenProjectFound_ShouldReturnResultSuccess()
    {
        // Arrange
        _projectServiceMock
            .Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        
        // Act
        var result = await _controller.DeleteProjectAsync(Guid.NewGuid(), Guid.NewGuid());
        
        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenProjectNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _projectServiceMock
            .Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new NotFoundError("")));
        
        // Act
        var result = await _controller.DeleteProjectAsync(Guid.NewGuid(), Guid.NewGuid());
        
        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenProjectDoesNotBelongToCompany_ShouldReturnNotFound()
    {
        // Arrange
        _projectServiceMock
            .Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new NotFoundError("")));
        
        // Act
        var result = await _controller.DeleteProjectAsync(Guid.NewGuid(), Guid.NewGuid());
        
        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}