using System.Threading.Tasks;
using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Adapters.In.Rest.Models.Responses;
using ConfigSync.Adapters.Tests.Fakes;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Endpoints;

public class DeviceEndpointTests
{
    [Fact]
    public async Task Create_ReturnsCreatedWithTheDevice()
    {
        // given
        var useCase = new InMemoryDeviceUseCase();
        var endpoint = AdaptersTestConfiguration.BuildEndpoint(useCase);

        // when
        var response = await endpoint.Create(AdaptersTestConfiguration.PasswordRequest(), TestContext.Current.CancellationToken);

        // then
        Assert.Equal(CreateDeviceOutcome.Created, response.Outcome);
        Assert.NotNull(response.Device);
        Assert.Equal("router_01", response.Device.Name);
        Assert.Null(response.ErrorDetails);
    }

    [Fact]
    public async Task Create_ReturnsAlreadyExistsForADuplicateName()
    {
        // given
        var useCase = new InMemoryDeviceUseCase();
        var endpoint = AdaptersTestConfiguration.BuildEndpoint(useCase);
        await endpoint.Create(AdaptersTestConfiguration.PasswordRequest(), TestContext.Current.CancellationToken);

        // when
        var response = await endpoint.Create(AdaptersTestConfiguration.PasswordRequest(), TestContext.Current.CancellationToken);

        // then
        Assert.Equal(CreateDeviceOutcome.AlreadyExists, response.Outcome);
        Assert.Null(response.Device);
    }

