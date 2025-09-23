using AutoMapper;
using LLCStroyCom.Application.MapperProfiles;
using LLCStroyCom.Application.Services;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace LLCStroyCom.Tests.Services;

public class DefectServiceTests
{
    private readonly IDefectService _defectService;
    private readonly Mock<IDefectRepository> _defectRepositoryMock;

    public DefectServiceTests()
    {
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
    public async Task GetAsync_WhenDefectFound_ShouldReturnDefectDto()
    {
        // Arrange
        var defect = new Defect()
        {
            Id = Guid.NewGuid(),
            Name = "Defect",
            Description = "Defect",
            ChiefEngineerId = null,
            Project = new Project(),
            Status = Status.New
        };

        _defectRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(defect);
        
        // Act
        var result = await _defectService.GetAsync(defect.Id);
        
        // Assert
        Assert.IsType<DefectDto>(result);
        Assert.Equal(defect.Name, result.Name);
        Assert.Equal(defect.Description, result.Description);
        Assert.Null(result.ChiefEngineer);
        Assert.IsType<ProjectDto>(result.Project);
        Assert.NotNull(result.Project);
    }

    [Fact]
    public async Task GetAsync_WhenNotFound_ShouldThrowCouldNotFindDefect()
    {
        // Arrange
        var defectId = Guid.NewGuid();
        
        _defectRepositoryMock
            .Setup(r => r.GetAsync(defectId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(CouldNotFindDefect.WithId(defectId));
        
        // Act
        var act = () => _defectService.GetAsync(defectId);
        
        // Assert
        await Assert.ThrowsAsync<CouldNotFindDefect>(act);
    }

    [Fact]
    public async Task GetAsync_WhenOperationCanceled_ShouldThrowOperationCanceled()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);

        _defectRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());
        
        // Act
        var act = () =>  _defectService.GetAsync(Guid.NewGuid(), cancellationToken);
        
        // Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(act);
    }
}