using System.Net;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using FluentAssertions.Execution;
using static DragaliaAPI.Services.Game.SavefileService;

namespace DragaliaAPI.Integration.Test.Other;

public class DeleteSavefileTest : TestFixture
{
    public DeleteSavefileTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        Environment.SetEnvironmentVariable("DEVELOPER_TOKEN", "supersecrettoken");
        this.Client.DefaultRequestHeaders.Add("Authorization", $"Bearer supersecrettoken");
    }

    [Fact]
    public async Task Delete_NoDeveloperToken_Returns401()
    {
        this.Client.DefaultRequestHeaders.Remove("Authorization");

        HttpResponseMessage importResponse = await this.Client.DeleteAsync($"savefile/delete/4");

        importResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_WrongDeveloperToken_Returns401()
    {
        this.Client.DefaultRequestHeaders.Remove("Authorization");
        this.Client.DefaultRequestHeaders.Add("Authorization", $"Bearer imfeeling22");

        HttpResponseMessage importResponse = await this.Client.DeleteAsync($"savefile/delete/4");

        importResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_LoadIndexResponseHasNewSavefile()
    {
        long viewerId = this.ApiContext.PlayerUserData
            .Single(x => x.DeviceAccountId == DeviceAccountId)
            .ViewerId;

        this.AddCharacter(Charas.Ilia);
        this.AddCharacter(Charas.DragonyuleIlia);

        HttpResponseMessage importResponse = await this.Client.DeleteAsync(
            $"savefile/delete/{viewerId}"
        );
        importResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        LoadIndexData storedSavefile = (
            await this.Client.PostMsgpack<LoadIndexData>("load/index", new LoadIndexRequest())
        ).data;

        storedSavefile.chara_list.Should().NotContain(x => x.chara_id == Charas.Ilia);
        storedSavefile.chara_list.Should().NotContain(x => x.chara_id == Charas.DragonyuleIlia);
    }
}
