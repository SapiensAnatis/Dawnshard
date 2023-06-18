using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;
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
