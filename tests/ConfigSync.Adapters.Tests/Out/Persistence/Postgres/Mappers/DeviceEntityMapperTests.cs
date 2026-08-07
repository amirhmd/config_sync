using System;
using ConfigSync.Adapters.Out.Persistence.Postgres.Mappers;
using ConfigSync.Domain;
using Xunit;

namespace ConfigSync.Adapters.Tests.Out.Persistence.Postgres.Mappers;

public class DeviceEntityMapperTests
{
    private static readonly byte[] Ciphertext = [0x01, 0x02, 0x03];

    [Fact]
    public void ToDomain_MapsEveryScalarField()
    {
        // given
        var mapper = new DeviceEntityMapper();
        var entity = AdaptersTestConfiguration.DeviceRow(passwordEncrypted: Ciphertext);

        // when
        var device = mapper.ToDomain(entity);

        // then
        Assert.Equal("router_01", device.Name);
        Assert.Equal("localhost", device.Host);
        Assert.Equal(2201, device.Port);
        Assert.Equal("device", device.Username);
    }

    [Fact]
    public void ToDomain_WithPasswordColumn_BuildsAPasswordCredential()
    {
        // given
        var mapper = new DeviceEntityMapper();
        var entity = AdaptersTestConfiguration.DeviceRow(passwordEncrypted: Ciphertext);

        // when
        var device = mapper.ToDomain(entity);

        // then
        Assert.Equal(DeviceAuthenticationType.Password, device.AuthenticationType);
        Assert.Equal(Ciphertext, device.Credential.GetCiphertext());
    }

    [Fact]
    public void ToDomain_WithPrivateKeyColumn_BuildsAPrivateKeyCredential()
    {
        // given
        var mapper = new DeviceEntityMapper();
        var entity = AdaptersTestConfiguration.DeviceRow(privateKeyEncrypted: Ciphertext);

        // when
        var device = mapper.ToDomain(entity);

        // then
        Assert.Equal(DeviceAuthenticationType.PrivateKey, device.AuthenticationType);
        Assert.Equal(Ciphertext, device.Credential.GetCiphertext());
    }

    [Fact]
    public void ToDomain_ThrowsWhenBothCredentialColumnsArePopulated()
    {
        // given
        var mapper = new DeviceEntityMapper();
        var entity = AdaptersTestConfiguration.DeviceRow(
            passwordEncrypted: Ciphertext,
            privateKeyEncrypted: Ciphertext);

        // when / then
        Assert.Throws<InvalidOperationException>(() => mapper.ToDomain(entity));
    }

    [Fact]
    public void ToDomain_ThrowsWhenNeitherCredentialColumnIsPopulated()
    {
        // given
        var mapper = new DeviceEntityMapper();
        var entity = AdaptersTestConfiguration.DeviceRow();

        // when / then
        Assert.Throws<InvalidOperationException>(() => mapper.ToDomain(entity));
    }
}
