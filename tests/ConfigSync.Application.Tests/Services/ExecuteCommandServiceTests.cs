using System;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using ConfigSync.Application.Ports.In;
using ConfigSync.Application.Services;
using Xunit;

namespace ConfigSync.Application.Tests.Services;

public class ExecuteCommandServiceTests
{
    [Fact]
    public async Task Execute_ReturnsANonEmptyReferenceId()
    {
        // given
        IExecuteCommandUseCase given = new ExecuteCommandService();

        // when
        Guid referenceId = await given.Execute("device123", "show version").ToTask();

        // then
        Assert.NotEqual(Guid.Empty, referenceId);
    }
}