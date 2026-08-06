using ConfigSync.Application.Ports.In;
using ConfigSync.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigSync.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IExecuteCommandUseCase, ExecuteCommandService>();
        services.AddSingleton<IDeviceUseCase, DeviceService>();
        return services;
    }
}