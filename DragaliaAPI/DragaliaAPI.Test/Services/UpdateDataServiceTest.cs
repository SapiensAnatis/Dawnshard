using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Features.Dmode;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using DragaliaAPI.Test.Utils;
using Xunit.Abstractions;

namespace DragaliaAPI.Test.Services;

public class UpdateDataServiceTest : RepositoryTestFixture
{
    private readonly ITestOutputHelper output;
    private readonly IMapper mapper;
    private readonly IUpdateDataService updateDataService;

    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;
    private readonly Mock<IMissionService> mockMissionService;
    private readonly Mock<IMissionProgressionService> mockMissionProgressionService;
    private readonly Mock<IPresentService> mockPresentService;
    private readonly Mock<IEventService> mockEventService;
    private readonly Mock<IDmodeService> mockDmodeService;

    public UpdateDataServiceTest(ITestOutputHelper output)
    {
        this.output = output;
        this.mockPlayerIdentityService = new(MockBehavior.Strict);
        this.mockMissionService = new(MockBehavior.Loose);
        this.mockMissionProgressionService = new(MockBehavior.Strict);
        this.mockPresentService = new(MockBehavior.Strict);
        this.mockEventService = new(MockBehavior.Strict);
        this.mockDmodeService = new(MockBehavior.Strict);
        this.mapper = UnitTestUtils.CreateMapper();

        this.updateDataService = new UpdateDataService(
            this.ApiContext,
            this.mockPlayerIdentityService.Object,
            this.mockMissionService.Object,
            this.mockMissionProgressionService.Object,
            this.mockPresentService.Object,
            this.mockEventService.Object,
            this.mockDmodeService.Object
        );

        CommonAssertionOptions.ApplyTimeOptions();
    }

    [Fact]
    public async Task SaveChangesAsync_PopulatesAll()
    {
        long viewerId = 2;
        CancellationTokenSource cts = new();

        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(viewerId);
        this.mockMissionProgressionService.Setup(x => x.ProcessMissionEvents(cts.Token))
            .Returns(Task.CompletedTask);

        DbPlayerUserData userData = new() { ViewerId = viewerId };

        DbPlayerCharaData charaData = new(viewerId, Charas.GalaLeonidas);

        DbPlayerDragonData dragonData = DbPlayerDragonDataFactory.Create(
            viewerId,
            Dragons.DreadkingRathalos
        );
        DbPlayerDragonReliability reliabilityData = DbPlayerDragonReliabilityFactory.Create(
            viewerId,
            Dragons.DreadkingRathalos
        );

        DbParty partyData =
            new()
            {
                ViewerId = viewerId,
                PartyName = "name",
                PartyNo = 1,
                Units = new List<DbPartyUnit>()
                {
                    new() { CharaId = Charas.GalaAlex, UnitNo = 1 }
                }
            };

        DbPlayerStoryState questStoryState =
            new()
            {
                ViewerId = viewerId,
                State = StoryState.Read,
                StoryId = 2,
                StoryType = StoryTypes.Quest
            };
        DbPlayerStoryState charaStoryState =
            new()
            {
                ViewerId = viewerId,
                State = StoryState.Read,
                StoryId = 4,
                StoryType = StoryTypes.Chara,
            };
        DbPlayerStoryState castleStoryState =
            new()
            {
                ViewerId = viewerId,
                State = StoryState.Unlocked,
                StoryId = 6,
                StoryType = StoryTypes.Castle,
            };

        DbPlayerStoryState dragonStoryState =
            new()
            {
                ViewerId = viewerId,
                State = StoryState.Unlocked,
                StoryId = 8,
                StoryType = StoryTypes.Dragon,
            };

        DbPlayerMaterial materialData =
            new()
            {
                ViewerId = viewerId,
                MaterialId = Materials.AlmightyOnesMaskFragment,
                Quantity = 10
            };

        DbQuest questData =
            new()
            {
                ViewerId = viewerId,
                QuestId = 100010104,
                IsMissionClear1 = true,
                IsMissionClear2 = true,
                IsMissionClear3 = true,
                State = 3
            };

        DbFortBuild buildData =
            new()
            {
                ViewerId = viewerId,
                BuildId = 4000,
                Level = 2,
                PositionX = 3,
                PositionZ = 4,
                PlantId = FortPlants.IronWeatherVane,
                IsNew = true,
                LastIncomeDate = DateTimeOffset.FromUnixTimeSeconds(5),
                BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(10),
                BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(15)
            };

        this.ApiContext.AddRange(
            new List<IDbPlayerData>()
            {
                userData,
                charaData,
                dragonData,
                reliabilityData,
                partyData,
                questStoryState,
                charaStoryState,
                castleStoryState,
                dragonStoryState,
                materialData,
                questData,
                buildData
            }
        );

        UpdateDataList list = await this.updateDataService.SaveChangesAsync(cts.Token);

        list.UserData.Should().BeEquivalentTo(this.mapper.Map<UserData>(userData));

        AssertOnlyContains<CharaList>(list.CharaList, charaData);

        AssertOnlyContains<DragonList>(list.DragonList, dragonData);

        AssertOnlyContains<DragonReliabilityList>(list.DragonReliabilityList, reliabilityData);

        AssertOnlyContains<PartyList>(list.PartyList, partyData);

        AssertOnlyContains<QuestStoryList>(list.QuestStoryList, questStoryState);

        list.UnitStoryList.Should()
            .ContainEquivalentOf(mapper.Map<UnitStoryList>(charaStoryState))
            .And.ContainEquivalentOf(mapper.Map<UnitStoryList>(dragonStoryState));

        AssertOnlyContains<CastleStoryList>(list.CastleStoryList, castleStoryState);

        AssertOnlyContains<QuestList>(list.QuestList, questData);

        AssertOnlyContains<MaterialList>(list.MaterialList, materialData);

        AssertOnlyContains<BuildList>(list.BuildList, buildData);

        list.DragonGiftList.Should().BeNull();

        this.output.WriteLine(
            "{0}",
            JsonSerializer.Serialize(list, new JsonSerializerOptions() { WriteIndented = true })
        );
    }

