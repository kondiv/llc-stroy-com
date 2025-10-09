using LLCStroyCom.Api.Controllers;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Models;
using LLCStroyCom.Domain.Services;
using LLCStroyCom.Domain.Specifications.Users;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace LLCStroyCom.Tests.Controllers;

public class CompanyEmployeesControllerTests
{
    private readonly Mock<ICompanyService> _companyServiceMock;
    private readonly CompanyEmployeesController _companyEmployeesController;

    public CompanyEmployeesControllerTests()
    {
        _companyServiceMock = new Mock<ICompanyService>();
        var loggerMock = new Mock<ILogger<CompanyEmployeesController>>();
        _companyEmployeesController = new CompanyEmployeesController(_companyServiceMock.Object, loggerMock.Object);
    }
    
    [Fact]
    public async Task AddEmployeeAsync_WhenAddedSuccessfully_ShouldReturnCreatedAtRouteResult()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();

        _companyServiceMock
            .Setup(x => x.AddEmployeeAsync(companyId, employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeId);
        
        // Act
        var result = await _companyEmployeesController.HireAsync(companyId, employeeId);
        
        // Assert
        var actualResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
        Assert.Equal(employeeId, actualResult.Value);
    }

    [Fact]
    public async Task AddEmployeeAsync_WhenEmployeeNotFound_ShouldReturnNotFoundObjectResult()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        
        _companyServiceMock
            .Setup(x => x.AddEmployeeAsync(companyId, employeeId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(CouldNotFindUser.WithId(employeeId));
        
        // Act
        var result = await _companyEmployeesController.HireAsync(companyId, employeeId);
        
        // Assert
        var actualResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.NotNull(actualResult.Value);
    }

    [Fact]
    public async Task AddEmployeeAsync_WhenCompanyNotFound_ShouldReturnNotFoundResult()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        
        _companyServiceMock
            .Setup(x => x.AddEmployeeAsync(companyId, employeeId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(CouldNotFindCompany.WithId(employeeId));
        
        // Act
        var result = await _companyEmployeesController.HireAsync(companyId, employeeId);
        
        // Assert
        var actualResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.NotNull(actualResult.Value);
    }

    [Fact]
    public async Task AddEmployeeAsync_WhenEmployeeAlreadyWorksInOtherCompany_ShouldReturnConflictObjectResult()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();

        _companyServiceMock
            .Setup(x => x.AddEmployeeAsync(companyId, employeeId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(AlreadyWorks.InCompany(Guid.NewGuid()));
        
        // Act
        var result = await _companyEmployeesController.HireAsync(companyId, employeeId);
        
        // Assert
        var actualResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.NotNull(actualResult.Value);
    }

    [Fact]
    public async Task AddEmployeeAsync_WhenRaceCondition_ShouldReturnConflictObjectResult()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        
        _companyServiceMock
            .Setup(x => x.AddEmployeeAsync(companyId, employeeId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateConcurrencyException());
        
        // Act
        var result = await _companyEmployeesController.HireAsync(companyId, employeeId);
        
        // Assert
        Assert.IsType<ConflictObjectResult>(result.Result);
    }
    
    [Fact]
    public async Task RemoveEmployeeAsync_WhenEmployeeRemovedSuccessfully_ShouldReturnNoContentResult()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        
        // Act
        var result = await _companyEmployeesController.RemoveAsync(companyId, employeeId);
        
        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task RemoveEmployeeAsync_WhenEmployeeNotFound_ShouldReturnNotFoundObjectResult()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        
        _companyServiceMock
            .Setup(x => x.RemoveEmployeeAsync(companyId, employeeId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(CouldNotFindUser.WithId(employeeId));
        
        // Act
        var result = await _companyEmployeesController.RemoveAsync(companyId, employeeId);
        
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task RemoveEmployeeAsync_WhenCompanyNotFound_ShouldReturnNotFoundObjectResult()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        
        _companyServiceMock
            .Setup(x => x.RemoveEmployeeAsync(companyId, employeeId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(CouldNotFindCompany.WithId(companyId));
        
        // Act
        var result = await _companyEmployeesController.RemoveAsync(companyId, employeeId);
        
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task RemoveEmployeeAsync_WhenDbUpdateConcurrencyException_ShouldReturnConflictObjectResult()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        
        _companyServiceMock
            .Setup(x => x.RemoveEmployeeAsync(companyId, employeeId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateConcurrencyException());
        
        // Act
        var result = await _companyEmployeesController.RemoveAsync(companyId, employeeId);
        
        // Assert
        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public void UpdateEmployeeAsync_ShouldReturnStatusCode405MethodNotAllowed()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var patchDocument = new JsonPatchDocument();
        
        // Act
        var result = _companyEmployeesController.UpdateAsync(companyId, employeeId, patchDocument);
        
        // Assert
        var actualResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(405, actualResult.StatusCode);
    }

    [Fact]
    public void ReplaceEmployeeAsync_ShouldReturnStatusCode405MethodNotAllowed()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        
        // Act
        var result = _companyEmployeesController.ReplaceEmployeeAsync(companyId, employeeId);
        
        // Assert
        var actualResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(405, actualResult.StatusCode);
    }

    [Fact]
    public async Task GetEmployeeAsync_WhenEmployeeFound_ShouldReturnEmployee()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        
        _companyServiceMock
            .Setup(x => x.GetEmployeeAsync(companyId, employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmployeeDto("name"));
        
        // Act
        var result = await _companyEmployeesController.GetAsync(companyId, employeeId);
        
        // Assert
        Assert.IsType<ActionResult<EmployeeDto>>(result);
        Assert.NotNull(result.Result);
        
        var actualResult = result.Result as OkObjectResult;
        Assert.NotNull(actualResult);
        Assert.NotNull(actualResult.Value);

        var value = actualResult.Value as EmployeeDto;
        Assert.NotNull(value);
        Assert.Equal("name", value.Name);
    }

    [Fact]
    public async Task GetEmployeeAsync_WhenEmployeeNotFound_ShouldReturnNotFoundObjectResult()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        
        _companyServiceMock
            .Setup(x => x.GetEmployeeAsync(companyId, employeeId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(CouldNotFindUser.WithId(employeeId));
        
        // Act
        var result = await _companyEmployeesController.GetAsync(companyId, employeeId);
        
        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task ListAsync_WhenPaginationResultEmpty_ShouldReturnOkObjectResult()
    {
        // Arrange
        var paginationResult = new PaginationResult<EmployeeDto>([], 1, 1, 0, 0);
        _companyServiceMock
            .Setup(x => x.ListCompanyEmployeesAsync(It.IsAny<Guid>(), It.IsAny<ApplicationUserSpecification>(),
                1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginationResult);
        
        // Act
        var result = await _companyEmployeesController.ListAsync(Guid.NewGuid(), new ApplicationUserFilter(),
            1, 1);
        
        // Assert
        var actualResult = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<PaginationResult<EmployeeDto>>(actualResult.Value);
        Assert.Empty(value.Items);
    }

    [Fact]
    public async Task ListAsync_WhenPaginationResultNotEmpty_ShouldReturnOkObjectResult()
    {
        // Arrange
        var paginationResult = new PaginationResult<EmployeeDto>([new EmployeeDto("name")], 1, 1, 1, 1);
        _companyServiceMock
            .Setup(x => x.ListCompanyEmployeesAsync(It.IsAny<Guid>(), It.IsAny<ApplicationUserSpecification>(),
                1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginationResult);
        
        // Act
        var result = await _companyEmployeesController.ListAsync(Guid.NewGuid(), new ApplicationUserFilter(),
            1, 1);
        
        // Assert
        var actualResult = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<PaginationResult<EmployeeDto>>(actualResult.Value);
        Assert.NotEmpty(value.Items);
    }
}