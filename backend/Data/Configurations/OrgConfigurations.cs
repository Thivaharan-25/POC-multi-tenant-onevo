using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnevoHr.Api.Models.OrgStructure;

namespace OnevoHr.Api.Data.Configurations;

public class LegalEntityConfiguration : IEntityTypeConfiguration<LegalEntity>
{
    public void Configure(EntityTypeBuilder<LegalEntity> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        builder.HasIndex(x => x.TenantId);

        builder.Property(x => x.OfficeAddressLabel).HasMaxLength(255);
        builder.Property(x => x.OfficeLatitude).HasPrecision(10, 7);
        builder.Property(x => x.OfficeLongitude).HasPrecision(10, 7);

        builder.HasMany(x => x.Departments)
            .WithOne(d => d.LegalEntity)
            .HasForeignKey(d => d.LegalEntityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Positions)
            .WithOne(p => p.LegalEntity)
            .HasForeignKey(p => p.LegalEntityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.LegalEntityId, x.Code }).IsUnique();
        builder.HasIndex(x => x.TenantId);

        builder.HasOne(x => x.ParentDepartment)
            .WithMany()
            .HasForeignKey(x => x.ParentDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.HeadPosition)
            .WithMany()
            .HasForeignKey(x => x.HeadPositionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Positions)
            .WithOne(p => p.Department)
            .HasForeignKey(p => p.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.LegalEntityId, x.Code }).IsUnique();
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.Status);

        builder.HasOne(x => x.ReportsToPosition)
            .WithMany()
            .HasForeignKey(x => x.ReportsToPositionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.PositionAssignments)
            .WithOne(a => a.Position)
            .HasForeignKey(a => a.PositionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PositionReportingHistoryConfiguration : IEntityTypeConfiguration<PositionReportingHistory>
{
    public void Configure(EntityTypeBuilder<PositionReportingHistory> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.PositionId });

        builder.HasOne(x => x.Position)
            .WithMany()
            .HasForeignKey(x => x.PositionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PositionAssignmentConfiguration : IEntityTypeConfiguration<PositionAssignment>
{
    public void Configure(EntityTypeBuilder<PositionAssignment> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.PositionId });
        builder.HasIndex(x => new { x.TenantId, x.EmployeeId });

        builder.HasOne(x => x.Employee)
            .WithMany(e => e.PositionAssignments)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class EmployeeHierarchyClosureConfiguration : IEntityTypeConfiguration<EmployeeHierarchyClosure>
{
    public void Configure(EntityTypeBuilder<EmployeeHierarchyClosure> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.ManagerEmployeeId, x.ReportEmployeeId }).IsUnique();
    }
}
