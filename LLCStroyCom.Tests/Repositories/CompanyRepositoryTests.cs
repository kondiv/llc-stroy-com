using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Infrastructure;
using LLCStroyCom.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Tests.Repositories;

public class CompanyRepositoryTests
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyRepositoryTests()
    {
        var context = GetInMemoryDbContext();
        _companyRepository = new CompanyRepository(context);
    }

    private static StroyComDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<StroyComDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        return new StroyComDbContext(options);
    }

    [Fact]
    public async Task GetAsync_WhenCompanyDoesNotExist_ShouldThrowCouldNotFindCompany()
    {
        // Arrange
        
        // Act
        var act = () => _companyRepository.GetAsync(Guid.NewGuid());
        
        // Assert
        await Assert.ThrowsAsync<CouldNotFindCompany>(act);
    }

    [Fact]
    public async Task GetAsync_WhenCompanyExists_ShouldReturnCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var company = new Company()
        {
            Id = companyId,
            Name = "ЮгСтройИнвест"
        };
        
        var context = GetInMemoryDbContext();
        ICompanyRepository companyRepository = new CompanyRepository(context);
        
        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();
        
        // Act
        var result = await companyRepository.GetAsync(companyId);
        
        // Assert
        Assert.Equal(company.Name, result.Name);
    }

    [Fact]
    public async Task GetAsync_WhenOperationIsCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        
        // Act
        var act = () => _companyRepository.GetAsync(Guid.NewGuid(), cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenCompanyDoesNotExist_ShouldCreateCompanyInDb()
    {
        // Arrange
        var company = new Company()
        {
            Name = "ЮгСтройИнвест"
        };
        
        var context = GetInMemoryDbContext();
        ICompanyRepository companyRepository = new CompanyRepository(context);
        
        // Act
        await companyRepository.CreateAsync(company);
        
        // Assert
        Assert.Equal(1, await context.Companies.CountAsync());
    }

    [Fact]
    public async Task CreateAsync_WhenCompanyExists_ShouldCreateCompanyInDb()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var company = new Company()
        {
            Id = companyId,
            Name = "ЮгСтройИнвест"
        };
        
        var context = GetInMemoryDbContext();
        ICompanyRepository companyRepository = new CompanyRepository(context);
        
        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();
        
        // Act
        var act = () => companyRepository.CreateAsync(company);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenCompanyIsInvalidModel_ShouldThrowDbUpdateException()
    {
        // Arrange
        var company = new Company();
        
        // Act
        var act = () => _companyRepository.CreateAsync(company);
        
        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(act);
    }
    
    [Fact]
    public async Task CreateAsync_WhenOperationIsCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        var company = new Company()
        {
            Name = "name",
        };
        
        // Act
        var act = () => _companyRepository.CreateAsync(company, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenCompanyIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Company? company = null;
        
        // Act
        var act = () => _companyRepository.CreateAsync(company!);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

    [Fact]
    public async Task DeleteAsync_WhenCompanyFound_ShouldDeleteCompanyFromDb()
    {
        // Arrange
        var company = new Company()
        {
            Id = Guid.NewGuid(),
            Name = "name"
        };
        var context = GetInMemoryDbContext();
        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();
        
        ICompanyRepository companyRepository = new CompanyRepository(context);
        
        // Act
        await companyRepository.DeleteAsync(company.Id);
        
        // Assert
        Assert.Equal(0, await context.Companies.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_WhenCompanyNotFound_ShouldThrowCompanyNotFoundException()
    {
        // Arrange
        
        // Act
        var act = () => _companyRepository.DeleteAsync(Guid.NewGuid());
        
        // Assert
        await Assert.ThrowsAsync<CouldNotFindCompany>(act);
    }

    [Fact]
    public async Task DeleteAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var company = new Company()
        {
            Id = Guid.NewGuid(),
            Name = "name"
        };
        var cancellationToken = new CancellationToken(canceled: true);
        var context = GetInMemoryDbContext();
        
        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();
        ICompanyRepository companyRepository = new CompanyRepository(context);
        
        // Act
        var act = () => companyRepository.DeleteAsync(company.Id, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(act);
        Assert.Equal(1, await context.Companies.CountAsync());
    }
}