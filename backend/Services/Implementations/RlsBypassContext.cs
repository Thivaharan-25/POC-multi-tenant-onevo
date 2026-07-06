using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

public sealed class RlsBypassContext : IRlsBypassContext
{
    public bool IsBypassEnabled { get; private set; }

    public IDisposable BeginTrustedRlsBypass()
    {
        IsBypassEnabled = true;
        return new RlsBypassScope(this);
    }

    private sealed class RlsBypassScope : IDisposable
    {
        private readonly RlsBypassContext _context;

        public RlsBypassScope(RlsBypassContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context.IsBypassEnabled = false;
        }
    }
}