    [Fact]
    public async Task SaveChangesAsync_RetrievesIdentityColumns()
    {
        CancellationTokenSource cts = new();

        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(ViewerId);
        this.mockMissionProgressionService.Setup(x => x.ProcessMissionEvents(cts.Token))
            .Returns(Task.CompletedTask);

        // This test is bullshit because in-mem works differently to an actual database in this regard
        this.ApiContext.AddRange(
            new List<IDbPlayerData>()
            {
                DbPlayerDragonDataFactory.Create(ViewerId, Dragons.Arsene),
                DbPlayerDragonDataFactory.Create(ViewerId, Dragons.GalaBeastVolk),
                DbPlayerDragonDataFactory.Create(ViewerId, Dragons.HighZodiark)
            }
        );

        UpdateDataList list = await this.updateDataService.SaveChangesAsync(cts.Token);

        list.DragonList.Should().NotBeNullOrEmpty();
        list.DragonList!.Select(x => x.DragonKeyId).Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public async Task SaveChangesAsync_NullIfNoUpdates()
    {
        CancellationTokenSource cts = new();
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(ViewerId);
        this.mockMissionProgressionService.Setup(x => x.ProcessMissionEvents(cts.Token))
            .Returns(Task.CompletedTask);

        UpdateDataList list = await this.updateDataService.SaveChangesAsync(cts.Token);

        list.UserData.Should().BeNull();
        list.CharaList.Should().BeNull();
        list.DragonList.Should().BeNull();
        list.DragonReliabilityList.Should().BeNull();
        list.PartyList.Should().BeNull();
        list.QuestStoryList.Should().BeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_NoDataFromOtherAccounts()
    {
        CancellationTokenSource cts = new();
        this.ApiContext.PlayerCharaData.Add(new(ViewerId + 1, Charas.GalaZethia));
        this.mockMissionProgressionService.Setup(x => x.ProcessMissionEvents(cts.Token))
            .Returns(Task.CompletedTask);

        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(ViewerId);

        (await this.updateDataService.SaveChangesAsync(cts.Token)).CharaList.Should().BeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_NullAfterSave()
    {
        CancellationTokenSource cts = new();
        this.ApiContext.PlayerCharaData.Add(new(ViewerId, Charas.HalloweenLowen));
        this.mockMissionProgressionService.Setup(x => x.ProcessMissionEvents(cts.Token))
            .Returns(Task.CompletedTask);

        await this.ApiContext.SaveChangesAsync();

        (await this.updateDataService.SaveChangesAsync(cts.Token)).CharaList.Should().BeNull();
    }

    private void AssertOnlyContains<TNetwork>(IEnumerable<TNetwork>? member, IDbPlayerData dbEntity)
    {
        member
            .Should()
            .ContainSingle()
            .And.ContainEquivalentOf(this.mapper.Map<TNetwork>(dbEntity));
    }
}
