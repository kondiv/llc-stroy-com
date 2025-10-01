using LLCStroyCom.Api.Controllers;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LLCStroyCom.Tests.Controllers;

public class DefectControllerTests
{
    private readonly Mock<IDefectService> _defectServiceMock;
    private readonly DefectController _defectController;
    
    public DefectControllerTests()
    {
        _defectServiceMock = new Mock<IDefectService>();
        _defectController = new DefectController(_defectServiceMock.Object);
    }

    private static DefectDto GetDefectDto()
    {
        return new DefectDto("Дефект", "Дефект небольшой", Status.InProgress,
            new ProjectDto(Guid.NewGuid(), "name", "city", Guid.NewGuid(), Status.InProgress, DateTimeOffset.UtcNow), null);
    }
    
    [Fact]
    public async Task GetAsync_WhenDefectFound_ShouldReturnDefectDto()
    {
        // Arrange
        var defectId = Guid.NewGuid();
        var defect = GetDefectDto();
        _defectServiceMock
            .Setup(s => s.GetAsync(defectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(defect);
        
        // Act
        var result = await _defectController.GetAsync(defectId);
        
        // Assert
        Assert.IsType<ActionResult<DefectDto>>(result);
        Assert.NotNull(result.Result);

        var actualResult = result.Result as OkObjectResult;
        Assert.NotNull(actualResult);
        Assert.Equal(200, actualResult.StatusCode);
        
        var value = actualResult.Value as DefectDto;
        Assert.NotNull(value);
        Assert.Equal(defect, value);
    }

    [Fact]
    public async Task GetAsync_WhenDefectNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var defectId = Guid.NewGuid();
        _defectServiceMock
            .Setup(s => s.GetAsync(defectId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(CouldNotFindDefect.WithId(defectId));
        
        // Act
        var result = await _defectController.GetAsync(defectId);
        
        // Assert
        Assert.IsType<ActionResult<DefectDto>>(result);
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}