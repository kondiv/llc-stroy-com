using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Specifications.Companies;
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
        var result = await companyRepository.CreateAsync(company);
        
        // Assert
        Assert.Equal(1, await context.Companies.CountAsync());
        Assert.NotNull(result);
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

    [Fact]
    public async Task GetExtendedAsync_WhenCompanyExists_ShouldReturnCompany()
    {
        // Arrange
        var company = new Company()
        {
            Id = Guid.NewGuid(),
            Name = "name",
            Employees =
            {
                new ApplicationUser()
                {
                    Email = "email",
                    HashPassword = "hashPassword",
                    Name = "name"
                }
            },
            Projects =
            {
                new Project()
                {
                    City = "city",
                    Name = "name"
                }
            }
        };
        
        var context = GetInMemoryDbContext();
        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();
        var repository = new CompanyRepository(context);
        
        // Act
        var result = await repository.GetExtendedAsync(company.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Employees);
        Assert.NotNull(result.Projects);
    }

    [Fact]
    public async Task GetExtendedAsync_WhenCompanyNotFound_ShouldThrowCompanyNotFoundException()
    {
        // Arrange
        // Act
        var act = () =>  _companyRepository.GetExtendedAsync(Guid.NewGuid());
        
        // Assert
        await Assert.ThrowsAsync<CouldNotFindCompany>(act);
    }

    [Fact]
    public async Task GetExtendedAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        
        // Act
        var act = () => _companyRepository.GetExtendedAsync(Guid.NewGuid(), cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task UpdateAsync_WhenCompanyNotFound_ShouldThrowDbUpdateException()
    {
        // Arrange
        
        // Act
        var act = () => _companyRepository.UpdateAsync(new Company());
        
        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(act);
    }

    [Fact]
    public async Task UpdateAsync_WhenCompanyFound_ShouldUpdateCompanyInDb()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var company = new Company()
        {
            Id = companyId,
            Name = "name"
        };
 
        var context = GetInMemoryDbContext();
        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();

        context.Entry(company).State = EntityState.Detached;

        var repository = new CompanyRepository(context);

        var companyToUpdate = new Company()
        {
            Id = companyId,
            Name = "new_name"
        };

        // Act
        await repository.UpdateAsync(companyToUpdate);
    
        // Assert
        var updatedCompany = await context.Companies.FindAsync(companyId);
        Assert.Equal("new_name", updatedCompany!.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenTryingToUpdateId_ShouldThrowDbUpdateException()
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
        var repository = new CompanyRepository(context);
        
        // Act
        company.Id = Guid.NewGuid();
        var act = () => _companyRepository.UpdateAsync(company);
        
        // Assert
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(act);
        Assert.Equal("name", company.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        
        // Act
        var act = () => _companyRepository.UpdateAsync(new Company(), cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(act);
    }

    [Fact]
    public async Task ListAsync_WhenItemsLessThanMaxPageSize_ShouldReturnPaginationResultWithNoNextPage()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        await context.Companies.AddAsync(new Company()
        {
            Id = Guid.NewGuid(),
            Name = "name"
        });
        await context.SaveChangesAsync();
        var repository = new CompanyRepository(context);
        
        const int maxPageSize = 2;
        const int page = 1;
        
        // Act
        var result = await repository.ListAsync(new CompanySpecification(new CompanyFilter()), maxPageSize, page);
        
        // Assert
        Assert.NotEmpty(result.Items);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.PageCount);
        Assert.False(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public async Task ListAsync_WhenItemsEqualsMaxPageSize_ShouldReturnPaginationResultWithNoNextPage()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        await context.Companies.AddAsync(new Company()
        {
            Id = Guid.NewGuid(),
            Name = "name"
        });
        await context.SaveChangesAsync();
        var repository = new CompanyRepository(context);
        
        const int maxPageSize = 1;
        const int page = 1;
        
        // Act
        var result = await repository.ListAsync(new CompanySpecification(new CompanyFilter()), maxPageSize, page);
        
        // Assert
        Assert.NotEmpty(result.Items);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.PageCount);
        Assert.False(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public async Task ListAsync_WhenItemsMoreThanMaxPageSize_ShouldReturnPaginationResultWithNextPage()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        await context.Companies.AddRangeAsync(
            new Company()
            {
                Id = Guid.NewGuid(),
                Name = "name"
            },
            new Company()
            {
                Id = Guid.NewGuid(),
                Name = "name1"
            });
        await context.SaveChangesAsync();
        var repository = new CompanyRepository(context);
        
        const int maxPageSize = 1;
        const int page = 1;
        
        // Act
        var result = await repository.ListAsync(new CompanySpecification(new CompanyFilter()), maxPageSize, page);
        
        // Assert
        Assert.NotEmpty(result.Items);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.PageCount);
        Assert.True(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public async Task ListAsync_WhenNoItems_ShouldReturnEmptyPaginationResult()
    {
        // Arrange
        const int maxPageSize = 10;
        const int page = 1;
        
        // Act
        var result = await _companyRepository.ListAsync(new CompanySpecification(new CompanyFilter()), maxPageSize, page);
        
        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.PageCount);
        Assert.False(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public async Task ListAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cancellationToken = new CancellationToken(canceled: true);
        
        // Act
        var act = () => _companyRepository.ListAsync(new CompanySpecification(new CompanyFilter()),
            10, 1, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }
}