using LLCStroyCom.Domain.Constants;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Infrastructure;
using LLCStroyCom.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Tests.Seeders;

public class RoleSeederTests
{
    private static StroyComDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<StroyComDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        return new StroyComDbContext(options);
    }
    
    [Fact]
    public async Task SeedAsync_WhenNoRoleRecordsInDb_ShouldAddAllNonExistingRoles()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var seeder = new RoleSeeder(context);
        
        // Act
        await seeder.SeedAsync();
        
        // Assert
        Assert.Equal(3, await context.Roles.CountAsync());
    }
    
    [Fact]
    public async Task SeedAsync_WhenSomeRoleRecordsInDb_ShouldAddRemainedRoles()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var seeder = new RoleSeeder(context);

        var engineer = new ApplicationRole() { Type = RoleType.Engineer };
        var manager = new ApplicationRole() { Type = RoleType.Manager };
        
        await context.Roles.AddRangeAsync(engineer, manager);
        await context.SaveChangesAsync();

        var rolesInDb = await context.Roles.CountAsync();
        
        // Act
        await seeder.SeedAsync();
        
        // Assert
        Assert.NotEqual(rolesInDb, await context.Roles.CountAsync());
        Assert.Equal(3, await context.Roles.CountAsync());
    }

    [Fact]
    public async Task SeedAsync_WhenAllRolesAreInDb_ShouldNotAddAnyRole()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var seeder = new RoleSeeder(context);
        
        var engineer = new ApplicationRole { Type = RoleType.Engineer };
        var manager = new ApplicationRole { Type = RoleType.Manager };
        var observer = new ApplicationRole { Type = RoleType.Observer };
        
        await context.Roles.AddRangeAsync(engineer, manager, observer);
        await context.SaveChangesAsync();

        var rolesInDb = await context.Roles.CountAsync();
        
        // Act
        await seeder.SeedAsync();
        
        // Assert
        Assert.Equal(rolesInDb, await context.Roles.CountAsync());
    }
}