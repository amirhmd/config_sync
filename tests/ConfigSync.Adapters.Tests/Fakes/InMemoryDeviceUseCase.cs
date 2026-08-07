using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConfigSync.Application.Ports.In;
using ConfigSync.Domain;

namespace ConfigSync.Adapters.Tests.Fakes;

internal sealed class InMemoryDeviceUseCase : IDeviceUseCase
{
    internal const string InvalidCursor = "invalid-cursor";

    private readonly List<Device> _devices = new List<Device>();

    public Task<CreateOutcome> CreateAsync(Device device, CancellationToken cancellationToken)
    {
        if (Find(device.Name) is not null)
        {
            return Task.FromResult(CreateOutcome.AlreadyExists);
        }

        _devices.Add(device);

        return Task.FromResult(CreateOutcome.Created);
    }

    public Task<Device?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return Task.FromResult(Find(name));
    }

    public Task<DevicePageOutcome> GetPageAsync(
        string? cursor,
        int limit,
        CancellationToken cancellationToken)
    {
        if (cursor == InvalidCursor)
        {
            return Task.FromResult<DevicePageOutcome>(DevicePageInvalidCursor.Instance);
        }

        var items = _devices.Take(limit).ToList();

        return Task.FromResult<DevicePageOutcome>(
            new DevicePageSuccess(new Page<Device>(items, null)));
    }

    public Task<DeleteOutcome> DeleteAsync(string name, CancellationToken cancellationToken)
    {
        var device = Find(name);

        if (device is null)
        {
            return Task.FromResult(DeleteOutcome.NotFound);
        }

        _devices.Remove(device);

        return Task.FromResult(DeleteOutcome.Deleted);
    }

    private Device? Find(string name)
    {
        return _devices.SingleOrDefault(device => device.Name == name);
    }
}
