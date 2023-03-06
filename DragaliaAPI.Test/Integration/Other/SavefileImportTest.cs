using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Json;

namespace DragaliaAPI.Test.Integration.Other;

/// <summary>
/// Tests <see cref="Controllers.Other.SavefileController"/>
/// </summary>
[Collection("DragaliaIntegration")]
public class SavefileImportTest : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture fixture;
    private readonly HttpClient client;

    public SavefileImportTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        this.client = fixture.CreateClient();

        Environment.SetEnvironmentVariable("DEVELOPER_TOKEN", "supersecrettoken");
        this.client.DefaultRequestHeaders.Add("Authorization", $"Bearer supersecrettoken");

        TestUtils.ApplyDateTimeAssertionOptions();
    }

    [Fact]
    public async Task Import_NoDeveloperToken_Returns401()
    {
        this.client.DefaultRequestHeaders.Remove("Authorization");

        HttpResponseMessage importResponse = await this.client.PostAsync(
            $"savefile/import/1",
            JsonContent.Create(new { })
        );

        importResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Import_WrongDeveloperToken_Returns401()
    {
        this.client.DefaultRequestHeaders.Remove("Authorization");
        this.client.DefaultRequestHeaders.Add("Authorization", "blub blub blub");

        HttpResponseMessage importResponse = await this.client.PostAsync(
            $"savefile/import/1",
            JsonContent.Create(new { })
        );

        importResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Import_LoadIndexReturnsImportedSavefile()
    {
        string savefileJson = File.ReadAllText(Path.Join("Data", "endgame_savefile.json"));
        long viewerId = this.fixture.ApiContext.PlayerUserData
            .Single(x => x.DeviceAccountId == fixture.DeviceAccountId)
            .ViewerId;

        LoadIndexData savefile = JsonSerializer
            .Deserialize<DragaliaResponse<LoadIndexData>>(savefileJson, ApiJsonOptions.Instance)!
            .data;

        HttpContent content = new StringContent(savefileJson);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

        HttpResponseMessage importResponse = await this.client.PostAsync(
            $"savefile/import/{viewerId}",
            content
        );
        importResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        LoadIndexData storedSavefile = (
            await this.client.PostMsgpack<LoadIndexData>("load/index", new LoadIndexRequest())
        ).data;

        storedSavefile
            .Should()
            .BeEquivalentTo(
                savefile,
                opts =>
                {
                    // Modified properties
                    // Inaccurate because primary keys
                    opts.Excluding(x => x.Name.Contains("key_id"));
                    opts.Excluding(x => x.Name.Contains("build_id"));
                    opts.Excluding(x => x.Name.Contains("plant_detail_id"));
                    opts.Excluding(x => x.user_data!.viewer_id);
                    // Inaccurate because transient
                    opts.Excluding(x => x.server_time);
                    opts.Excluding(x => x.spec_upgrade_time);
                    opts.Excluding(x => x.Name.Contains("last_income_time"));
                    opts.Excluding(x => x.user_data!.last_login_time);
                    // Inaccurate for other reasons
                    opts.Excluding(x => x.user_data!.stamina_single);
                    opts.Excluding(x => x.user_data!.stamina_multi);
                    opts.Excluding(
                        x =>
                            x.Path.StartsWith("ability_crest_list")
                            && (x.Name == "ability_1_level" || x.Name == "ability_2_level")
                    );
                    opts.Excluding(x => x.user_data!.level);
                    opts.Excluding(x => x.user_data!.crystal);

                    // Ignored properties
                    opts.Excluding(x => x.user_data!.prologue_end_time);
                    opts.Excluding(x => x.fort_plant_list);
                    opts.Excluding(x => x.fort_bonus_list);
                    opts.Excluding(x => x.user_guild_data);
                    opts.Excluding(x => x.guild_data);

                    // Properties with no implementation
                    opts.Excluding(x => x.Name.Contains("album"));
                    opts.Excluding(x => x.Name.Contains("shop"));

                    opts.Excluding(x => x.quest_event_list);
                    opts.Excluding(x => x.quest_bonus);
                    opts.Excluding(x => x.quest_carry_list);
                    opts.Excluding(x => x.quest_treasure_list);
                    opts.Excluding(x => x.quest_wall_list);
                    opts.Excluding(x => x.quest_entry_condition_list);
                    opts.Excluding(x => x.quest_bonus_stack_base_time);

                    opts.Excluding(x => x.user_summon_list);
                    opts.Excluding(x => x.summon_ticket_list);
                    opts.Excluding(x => x.summon_point_list);

                    opts.Excluding(x => x.user_treasure_trade_list);
                    opts.Excluding(x => x.treasure_trade_all_list);

                    opts.Excluding(x => x.astral_item_list);
                    opts.Excluding(x => x.party_power_data);
                    opts.Excluding(x => x.multi_server);
                    opts.Excluding(x => x.mission_notice);
                    opts.Excluding(x => x.walker_data);
                    opts.Excluding(x => x.equip_stamp_list);
                    opts.Excluding(x => x.exchange_ticket_list);
                    opts.Excluding(x => x.lottery_ticket_list);
                    opts.Excluding(x => x.gather_item_list);

                    opts.Excluding(x => x.present_notice);
                    opts.Excluding(x => x.friend_notice);
                    opts.Excluding(x => x.guild_notice);

                    return opts;
                }
            );
    }

    [Fact]
    public async Task Import_PropertiesMappedCorrectly()
    {
        long viewerId = this.fixture.ApiContext.PlayerUserData
            .Single(x => x.DeviceAccountId == fixture.DeviceAccountId)
            .ViewerId;

        HttpContent content = PrepareSavefileRequest();
        await this.client.PostAsync($"savefile/import/{viewerId}", content);

        fixture.ApiContext.PlayerStoryState
            .Single(x => x.DeviceAccountId == fixture.DeviceAccountId && x.StoryId == 110313011)
            .StoryType.Should()
            .Be(StoryTypes.Chara);

        fixture.ApiContext.PlayerStoryState
            .Single(x => x.DeviceAccountId == fixture.DeviceAccountId && x.StoryId == 210091011)
            .StoryType.Should()
            .Be(StoryTypes.Dragon);
    }

    [Fact]
    public async Task Import_IsIdempotent()
    {
        long viewerId = this.fixture.ApiContext.PlayerUserData
            .Single(x => x.DeviceAccountId == fixture.DeviceAccountId)
            .ViewerId;

        HttpContent content = PrepareSavefileRequest();

        HttpResponseMessage importResponse = await this.client.PostAsync(
            $"savefile/import/{viewerId}",
            content
        );
        importResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        HttpResponseMessage importResponse2 = await this.client.PostAsync(
            $"savefile/import/{viewerId}",
            content
        );
        importResponse2.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private static HttpContent PrepareSavefileRequest()
    {
        string savefileJson = File.ReadAllText(Path.Join("Data", "endgame_savefile.json"));

        LoadIndexData savefile = JsonSerializer
            .Deserialize<DragaliaResponse<LoadIndexData>>(savefileJson, ApiJsonOptions.Instance)!
            .data;

        HttpContent content = new StringContent(savefileJson);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        return content;
    }
}
