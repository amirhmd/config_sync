using ConfigSync.Adapters.Out.Persistence.Postgres;
using ConfigSync.Application.Ports.Out;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigSync.Adapters.Out.Persistence;

internal static class PersistenceDependencyInjection
{
    internal static IServiceCollection AddPersistenceAdapter(this IServiceCollection services)
    {

        services.AddSingleton<IDevicePersistence, DeviceRepository>();
        return services;
    }
}