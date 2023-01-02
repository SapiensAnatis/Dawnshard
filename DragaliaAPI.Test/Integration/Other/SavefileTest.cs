using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Json;

namespace DragaliaAPI.Test.Integration.Other;

/// <summary>
/// Tests <see cref="Controllers.Other.SavefileController"/>
/// </summary>
[Collection("DragaliaIntegration")]
public class SavefileTest : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture fixture;
    private readonly HttpClient client;

    public SavefileTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        this.client = fixture.CreateClient();

        Environment.SetEnvironmentVariable("DEVELOPER_TOKEN", "supersecrettoken");
        this.client.DefaultRequestHeaders.Add("Authorization", $"Bearer supersecrettoken");
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
    public async Task Import_LoadIndexReturnsImportedSavefile()
    {
        string savefileJson = File.ReadAllText(Path.Join("Data", "endgame_savefile.json"));
        long viewerId = fixture.ApiContext.PlayerUserData
            .Single(x => x.DeviceAccountId == fixture.DeviceAccountId)
            .ViewerId;

        JsonSerializerOptions options = new(JsonSerializerDefaults.General);
        options.Converters.Add(new UnixDateTimeJsonConverter());
        options.Converters.Add(new BoolIntJsonConverter());

        LoadIndexData savefile = (
            JsonSerializer.Deserialize<DragaliaResponse<LoadIndexData>>(savefileJson, options)
            ?? throw new Exception("Failed to load example savefile!")
        ).data;

        // JsonContent(savefile) doesn't appear to work, producing 400 Bad Request
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
                    opts.Excluding(x => x.Name.Contains("key_id"));
                    opts.Excluding(x => x.user_data.viewer_id);
                    opts.Excluding(x => x.server_time);
                    opts.Excluding(x => x.spec_upgrade_time);

                    // Ignored properties
                    opts.Excluding(x => x.user_data.prologue_end_time);

                    // Properties with no implementation
                    opts.Excluding(x => x.Name.Contains("album"));
                    opts.Excluding(x => x.Name.Contains("shop"));

                    opts.Excluding(x => x.fort_bonus_list);
                    opts.Excluding(x => x.fort_plant_list);
                    opts.Excluding(x => x.build_list);

                    opts.Excluding(x => x.quest_event_list);
                    opts.Excluding(x => x.quest_bonus);
                    opts.Excluding(x => x.quest_carry_list);
                    opts.Excluding(x => x.quest_treasure_list);
                    opts.Excluding(x => x.quest_wall_list);
                    opts.Excluding(x => x.quest_entry_condition_list);
                    opts.Excluding(x => x.quest_bonus_stack_base_time);

                    opts.Excluding(x => x.dragon_gift_list);

                    opts.Excluding(x => x.user_guild_data);
                    opts.Excluding(x => x.guild_data);

                    opts.Excluding(x => x.user_summon_list);
                    opts.Excluding(x => x.summon_ticket_list);
                    opts.Excluding(x => x.summon_point_list);

                    opts.Excluding(x => x.user_treasure_trade_list);
                    opts.Excluding(x => x.treasure_trade_all_list);

                    opts.Excluding(x => x.weapon_skin_list);
                    opts.Excluding(x => x.weapon_passive_ability_list);

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

                    return opts;
                }
            );
    }
}
