using System;
using System.Collections.Generic;
using System.Text;
using ConfigSync.Adapters.In.Rest.Models.Requests;
using ConfigSync.Domain;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Mappers;

public class DeviceMapperTests
{
    [Fact]
    public void ToDevice_WithPassword_SetsPasswordAuthenticationType()
    {
        // given
        var mapper = AdaptersTestConfiguration.BuildMapper(AdaptersTestConfiguration.BuildEncryption());

        // when
        var device = mapper.ToDevice(AdaptersTestConfiguration.PasswordRequest());

        // then
        Assert.Equal(DeviceAuthenticationType.Password, device.AuthenticationType);
    }

    [Fact]
    public void ToDevice_WithPrivateKey_SetsPrivateKeyAuthenticationType()
    {
        // given
        var mapper = AdaptersTestConfiguration.BuildMapper(AdaptersTestConfiguration.BuildEncryption());

        // when
        var device = mapper.ToDevice(AdaptersTestConfiguration.PrivateKeyRequest());

        // then
        Assert.Equal(DeviceAuthenticationType.PrivateKey, device.AuthenticationType);
    }

    [Fact]
    public void ToDevice_EncryptsPassword_SoCiphertextIsNotPlaintext()
    {
        // given
        var mapper = AdaptersTestConfiguration.BuildMapper(AdaptersTestConfiguration.BuildEncryption());
        var plaintext = "secret123";

        // when
        var device = mapper.ToDevice(AdaptersTestConfiguration.PasswordRequest(password: plaintext));

        // then
        Assert.NotEqual(Encoding.UTF8.GetBytes(plaintext), device.Credential.GetCiphertext());
    }

    [Fact]
    public void ToDevice_EncryptsPassword_SoItDecryptsBackToPlaintext()
    {
        // given
        var encryption = AdaptersTestConfiguration.BuildEncryption();
        var mapper = AdaptersTestConfiguration.BuildMapper(encryption);
        var plaintext = "secret123";

        // when
        var device = mapper.ToDevice(AdaptersTestConfiguration.PasswordRequest(password: plaintext));

        // then
        Assert.Equal(plaintext, encryption.DecryptPassword(device.Credential.GetCiphertext()));
    }

    [Fact]
    public void ToDevice_EncryptsPrivateKey_SoItDecryptsBackToPlaintext()
    {
        // given
        var encryption = AdaptersTestConfiguration.BuildEncryption();
        var mapper = AdaptersTestConfiguration.BuildMapper(encryption);
        var plaintext = "-----BEGIN OPENSSH PRIVATE KEY-----";

        // when
        var device = mapper.ToDevice(AdaptersTestConfiguration.PrivateKeyRequest(privateKey: plaintext));

        // then
        Assert.Equal(plaintext, encryption.DecryptPrivateKey(device.Credential.GetCiphertext()));
    }

    [Fact]
    public void ToDevice_KeepsTheNameExactlyAsSupplied()
    {
        // given
        var mapper = AdaptersTestConfiguration.BuildMapper(AdaptersTestConfiguration.BuildEncryption());

        // when
        var device = mapper.ToDevice(AdaptersTestConfiguration.PasswordRequest(name: "router_01"));

        // then
        Assert.Equal("router_01", device.Name);
    }

    [Fact]
    public void ToDevice_ThrowsWhenBothCredentialsProvided()
    {
        // given
        var mapper = AdaptersTestConfiguration.BuildMapper(AdaptersTestConfiguration.BuildEncryption());
        var request = new CreateDeviceRequest("router_01", "localhost", 2201, "device", "secret", "key");

        // when / then
        Assert.Throws<InvalidOperationException>(() => mapper.ToDevice(request));
    }

    [Fact]
    public void ToDevice_ThrowsWhenNeitherCredentialProvided()
    {
        // given
        var mapper = AdaptersTestConfiguration.BuildMapper(AdaptersTestConfiguration.BuildEncryption());
        var request = new CreateDeviceRequest("router_01", "localhost", 2201, "device", null, null);

        // when / then
        Assert.Throws<InvalidOperationException>(() => mapper.ToDevice(request));
    }

    [Fact]
    public void ToDetails_MapsAllFieldsWithPasswordWireValue()
    {
        // given
        var mapper = AdaptersTestConfiguration.BuildMapper(AdaptersTestConfiguration.BuildEncryption());
        var device = mapper.ToDevice(AdaptersTestConfiguration.PasswordRequest());

        // when
        var details = mapper.ToDetails(device);

        // then
        Assert.Equal("router_01", details.Name);
        Assert.Equal("localhost", details.Host);
        Assert.Equal(2201, details.Port);
        Assert.Equal("device", details.Username);
        Assert.Equal("password", details.AuthenticationType);
    }

    [Fact]
    public void ToDetails_MapsPrivateKeyWireValue()
    {
        // given
        var mapper = AdaptersTestConfiguration.BuildMapper(AdaptersTestConfiguration.BuildEncryption());
        var device = mapper.ToDevice(AdaptersTestConfiguration.PrivateKeyRequest());

        // when
        var details = mapper.ToDetails(device);

        // then
        Assert.Equal("private_key", details.AuthenticationType);
    }

    [Fact]
    public void ToPageDetails_MapsItemsInOrderAndCarriesCursor()
    {
        // given
        var mapper = AdaptersTestConfiguration.BuildMapper(AdaptersTestConfiguration.BuildEncryption());
        var first = mapper.ToDevice(AdaptersTestConfiguration.PasswordRequest(name: "router_01"));
        var second = mapper.ToDevice(AdaptersTestConfiguration.PasswordRequest(name: "router_02"));
        var page = new Page<Device>(new List<Device> { first, second }, "next-cursor");

        // when
        var details = mapper.ToPageDetails(page);

        // then
        Assert.Equal(2, details.Items.Count);
        Assert.Equal("router_01", details.Items[0].Name);
        Assert.Equal("router_02", details.Items[1].Name);
        Assert.Equal("next-cursor", details.NextCursor);
    }

    [Fact]
    public void ToPageDetails_WithEmptyPage_ReturnsEmptyItemsAndNullCursor()
    {
        // given
        var mapper = AdaptersTestConfiguration.BuildMapper(AdaptersTestConfiguration.BuildEncryption());
        var page = new Page<Device>(new List<Device>(), null);

        // when
        var details = mapper.ToPageDetails(page);

        // then
        Assert.Empty(details.Items);
        Assert.Null(details.NextCursor);
    }
}
