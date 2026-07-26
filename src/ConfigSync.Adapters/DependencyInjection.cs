using ConfigSync.Adapters.In.Rest.Api;
using ConfigSync.Adapters.In.Rest.Endpoints;
using ConfigSync.Adapters.In.Rest.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigSync.Adapters;

public static class DependencyInjection
{
    public static IServiceCollection AddAdapters(this IServiceCollection services)
    {
        services.AddSingleton<IExecutionPlanMapper, ExecutionPlanMapper>();
        services.AddSingleton<IExecuteCommandEndpoint, ExecuteCommandEndpoint>();
        return services;
    }
}