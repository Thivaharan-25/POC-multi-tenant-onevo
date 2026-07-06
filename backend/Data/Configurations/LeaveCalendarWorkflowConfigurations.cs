using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnevoHr.Api.Models.Calendar;
using OnevoHr.Api.Models.Leave;
using OnevoHr.Api.Models.Notifications;

namespace OnevoHr.Api.Data.Configurations;

public class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
{
    public void Configure(EntityTypeBuilder<LeaveType> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}

public class LeavePolicyConfiguration : IEntityTypeConfiguration<LeavePolicy>
{
    public void Configure(EntityTypeBuilder<LeavePolicy> builder)
    {
        builder.HasIndex(x => x.TenantId);

        builder.HasOne(x => x.LeaveType)
            .WithMany()
            .HasForeignKey(x => x.LeaveTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class LeavePolicyAssignmentConfiguration : IEntityTypeConfiguration<LeavePolicyAssignment>
{
    public void Configure(EntityTypeBuilder<LeavePolicyAssignment> builder)
    {
        builder.HasIndex(x => x.TenantId);

        builder.HasOne(x => x.LeavePolicy)
            .WithMany()
            .HasForeignKey(x => x.LeavePolicyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.EmployeeId });
        builder.HasIndex(x => x.Status);
        builder.Property(x => x.ConflictSnapshotJson).HasColumnType("jsonb");

        builder.HasOne(x => x.LeaveType)
            .WithMany()
            .HasForeignKey(x => x.LeaveTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class LeaveBalanceConfiguration : IEntityTypeConfiguration<LeaveBalance>
{
    public void Configure(EntityTypeBuilder<LeaveBalance> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.EmployeeId, x.LeaveTypeId, x.Year }).IsUnique();

        builder.HasOne(x => x.LeaveType)
            .WithMany()
            .HasForeignKey(x => x.LeaveTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class CalendarEventConfiguration : IEntityTypeConfiguration<CalendarEvent>
{
    public void Configure(EntityTypeBuilder<CalendarEvent> builder)
    {
        builder.HasIndex(x => x.TenantId);
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.UserId });
    }
}

public class ExternalCalendarConnectionConfiguration : IEntityTypeConfiguration<ExternalCalendarConnection>
{
    public void Configure(EntityTypeBuilder<ExternalCalendarConnection> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.UserId, x.Provider, x.ExternalCalendarId }).IsUnique();
        builder.Property(x => x.ScopesJson).HasColumnType("jsonb");

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ExternalCalendarEventLinkConfiguration : IEntityTypeConfiguration<ExternalCalendarEventLink>
{
    public void Configure(EntityTypeBuilder<ExternalCalendarEventLink> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.Provider, x.ExternalCalendarId, x.ExternalEventId }).IsUnique();

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CalendarEvent)
            .WithMany()
            .HasForeignKey(x => x.CalendarEventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ExternalCalendarConnection)
            .WithMany()
            .HasForeignKey(x => x.ExternalCalendarConnectionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
