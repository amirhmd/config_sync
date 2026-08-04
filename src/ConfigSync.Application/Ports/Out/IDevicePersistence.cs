using System.Threading;
using System.Threading.Tasks;
using ConfigSync.Domain;

namespace ConfigSync.Application.Ports.Out;

/// <summary>
/// Persistence of immutable device definitions. Devices are created and deleted, never updated.
/// </summary>
public interface IDevicePersistence
{
    /// <summary>
    /// Stores the device and its secret atomically. The secret is interpreted according to
    /// <see cref="Device.AuthenticationType"/>.
    /// Returns <see cref="CreateOutcome.AlreadyExists"/> when the name is taken.
    /// </summary>
    Task<CreateOutcome> InsertAsync(Device device, string secret, CancellationToken cancellationToken);

    /// <summary>
    /// Returns <c>null</c> when no device has the given name.
    /// </summary>
    Task<Device?> GetByNameAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Keyset pagination. Pass <c>null</c> for the first page, then the previous page's
    /// <see cref="Page{T}.NextCursor"/>. The cursor is opaque to callers.
    /// </summary>
    Task<Page<Device>> GetPageAsync(string? cursor, int limit, CancellationToken cancellationToken);

    /// <summary>
    /// Removes the device and its secret.
    /// Returns <see cref="DeleteOutcome.NotFound"/> when no device has the given name.
    /// </summary>
    Task<DeleteOutcome> DeleteAsync(string name, CancellationToken cancellationToken);
}
