using System.Threading;
using System.Threading.Tasks;

namespace OnevoHr.Api.Services.Interfaces;

public interface IEmailOutboxProcessorService
{
    Task<int> ProcessPendingEmailsAsync(CancellationToken ct);
}
