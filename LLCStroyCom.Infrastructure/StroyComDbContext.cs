using LLCStroyCom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LLCStroyCom.Infrastructure;

public class StroyComDbContext : DbContext
{
    public StroyComDbContext(DbContextOptions<StroyComDbContext> options)
        : base(options)
    {
        
    }
    
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<ApplicationRole> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");
        
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("user");

            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("email");
            
            entity.Property(e => e.HashPassword)
                .IsRequired()
                .HasMaxLength(512)
                .HasColumnName("hash_password");

            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);
        });

        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            entity.ToTable("role");

            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("type");
        });
    }
}