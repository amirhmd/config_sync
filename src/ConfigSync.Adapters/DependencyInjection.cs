using ConfigSync.Adapters.In.Rest;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigSync.Adapters;

public static class DependencyInjection
{
    public static IServiceCollection AddAdapters(this IServiceCollection services)
    {
        services.AddRestAdapter();
        return services;
    }
}
