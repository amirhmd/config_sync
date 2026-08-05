using ConfigSync.Adapters.In.Rest.Api;
using ConfigSync.Adapters.In.Rest.Endpoints;
using ConfigSync.Adapters.In.Rest.Mappers;
using ConfigSync.Adapters.In.Rest.Models.Requests;
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

        services.AddSingleton<IValidation<CreateDeviceRequest>, DeviceNameValidation>();
        services.AddSingleton<IValidation<CreateDeviceRequest>, DeviceHostValidation>();
        services.AddSingleton<IValidation<CreateDeviceRequest>, DevicePortValidation>();
        services.AddSingleton<IValidation<CreateDeviceRequest>, DeviceUsernameValidation>();
        services.AddSingleton<IValidation<CreateDeviceRequest>, DeviceCredentialValidation>();
        services.AddSingleton<IValidation<GetDevicesRequest>, PageLimitValidation>();

        return services;
    }
}
