using System.Threading;
using System.Threading.Tasks;
using ConfigSync.Application.Ports.Out;
using ConfigSync.Domain;

namespace ConfigSync.Adapters.Out.Persistence.Postgres;

internal sealed class DeviceRepository : IDevicePersistence
{
    public Task<CreateOutcome> InsertAsync(Device device, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<Device?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<Page<Device>> GetPageAsync(string? cursor, int limit, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<DeleteOutcome> DeleteAsync(string name, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}