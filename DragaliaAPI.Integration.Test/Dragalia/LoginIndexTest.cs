using DragaliaAPI.Models.Generated;
using Xunit.Abstractions;

namespace DragaliaAPI.Integration.Test.Dragalia;

public class LoginIndexTest : TestFixture
{
    public LoginIndexTest(
        CustomWebApplicationFactory<Program> factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper) { }

    [Fact]
    public async Task LoginIndex_ReturnsSuccess()
    {
        await this.Client
            .Invoking(x => x.PostMsgpack<LoginIndexData>("/login/index", new()))
            .Should()
            .NotThrowAsync();
    }
}
