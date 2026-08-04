using System;
using System.Collections.Generic;
using ConfigSync.Adapters.In.Rest.Mappers;
using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Domain;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Mappers;

public class DeviceMapperTests
{
    private static readonly IDeviceMapper Mapper = new DeviceMapper();

    [Fact]
    public void ToDevice_WithPassword_BuildsDeviceWithPasswordCredential()
    {
        // given
        var request = new CreateDeviceRequest(
            Name: "router_01", Host: "10.0.0.1", Port: 22, Username: "admin",
            Password: "s3cr3t", PrivateKey: null);

        // when
        var device = Mapper.ToDevice(request);

        // then
        Assert.Equal("router_01", device.Name);
        Assert.Equal("10.0.0.1", device.Host);
        Assert.Equal(22, device.Port);
        Assert.Equal("admin", device.Username);
        Assert.Equal(DeviceAuthenticationType.Password, device.AuthenticationType);
        Assert.Equal("s3cr3t", device.Credential.GetPlaintext());
    }

    [Fact]
    public void ToDevice_WithPrivateKey_BuildsDeviceWithPrivateKeyCredential()
    {
        // given
        var request = new CreateDeviceRequest(
            Name: "router_01", Host: "10.0.0.1", Port: 22, Username: "admin",
            Password: null, PrivateKey: "-----BEGIN KEY-----");

        // when
        var device = Mapper.ToDevice(request);

        // then
        Assert.Equal(DeviceAuthenticationType.PrivateKey, device.AuthenticationType);
        Assert.Equal("-----BEGIN KEY-----", device.Credential.GetPlaintext());
    }

    [Fact]
    public void ToDevice_NormalizesName_TrimsAndLowercases()
    {
        // given
        var request = new CreateDeviceRequest(
            Name: "  Router_01  ", Host: "10.0.0.1", Port: 22, Username: "admin",
            Password: "s3cr3t", PrivateKey: null);

        // when
        var device = Mapper.ToDevice(request);

        // then
        Assert.Equal("router_01", device.Name);
    }

    [Fact]
    public void ToDevice_WithNeitherCredential_Throws()
    {
        // given
        var request = new CreateDeviceRequest(
            Name: "router_01", Host: "10.0.0.1", Port: 22, Username: "admin",
            Password: null, PrivateKey: null);

        // when / then
        Assert.Throws<InvalidOperationException>(() => Mapper.ToDevice(request));
    }

    [Fact]
    public void ToDevice_WithBothCredentials_Throws()
    {
        // given
        var request = new CreateDeviceRequest(
            Name: "router_01", Host: "10.0.0.1", Port: 22, Username: "admin",
            Password: "s3cr3t", PrivateKey: "-----BEGIN KEY-----");

        // when / then
        Assert.Throws<InvalidOperationException>(() => Mapper.ToDevice(request));
    }

    [Fact]
    public void ToDetails_MapsAllFieldsAndPasswordWireValue()
    {
        // given
        var device = new Device(
            "router_01", "10.0.0.1", 22, "admin", DeviceCredential.Password("s3cr3t"));

        // when
        var details = Mapper.ToDetails(device);

        // then
        Assert.Equal("router_01", details.Name);
        Assert.Equal("10.0.0.1", details.Host);
        Assert.Equal(22, details.Port);
        Assert.Equal("admin", details.Username);
        Assert.Equal("password", details.AuthenticationType);
    }

    [Fact]
    public void ToDetails_MapsPrivateKeyWireValue()
    {
        // given
        var device = new Device(
            "router_01", "10.0.0.1", 22, "admin", DeviceCredential.PrivateKey("key"));

        // when
        var details = Mapper.ToDetails(device);

        // then
        Assert.Equal("private_key", details.AuthenticationType);
    }

    [Fact]
    public void ToDetails_DoesNotExposeSecret()
    {
        // given
        const string secret = "super-secret-value";
        var device = new Device(
            "router_01", "10.0.0.1", 22, "admin", DeviceCredential.Password(secret));

        // when
        var details = Mapper.ToDetails(device);

        // then
        Assert.DoesNotContain(
            secret,
            new[] { details.Name, details.Host, details.Username, details.AuthenticationType });
    }

    [Fact]
    public void ToPageDetails_MapsItemsInOrderAndCarriesCursor()
    {
        // given
        var page = new Page<DeviceSummary>(
            new List<DeviceSummary>
            {
                new("router_01", "10.0.0.1", 22, "admin", DeviceAuthenticationType.Password),
                new("router_02", "10.0.0.2", 22, "admin", DeviceAuthenticationType.PrivateKey)
            },
            NextCursor: "cursor-123");

        // when
        var details = Mapper.ToPageDetails(page);

        // then
        Assert.Equal("cursor-123", details.NextCursor);
        Assert.Equal(2, details.Items.Count);
        Assert.Equal("router_01", details.Items[0].Name);
        Assert.Equal("password", details.Items[0].AuthenticationType);
        Assert.Equal("router_02", details.Items[1].Name);
        Assert.Equal("private_key", details.Items[1].AuthenticationType);
    }

    [Fact]
    public void ToPageDetails_WithEmptyPage_ReturnsEmptyItemsAndNullCursor()
    {
        // given
        var page = new Page<DeviceSummary>(new List<DeviceSummary>(), NextCursor: null);

        // when
        var details = Mapper.ToPageDetails(page);

        // then
        Assert.Empty(details.Items);
        Assert.Null(details.NextCursor);
    }

    [Fact]
    public void ToPageDetails_WithUnknownAuthenticationType_Throws()
    {
        // given
        var page = new Page<DeviceSummary>(
            new List<DeviceSummary>
            {
                new("router_01", "10.0.0.1", 22, "admin", (DeviceAuthenticationType)999)
            },
            NextCursor: null);

        // when / then
        Assert.Throws<ArgumentOutOfRangeException>(() => Mapper.ToPageDetails(page));
    }
}
