using System.Threading;
using System.Threading.Tasks;
using ConfigSync.Application.Ports.In;
using ConfigSync.Application.Ports.Out;
using ConfigSync.Domain;
using Microsoft.Extensions.Logging;

namespace ConfigSync.Application.Services;

public sealed class DeviceService(IDevicePersistence persistence, ILogger<DeviceService> logger) : IDeviceUseCase
{
    public async Task<CreateOutcome> CreateAsync(Device device, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating device {DeviceName}", device.Name);

        var outcome = await persistence.InsertAsync(device, cancellationToken);

        logger.LogInformation("Create device {DeviceName} completed with outcome {Outcome}", device.Name, outcome.Value);

        return outcome;
    }

    public async Task<Device?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        logger.LogInformation("Reading device {DeviceName}", name);

        var device = await persistence.GetByNameAsync(name, cancellationToken);

        logger.LogInformation("Read device {DeviceName} completed, found {Found}", name, device is not null);

        return device;
    }

    public async Task<Page<Device>> GetPageAsync(string? cursor, int limit, CancellationToken cancellationToken)
    {
        logger.LogInformation("Reading device page with limit {Limit}", limit);

        var page = await persistence.GetPageAsync(cursor, limit, cancellationToken);

        logger.LogInformation("Read device page returned {ItemCount} devices, more available {HasMore}",
            page.Items.Count,
            page.NextCursor is not null);

        return page;
    }

    public async Task<DeleteOutcome> DeleteAsync(string name, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting device {DeviceName}", name);

        var outcome = await persistence.DeleteAsync(name, cancellationToken);

        logger.LogInformation("Delete device {DeviceName} completed with outcome {Outcome}", name, outcome.Value);

        return outcome;
    }
}
