using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using ConfigSync.Adapters.Tests.Fakes;

namespace ConfigSync.Adapters.Tests.In.Rest;

public class RestEndpointMappingsTests
{
    private const string ValidDeviceJson =
        """{"name":"router_01","host":"localhost","port":2201,"username":"device","password":"secret123"}""";

    private const string InvalidDeviceNameJson =
        """{"name":"Router 01","host":"localhost","port":2201,"username":"device","password":"secret123"}""";

    private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        return JsonDocument.Parse(body).RootElement;
    }

    private static async Task CreateValidDeviceAsync(DeviceEndpointTestHost host)
    {
        using var response = await host.PostJsonAsync("/v1/devices", ValidDeviceJson);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateDevice_ReturnsCreatedWithLocationAndTheStoredDevice()
    {
        // given
        await using var host = await DeviceEndpointTestHost.StartAsync();

        // when
        using var response = await host.PostJsonAsync("/v1/devices", ValidDeviceJson);

        // then
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/v1/devices/router_01", response.Headers.Location?.ToString());

        var body = await ReadJsonAsync(response);
        var device = body.GetProperty("device");
        Assert.Equal("router_01", device.GetProperty("name").GetString());
        Assert.Equal("localhost", device.GetProperty("host").GetString());
        Assert.Equal(2201, device.GetProperty("port").GetInt32());
        Assert.Equal("device", device.GetProperty("username").GetString());
        Assert.Equal("password", device.GetProperty("authenticationType").GetString());
    }

    [Fact]
    public async Task CreateDevice_DoesNotEchoTheCredential()
    {
        // given
        await using var host = await DeviceEndpointTestHost.StartAsync();

        // when
        using var response = await host.PostJsonAsync("/v1/devices", ValidDeviceJson);

        // then
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.DoesNotContain("secret123", body, System.StringComparison.Ordinal);
    }

    [Fact]
    public async Task CreateDevice_ReturnsConflictForADuplicateName()
    {
        // given
        await using var host = await DeviceEndpointTestHost.StartAsync();
        await CreateValidDeviceAsync(host);

        // when
        using var response = await host.PostJsonAsync("/v1/devices", ValidDeviceJson);

        // then
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal("AlreadyExists", body.GetProperty("outcome").GetString());
    }

    [Fact]
    public async Task CreateDevice_ReturnsBadRequestNamingTheOffendingProperty()
    {
        // given
        await using var host = await DeviceEndpointTestHost.StartAsync();

        // when
        using var response = await host.PostJsonAsync("/v1/devices", InvalidDeviceNameJson);

        // then
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errors = (await ReadJsonAsync(response))
            .GetProperty("errorDetails")
            .GetProperty("errors");
        Assert.Equal(1, errors.GetArrayLength());
        Assert.Equal("name", errors[0].GetProperty("propertyName").GetString());
    }

    [Fact]
    public async Task GetDevice_ReturnsOkWithTheStoredDevice()
    {
        // given
        await using var host = await DeviceEndpointTestHost.StartAsync();
        await CreateValidDeviceAsync(host);

        // when
        using var response = await host.GetAsync("/v1/devices/router_01");

        // then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var device = (await ReadJsonAsync(response)).GetProperty("device");
        Assert.Equal("router_01", device.GetProperty("name").GetString());
        Assert.Equal("password", device.GetProperty("authenticationType").GetString());
    }

    [Fact]
    public async Task GetDevice_ReturnsNotFoundForAnAbsentDevice()
    {
        // given
        await using var host = await DeviceEndpointTestHost.StartAsync();

        // when
        using var response = await host.GetAsync("/v1/devices/router_01");

        // then
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDevice_ReturnsBadRequestNamingTheOffendingProperty()
    {
        // given
        await using var host = await DeviceEndpointTestHost.StartAsync();

        // when
        using var response = await host.GetAsync("/v1/devices/Router%2001");

        // then
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errors = (await ReadJsonAsync(response))
            .GetProperty("errorDetails")
            .GetProperty("errors");
        Assert.Equal("name", errors[0].GetProperty("propertyName").GetString());
    }

    [Fact]
    public async Task GetDevices_ReturnsOkWithTheStoredDevices()
    {
        // given
        await using var host = await DeviceEndpointTestHost.StartAsync();
        await CreateValidDeviceAsync(host);

        // when
        using var response = await host.GetAsync("/v1/devices?limit=50");

        // then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var page = (await ReadJsonAsync(response)).GetProperty("page");
        var items = page.GetProperty("items");
        Assert.Equal(1, items.GetArrayLength());
        Assert.Equal("router_01", items[0].GetProperty("name").GetString());
    }

    [Fact]
    public async Task GetDevices_ReturnsAnEmptyPageWhenNothingIsStored()
    {
        // given
        await using var host = await DeviceEndpointTestHost.StartAsync();

        // when
        using var response = await host.GetAsync("/v1/devices?limit=50");

        // then
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var page = (await ReadJsonAsync(response)).GetProperty("page");
        Assert.Equal(0, page.GetProperty("items").GetArrayLength());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task GetDevices_ReturnsBadRequestForALimitOutOfRange(int limit)
    {
        // given
        await using var host = await DeviceEndpointTestHost.StartAsync();

        // when
        using var response = await host.GetAsync($"/v1/devices?limit={limit}");

        // then
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errors = (await ReadJsonAsync(response))
            .GetProperty("errorDetails")
            .GetProperty("errors");
        Assert.Equal("limit", errors[0].GetProperty("propertyName").GetString());
    }

    [Fact]
    public async Task DeleteDevice_ReturnsNoContentWithNoBody()
    {
        // given
        await using var host = await DeviceEndpointTestHost.StartAsync();
        await CreateValidDeviceAsync(host);

        // when
        using var response = await host.DeleteAsync("/v1/devices/router_01");

        // then
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task DeleteDevice_RemovesTheDevice()
    {
        // given
        await using var host = await DeviceEndpointTestHost.StartAsync();
        await CreateValidDeviceAsync(host);
        using var deleted = await host.DeleteAsync("/v1/devices/router_01");
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);

        // when
        using var response = await host.GetAsync("/v1/devices/router_01");

        // then
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDevice_ReturnsNotFoundForAnAbsentDevice()
    {
        // given
        await using var host = await DeviceEndpointTestHost.StartAsync();

        // when
        using var response = await host.DeleteAsync("/v1/devices/router_01");

        // then
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDevice_ReturnsBadRequestNamingTheOffendingProperty()
    {
        // given
        await using var host = await DeviceEndpointTestHost.StartAsync();

        // when
        using var response = await host.DeleteAsync("/v1/devices/Router%2001");

        // then
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errors = (await ReadJsonAsync(response))
            .GetProperty("errorDetails")
            .GetProperty("errors");
        Assert.Equal("name", errors[0].GetProperty("propertyName").GetString());
    }
    
    [Fact]
    public async Task GetDevices_ReturnsBadRequestForAnInvalidCursor()
    {
        // given
        await using var host = await DeviceEndpointTestHost.StartAsync();

        // when
        using var response = await host.GetAsync($"/v1/devices?limit=50&cursor={InMemoryDeviceUseCase.InvalidCursor}");

        // then
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errors = (await ReadJsonAsync(response))
            .GetProperty("errorDetails")
            .GetProperty("errors");

        var error = errors[0];

        Assert.Equal("cursor", error.GetProperty("propertyName").GetString());
        Assert.Equal("Cursor is invalid", error.GetProperty("message").GetString());
    }
}
