using ConfigSync.Adapters.In.Rest;
using ConfigSync.Adapters.Out.Persistence.Postgres;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigSync.Adapters;

public static class DependencyInjection
{
    public static IServiceCollection AddAdapters(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRestAdapter();
        services.AddPostgresAdapter(configuration);

        return services;
    }
}