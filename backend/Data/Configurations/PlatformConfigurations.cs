using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnevoHr.Api.Models.DeveloperPlatform;

namespace OnevoHr.Api.Data.Configurations;

public class PlatformUserConfiguration : IEntityTypeConfiguration<PlatformUser>
{
    public void Configure(EntityTypeBuilder<PlatformUser> builder)
    {
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.Email).HasMaxLength(320);
    }
}

public class PlatformPermissionConfiguration : IEntityTypeConfiguration<PlatformPermission>
{
    public void Configure(EntityTypeBuilder<PlatformPermission> builder)
    {
        builder.HasIndex(x => x.PermissionKey).IsUnique();
        builder.Property(x => x.PermissionKey).HasMaxLength(200);
    }
}

public class PlatformRolePermissionConfiguration : IEntityTypeConfiguration<PlatformRolePermission>
{
    public void Configure(EntityTypeBuilder<PlatformRolePermission> builder)
    {
        builder.HasIndex(x => new { x.PlatformRoleId, x.PlatformPermissionId }).IsUnique();

        builder.HasOne(x => x.PlatformRole)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(x => x.PlatformRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PlatformPermission)
            .WithMany()
            .HasForeignKey(x => x.PlatformPermissionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PlatformUserRoleConfiguration : IEntityTypeConfiguration<PlatformUserRole>
{
    public void Configure(EntityTypeBuilder<PlatformUserRole> builder)
    {
        builder.HasIndex(x => new { x.PlatformUserId, x.PlatformRoleId }).IsUnique();

        builder.HasOne(x => x.PlatformUser)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(x => x.PlatformUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PlatformRole)
            .WithMany()
            .HasForeignKey(x => x.PlatformRoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PlatformUserSessionConfiguration : IEntityTypeConfiguration<PlatformUserSession>
{
    public void Configure(EntityTypeBuilder<PlatformUserSession> builder)
    {
        builder.HasIndex(x => x.SessionTokenHash);

        builder.HasOne(x => x.PlatformUser)
            .WithMany()
            .HasForeignKey(x => x.PlatformUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PlatformAuthEventConfiguration : IEntityTypeConfiguration<PlatformAuthEvent>
{
    public void Configure(EntityTypeBuilder<PlatformAuthEvent> builder)
    {
        builder.HasIndex(x => x.Email);
    }
}

public class DemoRequestConfiguration : IEntityTypeConfiguration<DemoRequest>
{
    public void Configure(EntityTypeBuilder<DemoRequest> builder)
    {
        builder.ToTable("demo_access_requests");
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.RequesterEmail);
        builder.Property(x => x.RequestedModuleKeys).HasColumnType("jsonb");
        builder.Property(x => x.Metadata).HasColumnType("jsonb");
    }
}
