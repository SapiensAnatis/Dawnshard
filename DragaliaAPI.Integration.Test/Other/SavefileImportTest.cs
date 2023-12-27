using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Json;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Other;

/// <summary>
/// Tests <see cref="Controllers.Other.SavefileController"/>
/// </summary>
public class SavefileImportTest : TestFixture
{
    public SavefileImportTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        Environment.SetEnvironmentVariable("DEVELOPER_TOKEN", "supersecrettoken");
        this.Client.DefaultRequestHeaders.Add("Authorization", $"Bearer supersecrettoken");

        CommonAssertionOptions.ApplyTimeOptions();
    }

    [Fact]
    public async Task Import_NoDeveloperToken_Returns401()
    {
        this.Client.DefaultRequestHeaders.Remove("Authorization");

        HttpResponseMessage importResponse = await this.Client.PostAsync(
            $"savefile/import/1",
            JsonContent.Create(new { })
        );

        importResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Import_WrongDeveloperToken_Returns401()
    {
        this.Client.DefaultRequestHeaders.Remove("Authorization");
        this.Client.DefaultRequestHeaders.Add("Authorization", "blub blub blub");

        HttpResponseMessage importResponse = await this.Client.PostAsync(
            $"savefile/import/1",
            JsonContent.Create(new { })
        );

        importResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Import_LoadIndexReturnsImportedSavefile()
    {
        string savefileJson = File.ReadAllText(Path.Join("Data", "endgame_savefile.json"));

        LoadIndexData savefile = JsonSerializer
            .Deserialize<DragaliaResponse<LoadIndexData>>(savefileJson, ApiJsonOptions.Instance)!
            .data;

        HttpContent content = new StringContent(savefileJson);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

        HttpResponseMessage importResponse = await this.Client.PostAsync(
            $"savefile/import/{this.ViewerId}",
            content
        );
        importResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        this.ApiContext.PlayerUserData.AsNoTracking()
            .Single(x => x.ViewerId == this.ViewerId)
            .LastSaveImportTime.Should()
            .BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1));

        LoadIndexData storedSavefile = (
            await this.Client.PostMsgpack<LoadIndexData>("load/index", new LoadIndexRequest())
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
                    // Inaccurate because notification
                    opts.Excluding(x => x.present_notice);
                    opts.Excluding(x => x.shop_notice);
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
                    opts.Excluding(x => x.treasure_trade_all_list);
                    opts.Excluding(x => x.multi_server);
                    opts.Excluding(x => x.mission_notice);
                    opts.Excluding(x => x.user_data!.active_memory_event_id);

                    opts.Excluding(x => x.user_data!.fort_open_time);

                    // Ignored properties
                    opts.Excluding(x => x.user_data!.prologue_end_time);
                    opts.Excluding(x => x.fort_plant_list);
                    opts.Excluding(x => x.fort_bonus_list);
                    opts.Excluding(x => x.user_guild_data);
                    opts.Excluding(x => x.guild_data);

                    // Properties with no implementation
                    opts.Excluding(x => x.Name.Contains("album"));

                    opts.Excluding(x => x.quest_bonus);
                    opts.Excluding(x => x.quest_carry_list);
                    opts.Excluding(x => x.quest_entry_condition_list);
                    opts.Excluding(x => x.quest_bonus_stack_base_time);

                    opts.Excluding(x => x.user_summon_list);
                    opts.Excluding(x => x.summon_ticket_list);
                    opts.Excluding(x => x.summon_point_list);

                    opts.Excluding(x => x.special_shop_purchase);

                    opts.Excluding(x => x.astral_item_list);
                    opts.Excluding(x => x.walker_data);
                    opts.Excluding(x => x.exchange_ticket_list);
                    opts.Excluding(x => x.lottery_ticket_list);
                    opts.Excluding(x => x.gather_item_list);

                    opts.Excluding(x => x.friend_notice);
                    opts.Excluding(x => x.guild_notice);

                    return opts;
                }
            );
    }

    [Fact]
    public async Task Import_PropertiesMappedCorrectly()
    {
        HttpContent content = PrepareSavefileRequest();
        await this.Client.PostAsync($"savefile/import/{this.ViewerId}", content);

        this.ApiContext.PlayerStoryState.Single(
            x => x.ViewerId == this.ViewerId && x.StoryId == 110313011
        )
            .StoryType.Should()
            .Be(StoryTypes.Chara);

        this.ApiContext.PlayerStoryState.Single(
            x => x.ViewerId == this.ViewerId && x.StoryId == 210091011
        )
            .StoryType.Should()
            .Be(StoryTypes.Dragon);
    }

    [Fact]
    public async Task Import_DoesNotDeleteEmblems()
    {
        await this.AddToDatabase(
            new DbEmblem() { ViewerId = this.ViewerId, EmblemId = Emblems.IsolationSpeedslayer_1 }
        );

        this.ApiContext.ChangeTracker.Clear();

        HttpContent content = PrepareSavefileRequest();
        await this.Client.PostAsync($"savefile/import/{this.ViewerId}", content);

        this.ApiContext.Emblems.AsNoTracking()
            .Should()
            .Contain(
                x => x.ViewerId == this.ViewerId && x.EmblemId == Emblems.IsolationSpeedslayer_1
            );
    }

    [Fact]
    public async Task Import_DoesNotDeleteBuyableDragonGifts()
    {
        this.ApiContext.PlayerDragonGifts.Should()
            .Contain(
                x => x.ViewerId == this.ViewerId && x.DragonGiftId == DragonGifts.CompellingBook
            );

        HttpContent content = PrepareSavefileRequest();
        await this.Client.PostAsync($"savefile/import/{this.ViewerId}", content);

        this.ApiContext.PlayerDragonGifts.Should()
            .Contain(
                x => x.ViewerId == this.ViewerId && x.DragonGiftId == DragonGifts.CompellingBook
            );
    }

    [Fact]
    public async Task Import_IsIdempotent()
    {
        long viewerId = this.ApiContext.PlayerUserData.Single(x => x.ViewerId == ViewerId).ViewerId;

        HttpContent content = PrepareSavefileRequest();

        HttpResponseMessage importResponse = await this.Client.PostAsync(
            $"savefile/import/{viewerId}",
            content
        );
        importResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        HttpResponseMessage importResponse2 = await this.Client.PostAsync(
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
