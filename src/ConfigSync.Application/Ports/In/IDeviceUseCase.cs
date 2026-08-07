using System.Threading;
using System.Threading.Tasks;
using ConfigSync.Domain;

namespace ConfigSync.Application.Ports.In;

/// <summary>
/// Device definition lifecycle. Definitions are created and deleted, never updated.
/// Inputs are expected to be validated by the calling adapter.
/// </summary>
public interface IDeviceUseCase
{
    Task<CreateOutcome> CreateAsync(Device device, CancellationToken cancellationToken);

    /// <summary>
    /// Returns <c>null</c> when no device has the given name.
    /// </summary>
    Task<Device?> GetByNameAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Keyset pagination. Pass <c>null</c> as the cursor for the first page.
    /// </summary>
    Task<DevicePageOutcome> GetPageAsync(string? cursor, int limit, CancellationToken cancellationToken);

    Task<DeleteOutcome> DeleteAsync(string name, CancellationToken cancellationToken);
}
