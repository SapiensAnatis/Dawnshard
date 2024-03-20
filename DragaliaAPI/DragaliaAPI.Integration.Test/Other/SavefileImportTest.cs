using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Serialization;
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

        LoadIndexResponse savefile = JsonSerializer
            .Deserialize<DragaliaResponse<LoadIndexResponse>>(
                savefileJson,
                ApiJsonOptions.Instance
            )!
            .Data;

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

        LoadIndexResponse storedSavefile = (
            await this.Client.PostMsgpack<LoadIndexResponse>("load/index")
        ).Data;

        storedSavefile
            .Should()
            .BeEquivalentTo(
                savefile,
                opts =>
                {
                    // Modified properties
                    // Inaccurate because primary keys
                    opts.Excluding(x => x.Name.Contains("KeyId"));
                    opts.Excluding(x => x.Name.Contains("BuildId"));
                    opts.Excluding(x => x.Name.Contains("PlantDetailId"));
                    opts.Excluding(x => x.UserData!.ViewerId);
                    // Inaccurate because transient
                    opts.Excluding(x => x.ServerTime);
                    opts.Excluding(x => x.SpecUpgradeTime);
                    opts.Excluding(x => x.Name.Contains("LastIncomeTime"));
                    opts.Excluding(x => x.UserData!.LastLoginTime);
                    // Inaccurate because notification
                    opts.Excluding(x => x.PresentNotice);
                    opts.Excluding(x => x.ShopNotice);
                    // Inaccurate for other reasons
                    opts.Excluding(x => x.UserData!.StaminaSingle);
                    opts.Excluding(x => x.UserData!.StaminaMulti);
                    opts.Excluding(x =>
                        x.Path.StartsWith("AbilityCrestList")
                        && (x.Name == "Ability1Level" || x.Name == "Ability2Level")
                    );
                    opts.Excluding(x => x.UserData!.Level);
                    opts.Excluding(x => x.UserData!.Crystal);
                    opts.Excluding(x => x.TreasureTradeAllList);
                    opts.Excluding(x => x.MultiServer);
                    opts.Excluding(x => x.MissionNotice);
                    opts.Excluding(x => x.UserData!.ActiveMemoryEventId);

                    opts.Excluding(x => x.UserData!.FortOpenTime);

                    // Ignored properties
                    opts.Excluding(x => x.UserData!.PrologueEndTime);
                    opts.Excluding(x => x.FortPlantList);
                    opts.Excluding(x => x.FortBonusList);
                    opts.Excluding(x => x.UserGuildData);
                    opts.Excluding(x => x.GuildData);

                    // Properties with no implementation
                    opts.Excluding(x => x.Name.Contains("Album"));

                    opts.Excluding(x => x.QuestBonus);
                    opts.Excluding(x => x.QuestCarryList);
                    opts.Excluding(x => x.QuestEntryConditionList);
                    opts.Excluding(x => x.QuestBonusStackBaseTime);

                    opts.Excluding(x => x.UserSummonList);
                    opts.Excluding(x => x.SummonTicketList);
                    opts.Excluding(x => x.SummonPointList);

                    opts.Excluding(x => x.SpecialShopPurchase);

                    opts.Excluding(x => x.AstralItemList);
                    opts.Excluding(x => x.WalkerData);
                    opts.Excluding(x => x.ExchangeTicketList);
                    opts.Excluding(x => x.LotteryTicketList);
                    opts.Excluding(x => x.GatherItemList);

                    opts.Excluding(x => x.FriendNotice);
                    opts.Excluding(x => x.GuildNotice);

                    return opts;
                }
            );
    }

    [Fact]
    public async Task Import_PropertiesMappedCorrectly()
    {
        HttpContent content = PrepareSavefileRequest();
        await this.Client.PostAsync($"savefile/import/{this.ViewerId}", content);

        this.ApiContext.PlayerStoryState.Single(x =>
            x.ViewerId == this.ViewerId && x.StoryId == 110313011
        )
            .StoryType.Should()
            .Be(StoryTypes.Chara);

        this.ApiContext.PlayerStoryState.Single(x =>
            x.ViewerId == this.ViewerId && x.StoryId == 210091011
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
            .Contain(x =>
                x.ViewerId == this.ViewerId && x.EmblemId == Emblems.IsolationSpeedslayer_1
            );
    }

    [Fact]
    public async Task Import_DoesNotDeleteBuyableDragonGifts()
    {
        this.ApiContext.PlayerDragonGifts.Should()
            .Contain(x =>
                x.ViewerId == this.ViewerId && x.DragonGiftId == DragonGifts.CompellingBook
            );

        HttpContent content = PrepareSavefileRequest();
        await this.Client.PostAsync($"savefile/import/{this.ViewerId}", content);

        this.ApiContext.PlayerDragonGifts.Should()
            .Contain(x =>
                x.ViewerId == this.ViewerId && x.DragonGiftId == DragonGifts.CompellingBook
            );
    }

    [Fact]
    public async Task Import_DeletesDailyMissions()
    {
        this.ApiContext.CompletedDailyMissions.Add(
            new() { ViewerId = this.ViewerId, Progress = 1, }
        );

        HttpContent content = PrepareSavefileRequest();
        await this.Client.PostAsync($"savefile/import/{this.ViewerId}", content);

        this.ApiContext.CompletedDailyMissions.Should()
            .NotContain(x => x.ViewerId == this.ViewerId);
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

        LoadIndexResponse savefile = JsonSerializer
            .Deserialize<DragaliaResponse<LoadIndexResponse>>(
                savefileJson,
                ApiJsonOptions.Instance
            )!
            .Data;

        HttpContent content = new StringContent(savefileJson);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        return content;
    }
}
