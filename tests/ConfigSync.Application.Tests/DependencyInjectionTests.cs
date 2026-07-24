using ConfigSync.Application.Ports.In;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ConfigSync.Application.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddApplication_RegistersIExecuteCommandUseCaseAsSingleton()
    {
        // given
        var services = new ServiceCollection();
        services.AddApplication();
        var provider = services.BuildServiceProvider();

        // when
        var first = provider.GetRequiredService<IExecuteCommandUseCase>();
        var second = provider.GetRequiredService<IExecuteCommandUseCase>();

        // then
        Assert.Same(first, second);
    }
}