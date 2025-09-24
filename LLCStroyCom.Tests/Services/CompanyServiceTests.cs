using AutoMapper;
using LLCStroyCom.Application.MapperProfiles;
using LLCStroyCom.Application.Services;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace LLCStroyCom.Tests.Services;

public class CompanyServiceTests
{
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly ICompanyService _companyService;

    public CompanyServiceTests()
    {
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        var loggerMock = new Mock<ILogger<CompanyService>>();
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CompanyProfile>();
        }, new LoggerFactory());
        
        _companyService = new CompanyService(_companyRepositoryMock.Object, loggerMock.Object,
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
            Employees = [],
            Projects = []
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
}