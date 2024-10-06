using BugTracker.API.Entities;
using BugTracker.Shared.Commons;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.API.Data;

public class AppDbContext : IdentityDbContext<AuthUser, AuthRole, string>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AppDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<BugReport> BugReports { get; set; }
    public DbSet<BugAttachment> BugAttachments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<AuthRole>(entity =>
        {
            entity.ToTable("Roles");
        });

        builder.Entity<AuthUser>(entity =>
        {
            entity.ToTable("Users");
        });

        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UserRoles");
        });

        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("UserClaims");
        });

        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("UserLogins");
        });

        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });

        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("UserTokens");
        });
    }

    public override int SaveChanges()
    {
        ApplyAuditing();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditing();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditing()
    {
        var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value; ;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditableEntity auditableEntity)
            {
                if (entry.State == EntityState.Added)
                {
                    auditableEntity.SetCreatedInfo(currentUserId);
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditableEntity.SetModifiedInfo(currentUserId);
                }
            }
        }
    }
}
