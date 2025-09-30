using LLCStroyCom.Api.Controllers;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace LLCStroyCom.Tests.Controllers;

public class CompanyControllerTests
{
    private readonly Mock<ICompanyService> _companyServiceMock;
    private readonly Mock<IProjectService> _projectServiceMock;
    private readonly CompanyController _companyController;
    
    public CompanyControllerTests()
    {
        _companyServiceMock = new Mock<ICompanyService>();
        _projectServiceMock = new Mock<IProjectService>();
        var loggerMock = new Mock<ILogger<CompanyController>>();
        _companyController = new CompanyController(_companyServiceMock.Object, _projectServiceMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task GetAsync_WhenCompanyFound_ShouldReturnOkObjectResult()
    {
        // Arrange
        _companyServiceMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CompanyDto(Guid.NewGuid(), "company_name"));
        
        // Act
        var result = await _companyController.GetAsync(Guid.NewGuid());
        
        // Assert
        Assert.IsType<ActionResult<CompanyDto>>(result);
        Assert.NotNull(result.Result);
        
        var actualResult = result.Result as OkObjectResult;
        Assert.NotNull(actualResult);
        Assert.NotNull(actualResult.Value);

        var value = actualResult.Value as CompanyDto;
        Assert.NotNull(value);
    }

    [Fact]
    public async Task GetAsync_WhenCompanyNotFound_ShouldReturnNotFoundResult()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        _companyServiceMock
            .Setup(x => x.GetAsync(companyId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(CouldNotFindCompany.WithId(companyId));
        
        // Act
        var result = await _companyController.GetAsync(companyId);
        
        // Assert
        Assert.IsType<ActionResult<CompanyDto>>(result);
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateAsync_WhenCompanyCreated_ShouldReturnCreatedAtRouteResult()
    {
        // Arrange
        var request = new CompanyCreateRequest("name");
        var companyDto = new CompanyDto(Guid.NewGuid(), request.Name);
        _companyServiceMock
            .Setup(x => x.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(companyDto);
        
        // Act
        var result = await _companyController.CreateAsync(request);
        
        // Assert
        var actualResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        Assert.NotNull(actualResult);
        Assert.Equal(201, actualResult.StatusCode);
        Assert.Equal(companyDto, actualResult.Value);
    }

    [Fact]
    public async Task CreateAsync_WhenCompanyAlreadyExists_ShouldReturnConflictObjectResult()
    {
        // Arrange
        var request = new CompanyCreateRequest("name");

        _companyServiceMock
            .Setup(x => x.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateConcurrencyException());
        
        // Act
        var result = await _companyController.CreateAsync(request);
        
        // Assert
        var actualResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.NotNull(actualResult);
        Assert.Equal(409, actualResult.StatusCode);
    }

    [Fact]
    public async Task DeleteAsync_WhenDeletedSuccessfully_ShouldReturnNoContentResult()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        
        // Act
        var result = await _companyController.DeleteAsync(companyId);
        
        // Assert
        var actualResult = Assert.IsType<NoContentResult>(result);
        Assert.NotNull(actualResult);
        Assert.Equal(204, actualResult.StatusCode);
        _companyServiceMock.Verify(x => x.DeleteAsync(companyId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenCompanyNotFound_ShouldReturnNotFoundObjectResult()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        
        _companyServiceMock
            .Setup(x => x.DeleteAsync(companyId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(CouldNotFindCompany.WithId(companyId));
        
        // Act
        var result = await _companyController.DeleteAsync(companyId);
        
        // Assert
        var actualResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(actualResult);
        Assert.Equal(404, actualResult.StatusCode);
    }

    [Fact]
    public async Task PatchAsync_WhenUpdatedSuccessfully_ShouldReturnNoContentResult()
    {
        // Arrange
        var jsonPatchDocument = new JsonPatchDocument<CompanyPatchDto>();
        jsonPatchDocument.Replace(x => x.Name, "new_name");
        
        // Act
        var result = await _companyController.PatchAsync(Guid.NewGuid(), jsonPatchDocument);
        
        // Assert
        var actualResult = Assert.IsType<NoContentResult>(result);
        Assert.NotNull(actualResult);
        Assert.Equal(204, actualResult.StatusCode);
    }

    [Fact]
    public async Task PatchAsync_WhenCompanyNotFound_ShouldReturnNotFoundResult()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var jsonPatchDocument = new JsonPatchDocument<CompanyPatchDto>();

        _companyServiceMock
            .Setup(x => x.UpdateAsync(companyId, jsonPatchDocument, It.IsAny<CancellationToken>()))
            .ThrowsAsync(CouldNotFindCompany.WithId(companyId));
        
        // Act
        var result = await _companyController.PatchAsync(companyId, jsonPatchDocument);
        
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}