    [Fact]
    public async Task Create_ReturnsInvalidWithoutCallingTheUseCase()
    {
        // given
        var useCase = new InMemoryDeviceUseCase();
        var endpoint = AdaptersTestConfiguration.BuildEndpoint(useCase);
        var request = new CreateDeviceRequest("Router 01", "localhost", 2201, "device", "secret", null);

        // when
        var response = await endpoint.Create(request, TestContext.Current.CancellationToken);

        // then
        Assert.NotNull(response.ErrorDetails);
        Assert.Null(response.Outcome);
        Assert.Null(await useCase.GetByNameAsync("Router 01", TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Create_ReportsEveryInvalidFieldAtOnce()
    {
        // given
        var useCase = new InMemoryDeviceUseCase();
        var endpoint = AdaptersTestConfiguration.BuildEndpoint(useCase);
        var request = new CreateDeviceRequest("Router 01", "ssh://host", 0, "   ", null, null);

        // when
        var response = await endpoint.Create(request, TestContext.Current.CancellationToken);

        // then
        Assert.NotNull(response.ErrorDetails);
        Assert.Equal(5, response.ErrorDetails.Errors.Count);
    }

    [Fact]
    public async Task Get_ReturnsTheStoredDevice()
    {
        // given
        var useCase = new InMemoryDeviceUseCase();
        var endpoint = AdaptersTestConfiguration.BuildEndpoint(useCase);
        await endpoint.Create(AdaptersTestConfiguration.PasswordRequest(), TestContext.Current.CancellationToken);

        // when
        var response = await endpoint.Get(new GetDeviceRequest("router_01"), TestContext.Current.CancellationToken);

        // then
        Assert.NotNull(response.Device);
        Assert.Equal("router_01", response.Device.Name);
    }

    [Fact]
    public async Task Get_ReturnsNotFoundForAnAbsentDevice()
    {
        // given
        var useCase = new InMemoryDeviceUseCase();
        var endpoint = AdaptersTestConfiguration.BuildEndpoint(useCase);

        // when
        var response = await endpoint.Get(new GetDeviceRequest("router_01"), TestContext.Current.CancellationToken);

        // then
        Assert.Null(response.Device);
        Assert.Null(response.ErrorDetails);
    }

    [Fact]
    public async Task Get_ReturnsInvalidForAMalformedName()
    {
        // given
        var useCase = new InMemoryDeviceUseCase();
        var endpoint = AdaptersTestConfiguration.BuildEndpoint(useCase);

        // when
        var response = await endpoint.Get(new GetDeviceRequest("Router 01"), TestContext.Current.CancellationToken);

        // then
        Assert.NotNull(response.ErrorDetails);
        Assert.Null(response.Device);
    }

    [Fact]
    public async Task GetPage_ReturnsTheStoredDevices()
    {
        // given
        var useCase = new InMemoryDeviceUseCase();
        var endpoint = AdaptersTestConfiguration.BuildEndpoint(useCase);
        await endpoint.Create(AdaptersTestConfiguration.PasswordRequest(name: "router_01"), TestContext.Current.CancellationToken);
        await endpoint.Create(AdaptersTestConfiguration.PasswordRequest(name: "router_02"), TestContext.Current.CancellationToken);

        // when
        var response = await endpoint.GetPage(new GetDevicesRequest(null, 50), TestContext.Current.CancellationToken);

        // then
        Assert.NotNull(response.Page);
        Assert.Equal(2, response.Page.Items.Count);
    }

    [Fact]
    public async Task GetPage_ReturnsInvalidForALimitOutOfRange()
    {
        // given
        var useCase = new InMemoryDeviceUseCase();
        var endpoint = AdaptersTestConfiguration.BuildEndpoint(useCase);

        // when
        var response = await endpoint.GetPage(new GetDevicesRequest(null, 101), TestContext.Current.CancellationToken);

        // then
        Assert.NotNull(response.ErrorDetails);
        Assert.Null(response.Page);
    }

    [Fact]
    public async Task Delete_ReturnsDeletedForAStoredDevice()
    {
        // given
        var useCase = new InMemoryDeviceUseCase();
        var endpoint = AdaptersTestConfiguration.BuildEndpoint(useCase);
        await endpoint.Create(AdaptersTestConfiguration.PasswordRequest(), TestContext.Current.CancellationToken);

        // when
        var response = await endpoint.Delete(new DeleteDeviceRequest("router_01"), TestContext.Current.CancellationToken);

        // then
        Assert.Equal(DeleteDeviceOutcome.Deleted, response.Outcome);
    }

    [Fact]
    public async Task Delete_ReturnsNotFoundForAnAbsentDevice()
    {
        // given
        var useCase = new InMemoryDeviceUseCase();
        var endpoint = AdaptersTestConfiguration.BuildEndpoint(useCase);

        // when
        var response = await endpoint.Delete(new DeleteDeviceRequest("router_01"), TestContext.Current.CancellationToken);

        // then
        Assert.Equal(DeleteDeviceOutcome.NotFound, response.Outcome);
    }

    [Fact]
    public async Task Delete_ReturnsInvalidForAMalformedName()
    {
        // given
        var useCase = new InMemoryDeviceUseCase();
        var endpoint = AdaptersTestConfiguration.BuildEndpoint(useCase);

        // when
        var response = await endpoint.Delete(new DeleteDeviceRequest("Router 01"), TestContext.Current.CancellationToken);

        // then
        Assert.NotNull(response.ErrorDetails);
        Assert.Null(response.Outcome);
    }
    
    [Fact]
    public async Task GetPage_ReturnsInvalidForAnInvalidCursor()
    {
        // given
        var useCase = new InMemoryDeviceUseCase();
        var endpoint = AdaptersTestConfiguration.BuildEndpoint(useCase);
        var request = new GetDevicesRequest(InMemoryDeviceUseCase.InvalidCursor, 50);

        // when
        var response = await endpoint.GetPage(request, TestContext.Current.CancellationToken);

        // then
        Assert.Null(response.Page);
        Assert.NotNull(response.ErrorDetails);
        var error = Assert.Single(response.ErrorDetails.Errors);
        Assert.Equal("cursor", error.PropertyName);
        Assert.Equal("Cursor is invalid", error.Message);
    }
}
