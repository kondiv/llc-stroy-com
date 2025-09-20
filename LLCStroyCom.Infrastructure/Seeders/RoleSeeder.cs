using LLCStroyCom.Domain.Constants;
using LLCStroyCom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Infrastructure.Seeders;

public sealed class RoleSeeder
{
    private readonly StroyComDbContext _context;

    public RoleSeeder(StroyComDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        var existingRoleTypes = await _context.Roles.Select(r => r.Type).ToListAsync();
        var allRoleTypes = RoleType.AllTypes();

        foreach (var roleType in allRoleTypes)
        {
            if (!existingRoleTypes.Contains(roleType))
            {
                await _context.Roles.AddAsync(new ApplicationRole()
                {
                    Type = roleType
                });
            }
        }
        
        await _context.SaveChangesAsync();
    }
}