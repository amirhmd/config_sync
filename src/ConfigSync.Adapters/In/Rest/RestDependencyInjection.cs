using ConfigSync.Adapters.In.Rest.Api;
using ConfigSync.Adapters.In.Rest.Endpoints;
using ConfigSync.Adapters.In.Rest.Mappers;
using ConfigSync.Adapters.In.Rest.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigSync.Adapters.In.Rest;

internal static class RestDependencyInjection
{
    internal static IServiceCollection AddRestAdapter(this IServiceCollection services)
    {
        services.AddSingleton<IExecutionPlanMapper, ExecutionPlanMapper>();
        services.AddSingleton<IExecuteCommandEndpoint, ExecuteCommandEndpoint>();

        services.AddSingleton<IDeviceMapper, DeviceMapper>();
        services.AddSingleton<IDeviceEndpoint, DeviceEndpoint>();

        services.AddSingleton<NameValidation>();
        services.AddSingleton<HostValidation>();
        services.AddSingleton<PortValidation>();
        services.AddSingleton<UsernameValidation>();
        services.AddSingleton<CredentialValidation>();
        services.AddSingleton<LimitValidation>();
        services.AddSingleton<RequestValidation>();

        return services;
    }
}
