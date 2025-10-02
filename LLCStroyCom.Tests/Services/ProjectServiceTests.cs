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
using LLCStroyCom.Domain.ResultPattern;
using LLCStroyCom.Domain.ResultPattern.Errors;
using LLCStroyCom.Domain.Services;
using LLCStroyCom.Domain.Specifications.Projects;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Moq;

namespace LLCStroyCom.Tests.Services;

public class ProjectServiceTests
{
    private readonly IProjectService _projectService;
    private readonly Mock<IPageTokenService> _pageTokenServiceMock;
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;

    public ProjectServiceTests()
    {
        _pageTokenServiceMock = new Mock<IPageTokenService>();
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        var loggerFactory = new LoggerFactory();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserProfile>();
            cfg.AddProfile<ProjectProfile>();
            cfg.AddProfile<CompanyProfile>();
        }, loggerFactory);
        
        _projectService = new ProjectService(_pageTokenServiceMock.Object, _projectRepositoryMock.Object, _companyRepositoryMock.Object,
            mapperConfig.CreateMapper());
    }

    [Fact]
    public async Task GetAsync_WhenProjectExist_ShouldReturnProjectDto()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var project = new Project()
        {
            Id = Guid.NewGuid(),
            Name = "Project1",
            Status = Status.Completed,
            City = "Москва",
            CompanyId = companyId,
        };

        _projectRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Project>.Success(project));
        
        // Act
        var result = await _projectService.GetAsync(companyId, project.Id, CancellationToken.None);
        
        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(project.Name, result.Value.Name);
    }

    [Fact]
    public async Task GetAsync_WhenProjectDoesNotExist_ShouldThrowCouldNotFindProject()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Project>.Failure(new NotFoundError("not found")));
        
        // Act
        var result = await _projectService.GetAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<NotFoundError>(result.Error);
    }

    [Fact]
    public async Task GetAsync_WhenOperationIsCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());
        
        // Act
        var act = () => _projectService.GetAsync(Guid.NewGuid(), Guid.NewGuid(), new CancellationToken(true));
        
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
            .Throws(PageTokenDecodingException.ForToken(nameof(ProjectPageToken)));
        
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
            .Throws(PageTokenEncodingException.ForToken(nameof(ProjectPageToken)));
        
        // Act
        var act = () => _projectService.ListAsync(null, projectFilter, maxPageSize);
        
        // Assert
        await Assert.ThrowsAsync<PageTokenEncodingException>(act);
    }

    [Fact]
    public async Task UpdateAsync_WhenUpdateSucceeded_ShouldReturnResultSuccess()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        
        _projectRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Project>.Success(new Project(){ CompanyId = companyId }));

        _projectRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var jsonPatchDocument = new JsonPatchDocument<ProjectPatchDto>();
        var projectId = Guid.NewGuid();
        
        // Act
        var result = await _projectService.UpdateAsync(companyId, projectId, jsonPatchDocument);
        
        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task UpdateAsync_WhenProjectNotFound_ShouldReturnFailedResult()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Project>.Failure(new NotFoundError("")));
        
        var jsonPatchDocument = new JsonPatchDocument<ProjectPatchDto>();
        
        // Act
        var result = await _projectService.UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), jsonPatchDocument);
        
        // Assert
        Assert.False(result.Succeeded);
        Assert.IsType<NotFoundError>(result.Error);
        _projectRepositoryMock
            .Verify(x => x.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenUpdateFailed_ShouldReturnFailedResult()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        
        _projectRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Project>.Success(new Project(){ CompanyId = companyId}));

        _projectRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new DbUpdateConcurrencyError("")));

        var jsonPatchDocument = new JsonPatchDocument<ProjectPatchDto>();
        
        // Act
        var result = await _projectService.UpdateAsync(companyId, Guid.NewGuid(), jsonPatchDocument);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<DbUpdateConcurrencyError>(result.Error);
    }

    [Fact]
    public async Task UpdateAsync_WhenProjectDoesNotBelongToCompany_ShouldReturnFailedResult()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Project>.Success(new Project()));
        
        var jsonPatchDocument = new JsonPatchDocument<ProjectPatchDto>();
        
        // Act
        var result = await _projectService.UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), jsonPatchDocument);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<NotFoundError>(result.Error);
    }

    [Fact]
    public async Task UpdateAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        var jsonPatchDocument = new JsonPatchDocument<ProjectPatchDto>();
        
        _projectRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TaskCanceledException());
        
        // Act
        var act = () => _projectService.UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), jsonPatchDocument, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(act);
    }

    [Fact]
    public async Task DeleteAsync_WhenDeleteSucceeded_ShouldReturnResultSuccess()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        _projectRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Project>.Success(new Project() { CompanyId = companyId }));
        
        _projectRepositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        
        // Act
        var result = await _projectService.DeleteAsync(companyId, Guid.NewGuid());
        
        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task DeleteAsync_WhenProjectDoesNotBelongToCompany_ShouldReturnFailedResult()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Project>.Success(new Project()));
        
        // Act
        var result = await _projectService.DeleteAsync(Guid.NewGuid(), Guid.NewGuid());
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<NotFoundError>(result.Error);
    }

    [Fact]
    public async Task DeleteAsync_WhenProjectNotFound_ShouldReturnFailedResult()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Project>.Failure(new NotFoundError("NotFoundError")));
        
        // Act
        var result = await _projectService.DeleteAsync(Guid.NewGuid(), Guid.NewGuid());
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<NotFoundError>(result.Error);
    }
}