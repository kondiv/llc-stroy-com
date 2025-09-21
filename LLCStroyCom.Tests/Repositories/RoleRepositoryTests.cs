using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Infrastructure;
using LLCStroyCom.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Tests.Repositories;

public class RoleRepositoryTests
{
    private static StroyComDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<StroyComDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        return new StroyComDbContext(options);
    }

    [Fact]
    public async Task GetByNameAsync_WhenNameIsWhiteSpace_ShouldThrowArgumentException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IRoleRepository roleRepository = new RoleRepository(context);
        var roleName = "       ";
        
        // Act
        var act = () => roleRepository.GetByNameAsync(roleName);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }
    
    [Fact]
    public async Task GetByNameAsync_WhenNameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IRoleRepository roleRepository = new RoleRepository(context);
        string? roleName = null;
        
        // Act
        var act = () => roleRepository.GetByNameAsync(roleName!);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

    [Fact]
    public async Task GetByNameAsync_WhenNameIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IRoleRepository roleRepository = new RoleRepository(context);
        var roleName = string.Empty;
        
        // Act
        var act = () => roleRepository.GetByNameAsync(roleName);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }
    
    [Fact]
    public async Task GetByNameAsync_WhenRoleWithProvidedNameDoesNotExist_ShouldThrowCouldNotFindRoleException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IRoleRepository roleRepository = new RoleRepository(context);
        var roleName = "user";
        
        // Act
        var act = () => roleRepository.GetByNameAsync(roleName);
        
        // Assert
        await Assert.ThrowsAsync<CouldNotFindRole>(act);
    }

    [Fact]
    public async Task GetByNameAsync_WhenRoleWithProvidedNameExists_ShouldReturnRole()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IRoleRepository roleRepository = new RoleRepository(context);
        
        var roleName = "user";
        var role = new ApplicationRole()
        {
            Type = roleName,
        };
        
        await context.Roles.AddAsync(role);
        await context.SaveChangesAsync();
        
        // Act
        var result = await roleRepository.GetByNameAsync(roleName);
        
        // Assert
        Assert.Equal(role, result);
    }

    [Fact]
    public async Task GetByNameAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IRoleRepository roleRepository = new RoleRepository(context);
        var roleName = "user";
        var cancellationToken = new CancellationToken(canceled: true);
        
        // Act
        var act = () => roleRepository.GetByNameAsync(roleName, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }
}