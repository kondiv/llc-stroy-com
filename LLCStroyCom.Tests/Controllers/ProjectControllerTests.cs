using LLCStroyCom.Api.Controllers;
using LLCStroyCom.Api.Requests.Projects;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Models.Filters.Project;
using LLCStroyCom.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LLCStroyCom.Tests.Controllers;

public class ProjectControllerTests
{
    private readonly Mock<IProjectService> _projectServiceMock;
    private readonly ProjectController _controller;
        
    public ProjectControllerTests()
    {
        _projectServiceMock = new Mock<IProjectService>();
        _controller = new ProjectController(_projectServiceMock.Object);
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
                    new ProjectDto("name", "city", Guid.NewGuid(), Status.Completed, DateTimeOffset.UtcNow)
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
                    new ProjectDto("name", "city", Guid.NewGuid(), Status.Completed, DateTimeOffset.UtcNow)
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
    public async Task GetAsync_WhenProjectExists_ShouldReturnOk()
    {
        // Arrange
        _projectServiceMock
            .Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProjectDto("name", "city", Guid.NewGuid(), Status.Completed, DateTimeOffset.UtcNow));
        
        // Act
        var result = await _controller.GetAsync(Guid.NewGuid());
        
        // Assert
        Assert.IsType<ActionResult<ProjectDto>>(result);
        Assert.NotNull(result.Result);
        
        var actualResult = result.Result as OkObjectResult;
        Assert.NotNull(actualResult);
        Assert.Equal(200, actualResult.StatusCode);
        
        var value = actualResult.Value as ProjectDto;
        Assert.NotNull(value);
        Assert.Equal("name", value.Name);
        Assert.Equal("city", value.City);
    }

    [Fact]
    public async Task GetAsync_WhenProjectDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        _projectServiceMock
            .Setup(s => s.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(CouldNotFindProject.WithId(Guid.NewGuid()));
        
        // Act
        var result = await _controller.GetAsync(Guid.NewGuid());
        
        // Assert
        Assert.IsType<ActionResult<ProjectDto>>(result);
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}