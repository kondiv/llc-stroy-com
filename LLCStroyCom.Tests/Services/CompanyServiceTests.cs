using AutoMapper;
using LLCStroyCom.Application.MapperProfiles;
using LLCStroyCom.Application.Services;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Models;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.Services;
using LLCStroyCom.Domain.Specifications.Companies;
using LLCStroyCom.Domain.Specifications.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Moq;

namespace LLCStroyCom.Tests.Services;

public class CompanyServiceTests
{
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly ICompanyService _companyService;

    public CompanyServiceTests()
    {
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        var loggerMock = new Mock<ILogger<CompanyService>>();
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CompanyProfile>();
            cfg.AddProfile<UserProfile>();
        }, new LoggerFactory());
        
        _companyService = new CompanyService(
            _companyRepositoryMock.Object,
            _userRepositoryMock.Object,
            loggerMock.Object,
            mapperConfiguration.CreateMapper());
    }

    [Fact]
    public async Task GetAsync_WhenCompanyFound_ShouldReturnCompanyDto()
    {
        // Arrange
        var company = new Company()
        {
            Id = Guid.NewGuid(),
            Name = "Company",
        };
        
        _companyRepositoryMock
            .Setup(x => x.GetAsync(company.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);
        
        // Act
        var companyDto = await _companyService.GetAsync(company.Id);
        
        // Assert
        Assert.NotNull(companyDto);
        Assert.IsType<CompanyDto>(companyDto);
        Assert.Equal(company.Id, companyDto.Id);
        Assert.Equal(company.Name, companyDto.Name);
    }

    [Fact]
    public async Task GetAsync_WhenCompanyNotFound_ShouldThrowCouldNotFindCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        _companyRepositoryMock
            .Setup(x => x.GetAsync(companyId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(CouldNotFindCompany.WithId(companyId));
        
        // Act
        var act = () => _companyService.GetAsync(companyId, It.IsAny<CancellationToken>());
        
        // Assert
        await Assert.ThrowsAsync<CouldNotFindCompany>(act);
    }

    [Fact]
    public async Task GetAsync_WhenOperationCanceled_ShouldThrowOperationCanceled()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        _companyRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), cancellationToken))
            .ThrowsAsync(new OperationCanceledException());
        
        // Act
        var act = () => _companyService.GetAsync(Guid.NewGuid(), cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task DeleteAsync_WhenCompanyNotFound_ShouldThrowCouldNotFindCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        _companyRepositoryMock
            .Setup(x => x.DeleteAsync(companyId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(CouldNotFindCompany.WithId(companyId));
        
        // Act
        var act = () => _companyService.DeleteAsync(companyId);
        
        // Assert
        await Assert.ThrowsAsync<CouldNotFindCompany>(act);
    }

    [Fact]
    public async Task DeleteAsync_WhenOperationCanceled_ShouldThrowOperationCanceled()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        _companyRepositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<Guid>(), cancellationToken))
            .ThrowsAsync(new OperationCanceledException());
        
        // Act
        var act = () => _companyService.DeleteAsync(Guid.NewGuid(), cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task DeleteAsync_WhenDeletingSuccessful_ShouldReturnNothing()
    {
        // Arrange
        var defectId = Guid.NewGuid();
        
        // Act
        await _companyService.DeleteAsync(defectId);
        
        // Assert
        _companyRepositoryMock
            .Verify(r => r.DeleteAsync(defectId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenThrowsArgumentException_ShouldThrowProjectAlreadyExists()
    {
        // Arrange
        _companyRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException());
        
        // Act
        var act = () => _companyService.CreateAsync(new CompanyCreateRequest("name"));
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenCreateSuccessful_ShouldReturnNothing()
    {
        // Arrange
        var createCompanyRequest = new CompanyCreateRequest("name");
        
        // Act
        await _companyService.CreateAsync(createCompanyRequest);
        
        // Assert
        _companyRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenOperationCanceled_ShouldThrowOperationCanceled()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        
        _companyRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());
        
        // Act
        var act = () => _companyService.CreateAsync(new CompanyCreateRequest("name"), cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenRequestIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        CompanyCreateRequest? request = null;
        
        // Act
        var act = () => _companyService.CreateAsync(request!);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

    [Fact]
    public async Task AddEmployeeAsync_WhenUserNotFound_ShouldThrowCouldNotFindUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        _userRepositoryMock
            .Setup(x => x.GetAsync(userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(CouldNotFindUser.WithId(userId));
        
        // Act
        var act = () => _companyService.AddEmployeeAsync(Guid.NewGuid(), userId);
        
        // Assert
        await Assert.ThrowsAsync<CouldNotFindUser>(act);
    }
    
    [Fact]
    public async Task AddEmployeeAsync_WhenCompanyNotFound_ShouldThrowCouldNotFindCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        
        _companyRepositoryMock
            .Setup(x => x.GetExtendedAsync(companyId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(CouldNotFindCompany.WithId(companyId));
        
        // Act
        var act = () => _companyService.AddEmployeeAsync(companyId, Guid.NewGuid());
        
        // Assert
        await Assert.ThrowsAsync<CouldNotFindCompany>(act);
    }

    [Fact]
    public async Task AddEmployeeAsync_WhenEmployeeAlreadyHasCompany_ShouldThrowAlreadyWorks()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var employee = new ApplicationUser()
        {
            Id = userId,
        };
        var company = new Company()
        {
            Id = companyId,
        };
        employee.SetCompany(Guid.NewGuid());
        
        _userRepositoryMock
            .Setup(x => x.GetAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);
        
        _companyRepositoryMock
            .Setup(x => x.GetExtendedAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);
        
        // Act
        var act = () => _companyService.AddEmployeeAsync(companyId, userId);
        
        // Assert
        await Assert.ThrowsAsync<AlreadyWorks>(act);
    }

    // Can't happen in scenario of adding employee by AddEmployeeAsync method
    // Test written for testing Company entity method AddEmployee
    [Fact]
    public async Task AddEmployeeAsync_WhenCompanyAlreadyHasThisEmployee_ShouldThrowAlreadyWorks()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var employee = new ApplicationUser()
        {
            Id = userId,
        };
        var company = new Company()
        {
            Id = companyId,
        };
        company.AddEmployee(employee);
        
        _userRepositoryMock
            .Setup(x => x.GetAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);
        
        _companyRepositoryMock
            .Setup(x => x.GetExtendedAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);
        
        // Act
        var act = () => _companyService.AddEmployeeAsync(companyId, userId);
        
        // Assert
        await Assert.ThrowsAsync<AlreadyWorks>(act);
    }

    [Fact]
    public async Task AddEmployeeAsync_WhenDbUpdateConcurrencyException_ShouldThrowConcurrencyException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var employee = new ApplicationUser()
        {
            Id = userId,
        };
        var company = new Company()
        {
            Id = companyId,
        };
        
        _userRepositoryMock
            .Setup(x => x.GetAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);
        
        _companyRepositoryMock
            .Setup(x => x.GetExtendedAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);

        _companyRepositoryMock
            .Setup(x => x.UpdateAsync(company, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateConcurrencyException());
        
        // Act
        var act = () => _companyService.AddEmployeeAsync(companyId, userId);
        
        // Assert
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(act);
    }

    [Fact]
    public async Task AddEmployeeAsync_WhenOperationCanceledException_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        
        _userRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), cancellationToken))
            .ThrowsAsync(new OperationCanceledException());
        
        // Act
        var act = () => _companyService.AddEmployeeAsync(Guid.NewGuid(), Guid.NewGuid(), cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task AddEmployeeAsync_WhenEverythingOk_ShouldUpdateCompany()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var employee = new ApplicationUser()
        {
            Id = userId,
        };
        var company = new Company()
        {
            Id = companyId,
        };
        
        _userRepositoryMock
            .Setup(x => x.GetAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);
        
        _companyRepositoryMock
            .Setup(x => x.GetExtendedAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);
        
        // Act
        await _companyService.AddEmployeeAsync(companyId, userId);
        
        // Assert
        _companyRepositoryMock.Verify(x => x.UpdateAsync(company, It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(employee.CompanyId);
        Assert.NotEmpty(company.Employees);
    }

    [Fact]
    public async Task GetEmployeeAsync_WhenEmployeeFound_ShouldReturnEmployeeDto()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var employee = new ApplicationUser()
        {
            Id = employeeId,
            Name = "Илья",
        };
        employee.SetCompany(companyId);

        _userRepositoryMock
            .Setup(x => x.GetAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);
        
        // Act
        var result = await _companyService.GetEmployeeAsync(companyId, employeeId, CancellationToken.None);
        
        // Assert
        Assert.IsType<EmployeeDto>(result);
        Assert.Equal(employee.Name, result.Name);
    }

    [Fact]
    public async Task GetEmployeeAsync_WhenUserNotFound_ShouldThrowCouldNotFindUser()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        
        _userRepositoryMock
            .Setup(x => x.GetAsync(employeeId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(CouldNotFindUser.WithId(employeeId));
        
        // Act
        var act = () => _companyService.GetEmployeeAsync(companyId, employeeId, CancellationToken.None);
        
        // Assert
        await Assert.ThrowsAsync<CouldNotFindUser>(act);
    }

    [Fact]
    public async Task GetEmployeeAsync_WhenUserNotFromTheCompany_ShouldThrowCouldNotFindUser()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();

        var employee = new ApplicationUser();
        employee.SetCompany(Guid.NewGuid());

        _userRepositoryMock
            .Setup(x => x.GetAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);
        
        // Act
        var act = () => _companyService.GetEmployeeAsync(companyId, employeeId);
        
        // Assert
        await Assert.ThrowsAsync<CouldNotFindUser>(act);
    }

    [Fact]
    public async Task GetEmployeeAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);

        _userRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Guid>(), cancellationToken))
            .ThrowsAsync(new OperationCanceledException());
        
        // Act
        var act = () => _companyService.GetEmployeeAsync(Guid.NewGuid(), Guid.NewGuid(), cancellationToken);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task ListAsync_WhenPaginatedResultNotEmpty_ShouldReturnNotEmptyPaginatedResultOfEntityDto()
    {
        // Arrange
        var repositoryPaginatedResult = new PaginationResult<Company>([new Company() { Name = "name" }], 1, 1, 1, 1);
        _companyRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<CompanySpecification>(), 1, 1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryPaginatedResult);
        
        // Act
        var result = await _companyService.ListAsync(new CompanySpecification(new CompanyFilter()), 1, 1);
        
        // Assert
        Assert.IsType<PaginationResult<CompanyDto>>(result);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task ListAsync_WhenPaginatedResultEmpty_ShouldReturnEmptyPaginatedResultOfEntityDto()
    {
        // Arrange
        var repositoryPaginatedResult = new PaginationResult<Company>([], 1, 1, 0, 0);
        _companyRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<CompanySpecification>(), 1, 1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryPaginatedResult);
        
        // Act
        var result = await _companyService.ListAsync(new CompanySpecification(new CompanyFilter()), 1, 1);
        
        // Assert
        Assert.IsType<PaginationResult<CompanyDto>>(result);
        Assert.Empty(result.Items);
    }
    
    [Fact]
    public async Task ListAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        _companyRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<CompanySpecification>(), 1, 1, 
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException(cancellationToken));
        
        // Act
        var act = () => _companyService.ListAsync(new CompanySpecification(new CompanyFilter()), 1, 
            1, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task ListCompanyEmployeesAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        
        _userRepositoryMock
            .Setup(x => x.ListCompanyEmployeesAsync(It.IsAny<Guid>(), It.IsAny<ApplicationUserSpecification>(),
                1,1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException(cancellationToken));
        
        // Act
        var act = () => _companyService.ListCompanyEmployeesAsync(Guid.NewGuid(),
            new ApplicationUserSpecification(new ApplicationUserFilter()), 1, 1,
            cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task
        ListCompanyEmployeesAsync_WhenPaginatedResultNotEmpty_ShouldReturnNotEmptyPaginatedResultOfEntityDto()
    {
        // Arrange
        var repositoryPaginationResult = new PaginationResult<ApplicationUser>([new ApplicationUser() { Name = "name" }], 1, 1,
            1, 1);
        
        _userRepositoryMock
            .Setup(x => x.ListCompanyEmployeesAsync(It.IsAny<Guid>(), It.IsAny<ApplicationUserSpecification>(),
                1,1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryPaginationResult);
        
        // Act
        var result = await _companyService.ListCompanyEmployeesAsync(Guid.NewGuid(),
            new ApplicationUserSpecification(new ApplicationUserFilter()),
            1, 1);
        
        // Assert
        Assert.IsType<PaginationResult<EmployeeDto>>(result);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task ListCompanyEmployeesAsync_WhenPaginatedResultEmpty_ShouldReturnEmptyPaginatedResultOfEntityDto()
    {
        // Arrange
        var repositoryPaginationResult = new PaginationResult<ApplicationUser>([], 1, 1,
            1, 1);
        
        _userRepositoryMock
            .Setup(x => x.ListCompanyEmployeesAsync(It.IsAny<Guid>(), It.IsAny<ApplicationUserSpecification>(),
                1,1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryPaginationResult);
        
        // Act
        var result = await _companyService.ListCompanyEmployeesAsync(Guid.NewGuid(),
            new ApplicationUserSpecification(new ApplicationUserFilter()),
            1, 1);
        
        // Assert
        Assert.IsType<PaginationResult<EmployeeDto>>(result);
        Assert.Empty(result.Items);
    }
}