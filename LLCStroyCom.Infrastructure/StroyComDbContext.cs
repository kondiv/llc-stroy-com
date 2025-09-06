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
            
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("email");
            
            entity.Property(e => e.HashPassword)
                .IsRequired()
                .HasMaxLength(512)
                .HasColumnName("hash_password");
            
            entity.Property(e => e.RoleId)
                .IsRequired()
                .HasAnnotation("CheckConstraint", "Ck_ApplicatinoUser_RoleId_Positive")
                .HasColumnName("role_id");

            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            entity.ToTable(t =>
                t.HasCheckConstraint(
                "Ck_ApplicatinoUser_RoleId_Positive",
                "role_id > 0"));
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