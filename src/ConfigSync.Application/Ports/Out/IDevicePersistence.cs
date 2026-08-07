using System.Threading;
using System.Threading.Tasks;
using ConfigSync.Domain;

namespace ConfigSync.Application.Ports.Out;

/// <summary>
/// Persistence of immutable device definitions. Devices are created and deleted, never updated.
/// Credentials are stored and returned encrypted; decryption is an explicit, separate step.
/// </summary>
public interface IDevicePersistence
{
    /// <summary>
    /// Stores the device and its credential atomically.
    /// Returns <see cref="CreateOutcome.AlreadyExists"/> when the name is taken.
    /// </summary>
    Task<CreateOutcome> InsertAsync(Device device, CancellationToken cancellationToken);

    /// <summary>
    /// Returns <c>null</c> when no device has the given name.
    /// </summary>
    Task<Device?> GetByNameAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Keyset pagination. Pass <c>null</c> for the first page, then the previous page's
    /// <see cref="Page{T}.NextCursor"/>. The cursor is opaque to callers.
    /// Returns <see cref="DevicePageOutcome.InvalidCursor"/> when the cursor cannot be decoded.
    /// </summary>
    Task<DevicePageOutcome> GetPageAsync(string? cursor, int limit, CancellationToken cancellationToken);

    /// <summary>
    /// Removes the device and its credential.
    /// Returns <see cref="DeleteOutcome.NotFound"/> when no device has the given name.
    /// </summary>
    Task<DeleteOutcome> DeleteAsync(string name, CancellationToken cancellationToken);
}
