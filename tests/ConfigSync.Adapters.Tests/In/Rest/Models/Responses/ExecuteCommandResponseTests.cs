using System;
using ConfigSync.Adapters.In.Rest.Models.Responses;
using Xunit;

namespace ConfigSync.Adapters.Tests.In.Rest.Models.Responses;

public class ExecuteCommandResponseTests
{
    [Fact]
    public void Construction_SetsReferenceId()
    {
        // given
        var id = Guid.NewGuid();

        // when
        var given = new ExecuteCommandResponse { ReferenceId = id };

        // then
        Assert.Equal(id, given.ReferenceId);
    }
}
