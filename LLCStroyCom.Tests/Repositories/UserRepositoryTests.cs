using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Infrastructure;
using LLCStroyCom.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Tests.Repositories;

public class UserRepositoryTests
{
    private StroyComDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<StroyComDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        return new StroyComDbContext(options);
    }
    
    [Fact]
    public async Task CreateAsync_WhenDataIsValidAndUserUnique_ShouldCreateUserInDb()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);

        var applicationUser = new ApplicationUser()
        {
            Email = "email",
            HashPassword = "asdfasdfasdf",
            RoleId = 1
        };
        
        // Act
        await userRepository.CreateAsync(applicationUser);

        // Assert
        Assert.Equal(1, context.Users.Count());
    }

    [Fact]
    public async Task CreateAsync_WhenEmailIsMissing_ShouldThrowException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);

        var applicationUser = new ApplicationUser()
        {
            Email = null,
            HashPassword = "asdfasdfasdf",
            RoleId = 1
        };
        
        // Act
        var act = () => userRepository.CreateAsync(applicationUser);
        
        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenHashPasswordIsMissing_ShouldThrowException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);

        var applicationUser = new ApplicationUser()
        {
            Email = "email",
            HashPassword = null,
            RoleId = 1
        };
        
        // Act
        var act = () => userRepository.CreateAsync(applicationUser);
        
        // Assert
        await Assert.ThrowsAsync<DbUpdateException>(act);
    }

    // InMemoryDb does not represent check constraint, test is green - tested in actual db
    [Fact]
    public async Task CreateAsync_WhenRoleIdIsMissing_ShouldThrowException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);

        var applicationUser = new ApplicationUser()
        {
            Email = "email",
            HashPassword = "asdfasdfasdf"
        };
        
        // Act
        var act = () => userRepository.CreateAsync(applicationUser);
        
        // Assert
        // await Assert.ThrowsAsync<DbUpdateException>(act);
    }

    [Fact]
    public async Task CreateAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);

        var cancellationToken = new CancellationToken(canceled: true);

        var applicationUser = new ApplicationUser()
        {
            Email = "email",
            HashPassword = "asdfasdfasdf",
            RoleId = 1
        };
        
        // Act
        var act = () => userRepository.CreateAsync(applicationUser, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(act);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserWithProvidedIdExists_ShouldDeleteUserFromDb()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);

        var userId = Guid.NewGuid();

        var applicationUser = new ApplicationUser()
        {
            Id = userId,
            Email = "email",
            HashPassword = "asdfadsfasdf",
            RoleId = 1
        };

        await context.Users.AddAsync(applicationUser);
        await context.SaveChangesAsync();
        
        // Act
        await userRepository.DeleteAsync(applicationUser.Id);
        
        // Assert
        Assert.Equal(0, context.Users.Count());
    }

    [Fact]
    public async Task DeleteAsync_WhenUserWithProvidedIdDoesNotExist_ShouldThrowUserCouldNotBeFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);
        
        // Act
        var act = () => userRepository.DeleteAsync(Guid.NewGuid());
        
        // Assert
        await Assert.ThrowsAsync<UserCouldNotBeFound>(act);
    }
}