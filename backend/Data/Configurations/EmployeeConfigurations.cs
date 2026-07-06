using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnevoHr.Api.Models.Employees;

namespace OnevoHr.Api.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.EmployeeNumber }).IsUnique();
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.WorkEmail);

        builder.HasOne(x => x.LegalEntity)
            .WithMany()
            .HasForeignKey(x => x.LegalEntityId)
            .OnDelete(DeleteBehavior.Restrict);

        // EmployeeProfile is not a Phase 1 entity - no configuration needed here.
    }
}

public class EmployeeAssignmentHistoryConfiguration : IEntityTypeConfiguration<EmployeeAssignmentHistory>
{
    public void Configure(EntityTypeBuilder<EmployeeAssignmentHistory> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.EmployeeId });

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
