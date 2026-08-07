using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConfigSync.Application.Ports.Out;
using ConfigSync.Domain;

namespace ConfigSync.Application.Tests.Fakes;

internal sealed class InMemoryDevicePersistence : IDevicePersistence
{
    internal const string InvalidCursor = "invalid-cursor";
    private readonly List<Device> _devices = new();

    public Task<CreateOutcome> InsertAsync(Device device, CancellationToken cancellationToken)
    {
        if (Contains(device.Name))
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

    public Task<DevicePageOutcome> GetPageAsync(string? cursor, int limit, CancellationToken cancellationToken)
    {
        if (cursor == InvalidCursor)
        {
            return Task.FromResult(DevicePageOutcome.InvalidCursor());
        }

        var candidates = DevicesAfterCursor(cursor);
        var items = candidates.Take(limit).ToList();
        var nextCursor = NextCursor(candidates, items, limit);

        var page = new Page<Device>(items, nextCursor);

        return Task.FromResult(DevicePageOutcome.Success(page));
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

    private bool Contains(string name)
    {
        return Find(name) is not null;
    }

    private Device? Find(string name)
    {
        return _devices.SingleOrDefault(device => device.Name == name);
    }

    private List<Device> DevicesAfterCursor(string? cursor)
    {
        var orderedDevices = _devices.OrderBy(device => device.Name).ToList();

        if (cursor is null)
        {
            return orderedDevices;
        }

        return orderedDevices.Where(device => IsAfter(device.Name, cursor)).ToList();
    }

    private static bool IsAfter(string name, string cursor)
    {
        return string.CompareOrdinal(name, cursor) > 0;
    }

    private static string? NextCursor(List<Device> candidates, List<Device> items, int limit)
    {
        var moreDevicesRemain = candidates.Count > limit;

        if (!moreDevicesRemain)
        {
            return null;
        }

        var lastItemOnThisPage = items.Last();

        return lastItemOnThisPage.Name;
    }
}
