using System.Threading.Tasks;
using ConfigSync.Application.Ports.In;
using ConfigSync.Application.Services;
using ConfigSync.Application.Tests.Fakes;
using ConfigSync.Domain;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ConfigSync.Application.Tests.Services;

public class DeviceServiceTests
{
    private static IDeviceUseCase BuildService() =>
        new DeviceService(new InMemoryDevicePersistence(), NullLogger<DeviceService>.Instance);

    private static Device BuildDevice(string name) =>
        new(name, "localhost", 2201, "device", DeviceCredential.Password([0x01, 0x02]));

    [Fact]
    public async Task CreateAsync_ReturnsCreated_ForANewDevice()
    {
        // given
        var service = BuildService();

        // when
        var outcome = await service.CreateAsync(
            BuildDevice("router_01"), TestContext.Current.CancellationToken);

        // then
        Assert.Equal(CreateOutcome.Created, outcome);
    }

    [Fact]
    public async Task CreateAsync_ReturnsAlreadyExists_ForADuplicateName()
    {
        // given
        var service = BuildService();
        await service.CreateAsync(BuildDevice("router_01"), TestContext.Current.CancellationToken);

        // when
        var outcome = await service.CreateAsync(
            BuildDevice("router_01"), TestContext.Current.CancellationToken);

        // then
        Assert.Equal(CreateOutcome.AlreadyExists, outcome);
    }

    [Fact]
    public async Task GetByNameAsync_ReturnsTheStoredDevice()
    {
        // given
        var service = BuildService();
        await service.CreateAsync(BuildDevice("router_01"), TestContext.Current.CancellationToken);

        // when
        var device = await service.GetByNameAsync("router_01", TestContext.Current.CancellationToken);

        // then
        Assert.NotNull(device);
        Assert.Equal("router_01", device.Name);
    }

    [Fact]
    public async Task GetByNameAsync_ReturnsNull_WhenTheDeviceIsAbsent()
    {
        // given
        var service = BuildService();

        // when
        var device = await service.GetByNameAsync("absent", TestContext.Current.CancellationToken);

        // then
        Assert.Null(device);
    }

    [Fact]
    public async Task GetPageAsync_ReturnsACursor_WhenMoreDevicesRemain()
    {
        // given
        var service = BuildService();
        await service.CreateAsync(BuildDevice("router_01"), TestContext.Current.CancellationToken);
        await service.CreateAsync(BuildDevice("router_02"), TestContext.Current.CancellationToken);
        await service.CreateAsync(BuildDevice("router_03"), TestContext.Current.CancellationToken);

        // when
        var page = await service.GetPageAsync(null, 2, TestContext.Current.CancellationToken);

        // then
        Assert.Equal(2, page.Items.Count);
        Assert.NotNull(page.NextCursor);
    }

    [Fact]
    public async Task GetPageAsync_ReturnsNoCursor_OnTheLastPage()
    {
        // given
        var service = BuildService();
        await service.CreateAsync(BuildDevice("router_01"), TestContext.Current.CancellationToken);

        // when
        var page = await service.GetPageAsync(null, 10, TestContext.Current.CancellationToken);

        // then
        Assert.Single(page.Items);
        Assert.Null(page.NextCursor);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsDeleted_ForAStoredDevice()
    {
        // given
        var service = BuildService();
        await service.CreateAsync(BuildDevice("router_01"), TestContext.Current.CancellationToken);

        // when
        var outcome = await service.DeleteAsync("router_01", TestContext.Current.CancellationToken);

        // then
        Assert.Equal(DeleteOutcome.Deleted, outcome);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_ForAnAbsentDevice()
    {
        // given
        var service = BuildService();

        // when
        var outcome = await service.DeleteAsync("absent", TestContext.Current.CancellationToken);

        // then
        Assert.Equal(DeleteOutcome.NotFound, outcome);
    }
}
