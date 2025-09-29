using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Enums;
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
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Defect> Defects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");
        
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("user");

            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.HasIndex(e => new{e.Id, e.CompanyId}).IsUnique();
            
            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("name");
            
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
            
            entity.Property(e => e.CompanyId)
                .HasColumnName("company_id");
            
            entity.HasOne(u => u.Company)
                .WithMany(c => c.Employees)
                .HasForeignKey(u => u.CompanyId);

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

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_token");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("now()")
                .ValueGeneratedOnAdd()
                .HasColumnName("created_at");

            entity.Property(e => e.ExpiresAt)
                .IsRequired()
                .HasColumnName("expires_at");

            entity.Property(e => e.RevokedAt)
                .HasColumnName("revoked_at");

            entity.Property(e => e.TokenHash)
                .IsRequired()
                .HasColumnName("token_hash");
            
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("user_id");

            entity.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("project");
            
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => new {e.City, e.CompanyId, e.Name}).IsUnique();
            
            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("name");
            
            entity.Property(e => e.City)
                .IsRequired()
                .HasMaxLength(180)
                .HasColumnName("city");
            
            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            
            entity.Property(e => e.Status)
                .IsRequired()
                .HasColumnName("status");
            
            entity.Property(e => e.CompanyId)
                .IsRequired()
                .HasColumnName("company_id");
            
            entity.HasOne(p => p.Company)
                .WithMany(c => c.Projects)
                .HasForeignKey(p => p.CompanyId);
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("company");
            
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.Name).IsUnique();
            
            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(127)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Defect>(entity =>
        {
            entity.ToTable("defect");

            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnName("name");
            
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(512)
                .HasColumnName("description");
            
            entity.Property(e => e.Status)
                .IsRequired()
                .HasDefaultValue(Status.New)
                .HasColumnName("status");
            
            entity.Property(e => e.ProjectId)
                .IsRequired()
                .HasColumnName("project_id");
            
            entity.Property(e => e.ChiefEngineerId)
                .HasColumnName("chief_engineer_id");

            entity.HasOne(d => d.ChiefEngineer)
                .WithMany(e => e.Defects)
                .HasForeignKey(d => d.ChiefEngineerId);
            
            entity.HasOne(d => d.Project)
                .WithMany(p => p.Defects)
                .HasForeignKey(d => d.ProjectId);
        });
    }
}