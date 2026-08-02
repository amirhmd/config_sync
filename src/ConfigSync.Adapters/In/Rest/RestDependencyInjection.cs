using ConfigSync.Adapters.In.Rest.Api;
using ConfigSync.Adapters.In.Rest.Endpoints;
using ConfigSync.Adapters.In.Rest.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigSync.Adapters.In.Rest;

internal static class RestDependencyInjection
{
    internal static IServiceCollection AddRestAdapter(
        this IServiceCollection services)
    {
        services.AddSingleton<IExecutionPlanMapper, ExecutionPlanMapper>();
        services.AddSingleton<IExecuteCommandEndpoint, ExecuteCommandEndpoint>();

        return services;
    }
}