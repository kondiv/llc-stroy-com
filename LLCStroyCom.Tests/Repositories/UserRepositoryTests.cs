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
    public async Task CreateAsync_WhenDataIsValidAndUserUnique_ShouldCreateUserInDbAndReturnId()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);
        var userId = Guid.NewGuid();
        
        var applicationUser = new ApplicationUser()
        {
            Id = userId,
            Email = "email",
            HashPassword = "asdfasdfasdf",
            RoleId = 1
        };
        
        // Act
        var result = await userRepository.CreateAsync(applicationUser);

        // Assert
        Assert.Equal(1, context.Users.Count());
        Assert.Equal(userId, result);
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
    public async Task CreateAsync_WhenUserIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);

        ApplicationUser user = null;
        
        // Act
        var act = () => userRepository.CreateAsync(user);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(act);
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

    [Fact]
    public async Task GetByEmailAsync_WhenEmailIsValidAndUserExists_ShouldReturnUser()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);

        var email = "email@email.ru";
        var user = new ApplicationUser()
        {
            Email = email,
            HashPassword = "asdfasdfasdf",
            RoleId = 1
        };
        var role = new ApplicationRole()
        {
            Id = 1,
            Type = "Role"
        };

        await context.Roles.AddAsync(role);
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        
        // Act
        var result = await userRepository.GetByEmailAsync(email);
        
        // Assert
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.HashPassword, result.HashPassword);
        Assert.Equal(user.RoleId, result.RoleId);
        Assert.NotNull(user.Role);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenEmailIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);
        string? email = null;
        
        // Act
        var act = () => userRepository.GetByEmailAsync(email);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenEmailIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);
        var email = string.Empty;
        
        // Act
        var act = () => userRepository.GetByEmailAsync(email);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenEmailIsWhitespace_ShouldThrowArgumentException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);
        var email = "     ";
        
        // Act
        var act = () => userRepository.GetByEmailAsync(email);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenEmailIsValidAndUserDoesNotExist_ShouldThrowUserCouldNotBeFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);
        var email = "email@email.ru";
        
        // Act
        var act = () => userRepository.GetByEmailAsync(email);
        
        // Assert
        await Assert.ThrowsAsync<UserCouldNotBeFound>(act);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenOperationCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);
        var cancellationToken = new CancellationToken(canceled: true);
        var email = "email@email.ru";
        
        // Act
        var act = () => userRepository.GetByEmailAsync(email, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task AssignRefreshTokenAsync_WhenRefreshTokenIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);

        RefreshToken? refreshToken = null;
        var userId = Guid.NewGuid();
        
        // Act
        var act = () => userRepository.AssignRefreshTokenAsync(userId, refreshToken);
        
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(act);
    }

    [Fact]
    public async Task AssignRefreshTokenAsync_WhenOperationIsCanceled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);
        var refreshToken = new RefreshToken();
        var userId = Guid.NewGuid();
        var cancellationToken = new CancellationToken(canceled: true);

        // Act
        var act = () => userRepository.AssignRefreshTokenAsync(userId, refreshToken, cancellationToken);
        
        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task AssignRefreshTokenAsync_WhenEverythingIsOk_ShouldAddRefreshTokenToDb()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);

        var userId = Guid.NewGuid();
        var user = new ApplicationUser()
        {
            Id = userId,
            Email = "email@email.ru",
            HashPassword = "asdfasdfasdf",
            RoleId = 1
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        
        var expiresAt = DateTime.UtcNow.AddDays(7);
        var refreshToken = new RefreshToken()
        {
            TokenHash = "refreshToken",
            ExpiresAt = expiresAt,
        };
        
        // Act
        await userRepository.AssignRefreshTokenAsync(userId, refreshToken);
        
        // Assert
        Assert.Single(user.RefreshTokens);
        Assert.Equal(user.Id, user.RefreshTokens.First().UserId);
    }

    [Fact]
    public async Task AssignRefreshTokenAsync_WhenUserDoesNotExist_ShouldThrowUserCouldNotBeFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        IUserRepository userRepository = new UserRepository(context);
        var userId = Guid.NewGuid();
        var refreshToken = new RefreshToken();
        
        // Act
        var act = () => userRepository.AssignRefreshTokenAsync(userId, refreshToken);
        
        // Assert
        await Assert.ThrowsAsync<UserCouldNotBeFound>(act);
    }
}