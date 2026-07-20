using System.Text.Json;
using OnevoHr.Api.DTOs.Agents;
using OnevoHr.Api.Models.Agents;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

public class AgentActivityService : IAgentActivityService
{
    private const string DefaultApplicationCategory = "Uncategorized";
    private const string DefaultAppCategoryType = "unknown";

    private readonly IAgentActivityRepository _repository;

    public AgentActivityService(IAgentActivityRepository repository)
    {
        _repository = repository;
    }

    public async Task<ClockStateResponseDto?> ClockInAsync(Guid registeredAgentId, Guid tenantId, Guid? employeeId)
    {
        var state = await _repository.GetClockStateAsync(registeredAgentId);
        if (state is not null && state.IsClockedIn)
        {
            return null;
        }

        var now = DateTimeOffset.UtcNow;

        if (state is null)
        {
            state = new AgentClockState
            {
                Id = Guid.NewGuid(),
                RegisteredAgentId = registeredAgentId,
                TenantId = tenantId,
                EmployeeId = employeeId,
                IsClockedIn = true,
                ClockedInAt = now,
                ClockedOutAt = null
            };
            await _repository.AddClockStateAsync(state);
        }
        else
        {
            state.IsClockedIn = true;
            state.ClockedInAt = now;
            state.ClockedOutAt = null;
        }

        await _repository.SaveChangesAsync();

        return new ClockStateResponseDto(state.IsClockedIn, state.ClockedInAt, state.ClockedOutAt);
    }

    public async Task<ClockStateResponseDto?> ClockOutAsync(Guid registeredAgentId)
    {
        var state = await _repository.GetClockStateAsync(registeredAgentId);
        if (state is null || !state.IsClockedIn)
        {
            return null;
        }

        state.IsClockedIn = false;
        state.ClockedOutAt = DateTimeOffset.UtcNow;
        await _repository.SaveChangesAsync();

        return new ClockStateResponseDto(state.IsClockedIn, state.ClockedInAt, state.ClockedOutAt);
    }

    public async Task<bool> SubmitAppUsageAsync(Guid registeredAgentId, Guid tenantId, Guid? employeeId, IReadOnlyList<AppUsageSampleDto> samples)
    {
        if (employeeId is null)
        {
            return false;
        }

        var state = await _repository.GetClockStateAsync(registeredAgentId);
        if (state is null || !state.IsClockedIn)
        {
            return false;
        }

        foreach (var sample in samples)
        {
            await UpsertApplicationUsageAsync(tenantId, employeeId.Value, sample);
        }

        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> IngestAsync(Guid registeredAgentId, Guid tenantId, Guid? employeeId, IngestRequestDto request)
    {
        if (employeeId is null)
        {
            return false;
        }

        var state = await _repository.GetClockStateAsync(registeredAgentId);
        if (state is null || !state.IsClockedIn)
        {
            return false;
        }

        foreach (var item in request.Batch)
        {
            // Only app_usage is handled today; other documented batch types
            // (meeting, screenshot_capture, ...) are accepted but ignored.
            if (!string.Equals(item.Type, "app_usage", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            AppUsageDataDto? data;
            try
            {
                data = item.Data.Deserialize<AppUsageDataDto>();
            }
            catch (JsonException)
            {
                continue;
            }

            if (data is null)
            {
                continue;
            }

            var sample = new AppUsageSampleDto(
                data.ApplicationName,
                data.ProcessName,
                data.WindowTitleHash,
                data.Date,
                data.Seconds);

            await UpsertApplicationUsageAsync(tenantId, employeeId.Value, sample);
        }

        await _repository.SaveChangesAsync();

        return true;
    }

    private async Task UpsertApplicationUsageAsync(Guid tenantId, Guid employeeId, AppUsageSampleDto sample)
    {
        var existing = await _repository.GetApplicationUsageAsync(tenantId, employeeId, sample.Date, sample.ApplicationName);

        if (existing is not null)
        {
            existing.TotalSeconds += sample.Seconds;
            existing.WindowTitleHash = sample.WindowTitleHash;
        }
        else
        {
            var usage = new ApplicationUsage
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                EmployeeId = employeeId,
                Date = sample.Date,
                ApplicationName = sample.ApplicationName,
                ProcessName = sample.ProcessName,
                ApplicationCategory = DefaultApplicationCategory,
                AppCategoryType = DefaultAppCategoryType,
                WindowTitleHash = sample.WindowTitleHash,
                TotalSeconds = sample.Seconds
            };
            await _repository.AddApplicationUsageAsync(usage);
        }
    }
}
