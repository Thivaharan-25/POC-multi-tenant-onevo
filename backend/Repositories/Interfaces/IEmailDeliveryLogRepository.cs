using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OnevoHr.Api.Models.Generated;

namespace OnevoHr.Api.Repositories.Interfaces;

public interface IEmailDeliveryLogRepository
{
    Task AddAsync(EmailDeliveryLog log);
    Task<List<EmailDeliveryLog>> GetQueuedEmailsAsync(int limit, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
