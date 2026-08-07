using ConfigSync.Adapters.Out.Persistence.Postgres;
using ConfigSync.Adapters.Out.Persistence.Postgres.Mappers;
using ConfigSync.Application.Ports.Out;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigSync.Adapters.Out.Persistence;

internal static class PersistenceDependencyInjection
{
    internal static IServiceCollection AddPersistenceAdapter(this IServiceCollection services)
    {

        services.AddSingleton<IDevicePersistence, DeviceRepository>();
        services.AddSingleton<IDeviceEntityMapper, DeviceEntityMapper>();
        services.AddSingleton<CursorCodec>();
        return services;
    }
}