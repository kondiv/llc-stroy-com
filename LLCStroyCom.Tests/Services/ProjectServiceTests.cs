using AutoMapper;
using LLCStroyCom.Application.MapperProfiles;
using LLCStroyCom.Application.Services;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Models.Filters.Project;
using LLCStroyCom.Domain.Models.PageTokens;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Services;
using LLCStroyCom.Domain.Specifications.Projects;
using Microsoft.Extensions.Logging;
using Moq;

namespace LLCStroyCom.Tests.Services;

public class ProjectServiceTests
{
    private readonly IProjectService _projectService;
    private readonly Mock<IPageTokenService> _pageTokenServiceMock;
    private readonly Mock<IProjectRepository> _projectRepositoryMock;

    public ProjectServiceTests()
    {
        _pageTokenServiceMock = new Mock<IPageTokenService>();
        _projectRepositoryMock = new Mock<IProjectRepository>();
        var loggerFactory = new LoggerFactory();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserProfile>();
            cfg.AddProfile<ProjectProfile>();
            cfg.AddProfile<CompanyProfile>();
        }, loggerFactory);
        _projectService = new ProjectService(_pageTokenServiceMock.Object, _projectRepositoryMock.Object, mapperConfig.CreateMapper());
    }

    [Fact]
    public async Task GetAsync_WhenProjectExist_ShouldReturnProjectDto()
    {
        // Arrange
        var project = new Project()
        {
            Id = Guid.NewGuid(),
            Name = "Project1",
            Status = Status.Completed,
            CompanyId = Guid.NewGuid(),
            City = "Москва"
        };

        _projectRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        
        // Act
        var result = await _projectService.GetAsync(project.Id, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.IsType<ProjectDto>(result);
        Assert.Equal(project.Name, result.Name);
        Assert.Equal(project.Status, result.Status);
        Assert.Equal(project.CompanyId, result.CompanyId);
    }

    [Fact]
    public async Task GetAsync_WhenProjectDoesNotExist_ShouldThrowCouldNotFindProject()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(CouldNotFindProject.WithId(Guid.NewGuid()));
        
        // Act
        var act = () => _projectService.GetAsync(Guid.NewGuid());
        
        // Assert
        await Assert.ThrowsAsync<CouldNotFindProject>(act);
    }

    [Fact]
    public async Task GetAsync_WhenOperationIsCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());
        
        // Act
        var act = () => _projectService.GetAsync(Guid.NewGuid(), new CancellationToken(true));
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task ListAsync_WhenEmptyCollectionReturned_ShouldReturnEmptyCollectionAndNullPageToken()
    {
        // Arrange
        var projectFilter = new ProjectFilter();
        ProjectPageToken? pageToken = null;
        int maxPageSize = 10;
        var specification = new ProjectSpecification(projectFilter, pageToken, maxPageSize);

        _projectRepositoryMock
            .Setup(r => r.ListAsync(specification, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        string? plainPageToken = null;
        
        // Act
        var result = await _projectService.ListAsync(plainPageToken, projectFilter, maxPageSize);
        
        // Assert
        Assert.IsType<PaginatedProjectListResponse>(result);
        Assert.NotNull(result);
        Assert.Empty(result.Projects);
        Assert.Null(result.PageToken);
    }

    [Fact]
    public async Task ListAsync_WhenCollectionReturnedButNoElementsLeft_ShouldReturnProjectsAndNullPageToken()
    {
        // Arrange
        var projectFilter = new ProjectFilter();
        var maxPageSize = 1;

        _projectRepositoryMock
            .Setup(r => r.ListAsync(It.IsAny<ProjectSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project>() { new Project() { Name = "Project1" } });
        
        // Act
        var result = await _projectService.ListAsync(null, projectFilter, maxPageSize);
        
        // Assert
        Assert.IsType<PaginatedProjectListResponse>(result);
        Assert.NotEmpty(result.Projects);
        Assert.Null(result.PageToken);
    }

    [Fact]
    public async Task ListAsync_WhenCollectionReturnedAndElementsLeft_ShouldReturnProjectsAndPageToken()
    {
        // Arrange
        var projectFilter = new ProjectFilter();
        var maxPageSize = 1;

        _projectRepositoryMock
            .Setup(r => r.ListAsync(It.IsAny<ProjectSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project>() { new Project() { Name = "Project1" }, new Project() { Name = "Project2" } });
        
        _pageTokenServiceMock
            .Setup(s => s.Encode(It.IsAny<ProjectPageToken>()))
            .Returns("encoded-page-token");
        
        // Act
        var result = await _projectService.ListAsync(null, projectFilter, maxPageSize);
        
        // Assert
        Assert.IsType<PaginatedProjectListResponse>(result);
        Assert.NotEmpty(result.Projects);
        Assert.NotNull(result.PageToken);
        Assert.Equal("encoded-page-token", result.PageToken);
        
        _pageTokenServiceMock.Verify(s => s.Encode(It.IsAny<ProjectPageToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ListAsync_WhenPageTokenNotNull_ShouldReturnProjectsAndNewPageToken()
    {
        // Arrange
        var projectFilter = new ProjectFilter();
        var pageToken = new ProjectPageToken()
        {
            OrderBy = "name"
        };
        var maxPageSize = 1;

        _projectRepositoryMock
            .Setup(r => r.ListAsync(It.IsAny<ProjectSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project>() { new Project() { Name = "Project1" }, new Project() { Name = "Project2" } });
        
        _pageTokenServiceMock
            .Setup(s => s.Encode(It.IsAny<ProjectPageToken>()))
            .Returns("new-encoded-page-token");

        _pageTokenServiceMock
            .Setup(s => s.Decode<ProjectPageToken>("old-encoded-page-token"))
            .Returns(pageToken);
        
        // Act
        var result = await _projectService.ListAsync("old-encoded-page-token",  projectFilter, maxPageSize);
        
        // Assert
        Assert.IsType<PaginatedProjectListResponse>(result);
        Assert.NotEmpty(result.Projects);
        Assert.NotNull(result.PageToken);
        Assert.Equal("new-encoded-page-token", result.PageToken);
        
        _pageTokenServiceMock.Verify(s => s.Encode(It.IsAny<ProjectPageToken>()), Times.Once);
        _pageTokenServiceMock.Verify(s => s.Decode<ProjectPageToken>("old-encoded-page-token"), Times.Once);
    }

    [Fact]
    public async Task ListAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(true);
        var projectFilter = new ProjectFilter();
        var maxPageSize = 1;
        
        _projectRepositoryMock
            .Setup(r => r.ListAsync(It.IsAny<ProjectSpecification>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());
        
        // Act
        var act = () => _projectService.ListAsync(null, projectFilter, maxPageSize, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }
    
    [Fact]
    public async Task ListAsync_WhenDecodingException_ShouldThrowEncodingException()
    {
        // Arrange
        var projectFilter = new ProjectFilter();
        var maxPageSize = 1;
        var invalidPageToken = "wrong-format-or-something-is-wrong";
        
        _pageTokenServiceMock
            .Setup(s => s.Decode<ProjectPageToken>(invalidPageToken))
            .Throws(PageTokenDecodingException.ForToken(typeof(ProjectPageToken).Name));
        
        // Act
        var act = () => _projectService.ListAsync(invalidPageToken, projectFilter, maxPageSize);
        
        // Assert
        await Assert.ThrowsAsync<PageTokenDecodingException>(act);
    }
    
    [Fact]
    public async Task ListAsync_WhenEncodingException_ShouldThrowEncodingException()
    {
        // Arrange
        var projectFilter = new ProjectFilter();
        var maxPageSize = 1;

        _projectRepositoryMock
            .Setup(r => r.ListAsync(It.IsAny<ProjectSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project>() { new Project(), new Project() });
        
        _pageTokenServiceMock
            .Setup(s => s.Encode(It.IsAny<ProjectPageToken>()))
            .Throws(PageTokenEncodingException.ForToken(typeof(ProjectPageToken).Name));
        
        // Act
        var act = () => _projectService.ListAsync(null, projectFilter, maxPageSize);
        
        // Assert
        await Assert.ThrowsAsync<PageTokenEncodingException>(act);
    }
}