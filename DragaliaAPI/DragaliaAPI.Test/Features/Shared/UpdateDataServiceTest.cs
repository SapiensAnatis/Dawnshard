﻿using System.Diagnostics;
using System.Text.Json;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Features.Dmode;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Friends;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Shared;
using DragaliaAPI.Mapping.Mapperly;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using DragaliaAPI.Test.Utils;

namespace DragaliaAPI.Test.Features.Shared;

public class UpdateDataServiceTest : RepositoryTestFixture
{
    private readonly ITestOutputHelper output;
    private readonly IUpdateDataService updateDataService;

    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;
    private readonly Mock<IMissionService> mockMissionService;
    private readonly Mock<IMissionProgressionService> mockMissionProgressionService;
    private readonly Mock<IPresentService> mockPresentService;
    private readonly Mock<IEventService> mockEventService;
    private readonly Mock<IDmodeService> mockDmodeService;
    private readonly Mock<IFriendNotificationService> mockFriendNotificationService;
    private readonly ActivitySource mockActivitySource;

    public UpdateDataServiceTest(ITestOutputHelper output)
    {
        this.output = output;
        this.mockPlayerIdentityService = new(MockBehavior.Strict);
        this.mockMissionService = new(MockBehavior.Loose);
        this.mockMissionProgressionService = new(MockBehavior.Strict);
        this.mockPresentService = new(MockBehavior.Strict);
        this.mockEventService = new(MockBehavior.Strict);
        this.mockDmodeService = new(MockBehavior.Strict);
        this.mockFriendNotificationService = new(MockBehavior.Strict);
        this.mockActivitySource = new ActivitySource("TestSource");

        this.mockFriendNotificationService.Setup(x =>
                x.GetFriendNotice(It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new FriendNotice());

        this.updateDataService = new UpdateDataService(
            this.ApiContext,
            this.mockPlayerIdentityService.Object,
            this.mockMissionService.Object,
            this.mockMissionProgressionService.Object,
            this.mockPresentService.Object,
            this.mockEventService.Object,
            this.mockDmodeService.Object,
            this.mockFriendNotificationService.Object,
            this.mockActivitySource
        );
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

        DbPlayerDragonData dragonData = new DbPlayerDragonData(
            viewerId,
            DragonId.DreadkingRathalos
        );
        DbPlayerDragonReliability reliabilityData = new DbPlayerDragonReliability(
            viewerId,
            DragonId.DreadkingRathalos
        );

        DbParty partyData = new()
        {
            ViewerId = viewerId,
            PartyName = "name",
            PartyNo = 1,
            Units = new List<DbPartyUnit>()
            {
                new() { CharaId = Charas.GalaAlex, UnitNo = 1 },
            },
        };

        DbPlayerStoryState questStoryState = new()
        {
            ViewerId = viewerId,
            State = StoryState.Read,
            StoryId = 2,
            StoryType = StoryTypes.Quest,
        };
        DbPlayerStoryState charaStoryState = new()
        {
            ViewerId = viewerId,
            State = StoryState.Read,
            StoryId = 4,
            StoryType = StoryTypes.Chara,
        };
        DbPlayerStoryState castleStoryState = new()
        {
            ViewerId = viewerId,
            State = StoryState.Unlocked,
            StoryId = 6,
            StoryType = StoryTypes.Castle,
        };

        DbPlayerStoryState dragonStoryState = new()
        {
            ViewerId = viewerId,
            State = StoryState.Unlocked,
            StoryId = 8,
            StoryType = StoryTypes.Dragon,
        };

        DbPlayerMaterial materialData = new()
        {
            ViewerId = viewerId,
            MaterialId = Materials.AlmightyOnesMaskFragment,
            Quantity = 10,
        };

        DbQuest questData = new()
        {
            ViewerId = viewerId,
            QuestId = 100010104,
            IsMissionClear1 = true,
            IsMissionClear2 = true,
            IsMissionClear3 = true,
            State = 3,
        };

        DbFortBuild buildData = new()
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
            BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(15),
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
                buildData,
            }
        );

        UpdateDataList list = await this.updateDataService.SaveChangesAsync(cts.Token);

        list.UserData.Should().BeEquivalentTo(userData.MapToUserData());

        AssertOnlyContains(list.CharaList, charaData, CharaMapper.ToCharaList);

        AssertOnlyContains(list.DragonList, dragonData, DragonMapper.ToDragonList);

        AssertOnlyContains(
            list.DragonReliabilityList,
            reliabilityData,
            DragonReliabilityMapper.ToDragonReliabilityList
        );

        AssertOnlyContains(list.PartyList, partyData, PartyMapper.MapToPartyList);

        AssertOnlyContains(list.QuestStoryList, questStoryState, StoryMapper.MapToQuestStoryList);

        list.UnitStoryList.Should()
            .ContainEquivalentOf(charaStoryState.MapToUnitStoryList())
            .And.ContainEquivalentOf(dragonStoryState.MapToUnitStoryList());

        AssertOnlyContains(
            list.CastleStoryList,
            castleStoryState,
            StoryMapper.MapToCastleStoryList
        );

        AssertOnlyContains(list.QuestList, questData, QuestMapper.MapToQuestList);

        AssertOnlyContains(list.MaterialList, materialData, MaterialMapper.MapToMaterialList);

        AssertOnlyContains(list.BuildList, buildData, FortBuildMapper.MapToBuildList);

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
                new DbPlayerDragonData(ViewerId, DragonId.Arsene),
                new DbPlayerDragonData(ViewerId, DragonId.GalaBeastVolk),
                new DbPlayerDragonData(ViewerId, DragonId.HighZodiark),
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

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        (await this.updateDataService.SaveChangesAsync(cts.Token)).CharaList.Should().BeNull();
    }

    private static void AssertOnlyContains<TNetwork, TDatabase>(
        IEnumerable<TNetwork>? member,
        TDatabase dbEntity,
        Func<TDatabase, TNetwork> mapper
    )
    {
        member
            .Should()
            .ContainSingle()
            .And.ContainEquivalentOf(
                mapper(dbEntity),
                opts => opts.WithTimeSpanTolerance(TimeSpan.FromSeconds(1))
            );
    }
}
