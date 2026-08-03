using System;
using ConfigSync.Adapters.Out.Persistence.Postgres.Entities;
using Xunit;

namespace ConfigSync.Adapters.Tests.Out.Persistence.Postgres.Entities;

public class DeviceEntityTests
{
    [Fact]
    public void Construction_WithPrivateKeyCredential_SetsAllProperties()
    {
        // given
        var createdAt = DateTimeOffset.UtcNow;
        var privateKeyBytes = new byte[] { 0x01, 0x02, 0x03 };

        // when
        var given = new DeviceEntity
        {
            Id = "device123",
            Host = "localhost",
            Port = 2201,
            Username = "device",
            AuthenticationType = "private_key",
            PasswordEncrypted = null,
            PrivateKeyEncrypted = privateKeyBytes,
            Version = 1,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };

        // then
        Assert.Equal("device123", given.Id);
        Assert.Equal("localhost", given.Host);
        Assert.Equal(2201, given.Port);
        Assert.Equal("device", given.Username);
        Assert.Equal("private_key", given.AuthenticationType);
        Assert.Null(given.PasswordEncrypted);
        Assert.Equal(privateKeyBytes, given.PrivateKeyEncrypted);
        Assert.Equal(1, given.Version);
        Assert.Equal(createdAt, given.CreatedAt);
        Assert.Equal(createdAt, given.UpdatedAt);
    }

    [Fact]
    public void Construction_WithPasswordCredential_SetsAllProperties()
    {
        // given
        var createdAt = DateTimeOffset.UtcNow;
        var passwordBytes = new byte[] { 0x0A, 0x0B, 0x0C };

        // when
        var given = new DeviceEntity
        {
            Id = "device456",
            Host = "10.0.0.1",
            Port = 22,
            Username = "admin",
            AuthenticationType = "password",
            PasswordEncrypted = passwordBytes,
            PrivateKeyEncrypted = null,
            Version = 3,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };

        // then
        Assert.Equal("password", given.AuthenticationType);
        Assert.Equal(passwordBytes, given.PasswordEncrypted);
        Assert.Null(given.PrivateKeyEncrypted);
        Assert.Equal(3, given.Version);
    }
}
