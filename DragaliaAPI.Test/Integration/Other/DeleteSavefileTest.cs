using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Extensions.Configuration;

namespace DragaliaAPI.Test.Integration.Other;

[Collection("DragaliaIntegration")]
public class DeleteSavefileTest : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture fixture;
    private readonly HttpClient client;

    public DeleteSavefileTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        this.client = fixture.CreateClient();
        this.client.DefaultRequestHeaders.Add(
            "Developer-Token",
            $"Bearer {fixture.Configuration.GetValue<string>("DeveloperToken")}"
        );
    }

    [Fact]
    public async Task Delete_NoDeveloperToken_Returns401()
    {
        this.client.DefaultRequestHeaders.Remove("Developer-Token");

        HttpResponseMessage importResponse = await this.client.DeleteAsync($"savefile/delete/4");

        importResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_LoadIndexResponseHasNewSavefile()
    {
        long viewerId = fixture.ApiContext.PlayerUserData
            .Single(x => x.DeviceAccountId == fixture.DeviceAccountId)
            .ViewerId;

        await this.fixture.AddCharacter(Charas.Ilia);
        await this.fixture.AddCharacter(Charas.DragonyuleIlia);

        HttpResponseMessage importResponse = await this.client.DeleteAsync(
            $"savefile/delete/{viewerId}"
        );
        importResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        LoadIndexData storedSavefile = (
            await this.client.PostMsgpack<LoadIndexData>("load/index", new LoadIndexRequest())
        ).data;

        // Very lackluster test, because the test fixture savefile is basically empty anyway
        storedSavefile.chara_list
            .Should()
            .ContainSingle()
            .And.AllSatisfy(x => x.chara_id.Should().Be(Charas.ThePrince));
        // Not so due to the default data being given
        // storedSavefile.material_list.Should().BeEmpty();
        // storedSavefile.dragon_list.Should().BeEmpty();
        // storedSavefile.dragon_reliability_list.Should().BeEmpty();
        storedSavefile.quest_story_list.Should().BeEmpty();
        storedSavefile.castle_story_list.Should().BeEmpty();
    }
}
