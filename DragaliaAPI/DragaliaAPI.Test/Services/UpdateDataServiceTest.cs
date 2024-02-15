using System.Text.Json;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Database.Test;
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
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
using static DragaliaAPI.Test.Utils.IdentityTestUtils;

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
            this.mapper,
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
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(viewerId);
        this.mockMissionProgressionService.Setup(x => x.ProcessMissionEvents())
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

        UpdateDataList list = await this.updateDataService.SaveChangesAsync();

        list.user_data.Should().BeEquivalentTo(this.mapper.Map<UserData>(userData));

        AssertOnlyContains<CharaList>(list.chara_list, charaData);

        AssertOnlyContains<DragonList>(list.dragon_list, dragonData);

        AssertOnlyContains<DragonReliabilityList>(list.dragon_reliability_list, reliabilityData);

        AssertOnlyContains<PartyList>(list.party_list, partyData);

        AssertOnlyContains<QuestStoryList>(list.quest_story_list, questStoryState);

        list.unit_story_list.Should()
            .ContainEquivalentOf(mapper.Map<UnitStoryList>(charaStoryState))
            .And.ContainEquivalentOf(mapper.Map<UnitStoryList>(dragonStoryState));

        AssertOnlyContains<CastleStoryList>(list.castle_story_list, castleStoryState);

        AssertOnlyContains<QuestList>(list.quest_list, questData);

        AssertOnlyContains<MaterialList>(list.material_list, materialData);

        AssertOnlyContains<BuildList>(list.build_list, buildData);

        list.dragon_gift_list.Should().BeNull();

        this.output.WriteLine(
            "{0}",
            JsonSerializer.Serialize(list, new JsonSerializerOptions() { WriteIndented = true })
        );
    }

    [Fact]
    public async Task SaveChangesAsync_RetrievesIdentityColumns()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(ViewerId);
        this.mockMissionProgressionService.Setup(x => x.ProcessMissionEvents())
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

        UpdateDataList list = await this.updateDataService.SaveChangesAsync();

        list.dragon_list.Should().NotBeNullOrEmpty();
        list.dragon_list!.Select(x => x.dragon_key_id).Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public async Task SaveChangesAsync_NullIfNoUpdates()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(ViewerId);
        this.mockMissionProgressionService.Setup(x => x.ProcessMissionEvents())
            .Returns(Task.CompletedTask);

        UpdateDataList list = await this.updateDataService.SaveChangesAsync();

        list.user_data.Should().BeNull();
        list.chara_list.Should().BeNull();
        list.dragon_list.Should().BeNull();
        list.dragon_reliability_list.Should().BeNull();
        list.party_list.Should().BeNull();
        list.quest_story_list.Should().BeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_NoDataFromOtherAccounts()
    {
        this.ApiContext.PlayerCharaData.Add(new(ViewerId + 1, Charas.GalaZethia));
        this.mockMissionProgressionService.Setup(x => x.ProcessMissionEvents())
            .Returns(Task.CompletedTask);

        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(ViewerId);

        (await this.updateDataService.SaveChangesAsync()).chara_list.Should().BeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_NullAfterSave()
    {
        this.ApiContext.PlayerCharaData.Add(new(ViewerId, Charas.HalloweenLowen));
        this.mockMissionProgressionService.Setup(x => x.ProcessMissionEvents())
            .Returns(Task.CompletedTask);

        await this.ApiContext.SaveChangesAsync();

        (await this.updateDataService.SaveChangesAsync()).chara_list.Should().BeNull();
    }

    private void AssertOnlyContains<TNetwork>(IEnumerable<TNetwork> member, IDbPlayerData dbEntity)
    {
        member
            .Should()
            .ContainSingle()
            .And.ContainEquivalentOf(this.mapper.Map<TNetwork>(dbEntity));
    }
}
