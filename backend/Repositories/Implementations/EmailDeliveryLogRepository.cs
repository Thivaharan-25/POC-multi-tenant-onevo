using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Repositories.Interfaces;

namespace OnevoHr.Api.Repositories.Implementations;

public class EmailDeliveryLogRepository : IEmailDeliveryLogRepository
{
    private readonly AppDbContext _db;

    public EmailDeliveryLogRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(EmailDeliveryLog log)
    {
        await _db.EmailDeliveryLogs.AddAsync(log);
    }

    public async Task<List<EmailDeliveryLog>> GetQueuedEmailsAsync(int limit, CancellationToken cancellationToken)
    {
        return await _db.EmailDeliveryLogs
            .Where(e => e.Status == "queued")
            .OrderBy(e => e.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }
}
