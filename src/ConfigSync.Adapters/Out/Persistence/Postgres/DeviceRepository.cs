using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConfigSync.Adapters.Out.Persistence.Postgres.Entities;
using ConfigSync.Adapters.Out.Persistence.Postgres.Exceptions;
using ConfigSync.Adapters.Out.Persistence.Postgres.Mappers;
using ConfigSync.Application.Ports.Out;
using ConfigSync.Domain;
using Dapper;
using Npgsql;

namespace ConfigSync.Adapters.Out.Persistence.Postgres;

internal sealed class DeviceRepository(NpgsqlDataSource dataSource, IDeviceEntityMapper entityMapper, CursorCodec cursorCodec) : IDevicePersistence
{
    private const int MinimumLimit = 1;
    private const int MaximumLimit = 100;

    // Ids start at 1, so 0 represents the position before the first row.
    private const long FirstPageAfterId = 0;

    private const string SelectColumnsSql =
        """
        SELECT
            id                    AS "Id",
            name                  AS "Name",
            host                  AS "Host",
            port                  AS "Port",
            username              AS "Username",
            password_encrypted    AS "PasswordEncrypted",
            private_key_encrypted AS "PrivateKeyEncrypted",
            created_at            AS "CreatedAt"
        FROM devices
        """;

    private const string InsertSql =
        """
        INSERT INTO devices (
            name,
            host,
            port,
            username,
            password_encrypted,
            private_key_encrypted
        )
        VALUES (
            @Name,
            @Host,
            @Port,
            @Username,
            @PasswordEncrypted,
            @PrivateKeyEncrypted
        )
        ON CONFLICT (name) DO NOTHING;
        """;

    private const string GetByNameSql = $"{SelectColumnsSql} WHERE name = @Name;";

    private const string GetPageSql = $"{SelectColumnsSql} WHERE id > @AfterId ORDER BY id LIMIT @FetchLimit;";

    private const string DeleteSql = "DELETE FROM devices WHERE name = @Name;";

    public async Task<CreateOutcome> InsertAsync(Device device, CancellationToken cancellationToken)
    {
        byte[]? passwordEncrypted = null;
        byte[]? privateKeyEncrypted = null;

        if (device.AuthenticationType == DeviceAuthenticationType.Password)
        {
            passwordEncrypted = device.Credential.GetCiphertext();
        }
        else if (device.AuthenticationType == DeviceAuthenticationType.PrivateKey)
        {
            privateKeyEncrypted = device.Credential.GetCiphertext();
        }
        else
        {
            throw new InvalidOperationException($"Unsupported authentication type '{device.AuthenticationType}'.");
        }

        var parameters = new
        {
            device.Name,
            device.Host,
            device.Port,
            device.Username,
            PasswordEncrypted = passwordEncrypted,
            PrivateKeyEncrypted = privateKeyEncrypted
        };

        int affectedRows;

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(InsertSql, parameters, cancellationToken: cancellationToken);

            affectedRows = await connection.ExecuteAsync(command);
        }
        catch (NpgsqlException exception) when (exception.IsTransient)
        {
            throw new PostgresTransientException($"Transient DB failure inserting device '{device.Name}'.", exception);
        }

        return affectedRows switch
        {
            1 => CreateOutcome.Created,
            0 => CreateOutcome.AlreadyExists,
            _ => throw new InvalidOperationException($"Insert for device '{device.Name}' affected {affectedRows} rows; expected 0 or 1.")
        };
    }

    public async Task<Device?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        DeviceEntity? entity;

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(GetByNameSql, new { Name = name }, cancellationToken: cancellationToken);

            entity = await connection.QuerySingleOrDefaultAsync<DeviceEntity>(command);
        }
        catch (NpgsqlException exception) when (exception.IsTransient)
        {
            throw new PostgresTransientException($"Transient DB failure reading device '{name}'.", exception);
        }

        return entity is null ? null : entityMapper.ToDomain(entity);
    }

    public async Task<DevicePageOutcome> GetPageAsync(string? cursor, int limit, CancellationToken cancellationToken)
    {
        if (limit < MinimumLimit || limit > MaximumLimit)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), limit, $"Limit must be between {MinimumLimit} and {MaximumLimit}.");
        }

        var afterId = FirstPageAfterId;

        if (cursor is not null)
        {
            var decoded = cursorCodec.Decode(cursor);

            if (!decoded.IsValid)
            {
                return DevicePageInvalidCursor.Instance;
            }

            afterId = decoded.AfterId;
        }

        List<DeviceEntity> entities;

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            // One extra row answers whether another page exists.
            var parameters = new
            {
                AfterId = afterId,
                FetchLimit = limit + 1
            };

            var command = new CommandDefinition(GetPageSql, parameters, cancellationToken: cancellationToken);

            var rows = await connection.QueryAsync<DeviceEntity>(command);

            entities = rows.ToList();
        }
        catch (NpgsqlException exception) when (exception.IsTransient)
        {
            throw new PostgresTransientException($"Transient DB failure reading device page with limit {limit}.", exception);
        }

        var moreRemain = entities.Count > limit;

        // The cursor must contain the id of the last returned row,
        // not the extra row used to detect another page.
        var nextCursor = moreRemain ? cursorCodec.Encode(entities[limit - 1].Id) : null;

        var devices = entities
            .Take(limit)
            .Select(entityMapper.ToDomain)
            .ToList();

        return new DevicePageSuccess(new Page<Device>(devices, nextCursor));
    }

    public async Task<DeleteOutcome> DeleteAsync(string name, CancellationToken cancellationToken)
    {
        int affectedRows;

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(DeleteSql, new { Name = name }, cancellationToken: cancellationToken);

            affectedRows = await connection.ExecuteAsync(command);
        }
        catch (NpgsqlException exception) when (exception.IsTransient)
        {
            throw new PostgresTransientException($"Transient DB failure deleting device '{name}'.", exception);
        }

        return affectedRows switch
        {
            1 => DeleteOutcome.Deleted,
            0 => DeleteOutcome.NotFound,
            _ => throw new InvalidOperationException($"Delete for device '{name}' affected {affectedRows} rows; expected 0 or 1.")
        };
    }
}