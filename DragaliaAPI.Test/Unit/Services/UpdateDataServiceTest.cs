using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Database.Test;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using DragaliaAPI.Test.Utils;
using Xunit.Abstractions;
using static DragaliaAPI.Test.Utils.IdentityTestUtils;

namespace DragaliaAPI.Test.Unit.Services;

public class UpdateDataServiceTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly ITestOutputHelper output;

    private readonly IMapper mapper;
    private readonly IUpdateDataService updateDataService;

    public UpdateDataServiceTest(DbTestFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.output = output;

        this.mapper = new MapperConfiguration(
            cfg => cfg.AddMaps(typeof(Program).Assembly)
        ).CreateMapper();
        this.updateDataService = new UpdateDataService(
            this.fixture.ApiContext,
            this.mapper,
            IdentityTestUtils.MockPlayerDetailsService.Object
        );

        TestUtils.ApplyDateTimeAssertionOptions();
    }

    [Fact]
    public void GetUpdateDataList_PopulatesAll()
    {
        string deviceAccountId = "new id";

        DbPlayerUserData userData = new(deviceAccountId);

        DbPlayerCharaData charaData = new(deviceAccountId, Charas.GalaLeonidas);

        DbPlayerDragonData dragonData = DbPlayerDragonDataFactory.Create(
            deviceAccountId,
            Dragons.DreadkingRathalos
        );
        DbPlayerDragonReliability reliabilityData = DbPlayerDragonReliabilityFactory.Create(
            deviceAccountId,
            Dragons.DreadkingRathalos
        );

        DbParty partyData =
            new()
            {
                DeviceAccountId = deviceAccountId,
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
                DeviceAccountId = deviceAccountId,
                State = StoryState.Read,
                StoryId = 2,
                StoryType = StoryTypes.Quest
            };
        DbPlayerStoryState charaStoryState =
            new()
            {
                DeviceAccountId = deviceAccountId,
                State = StoryState.Read,
                StoryId = 4,
                StoryType = StoryTypes.Chara,
            };
        DbPlayerStoryState castleStoryState =
            new()
            {
                DeviceAccountId = deviceAccountId,
                State = StoryState.Unlocked,
                StoryId = 6,
                StoryType = StoryTypes.Castle,
            };

        DbPlayerStoryState dragonStoryState =
            new()
            {
                DeviceAccountId = deviceAccountId,
                State = StoryState.Unlocked,
                StoryId = 8,
                StoryType = StoryTypes.Dragon,
            };

        DbPlayerMaterial materialData =
            new()
            {
                DeviceAccountId = deviceAccountId,
                MaterialId = Materials.AlmightyOnesMaskFragment,
                Quantity = 10
            };

        DbQuest questData =
            new()
            {
                DeviceAccountId = deviceAccountId,
                QuestId = 100010104,
                IsMissionClear1 = true,
                IsMissionClear2 = true,
                IsMissionClear3 = true,
                State = 3
            };

        DbFortBuild buildData =
            new()
            {
                DeviceAccountId = deviceAccountId,
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

        this.fixture.ApiContext.AddRange(
            new List<IDbHasAccountId>()
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

        UpdateDataList list = this.updateDataService.GetUpdateDataList(deviceAccountId);

        using AssertionScope scope = new();

        list.user_data.Should().BeEquivalentTo(this.mapper.Map<UserData>(userData));

        AssertOnlyContains<CharaList>(list.chara_list, charaData);

        AssertOnlyContains<DragonList>(list.dragon_list, dragonData);

        AssertOnlyContains<DragonReliabilityList>(list.dragon_reliability_list, reliabilityData);

        AssertOnlyContains<PartyList>(list.party_list, partyData);

        AssertOnlyContains<QuestStoryList>(list.quest_story_list, questStoryState);

        list.unit_story_list
            .Should()
            .ContainEquivalentOf(mapper.Map<UnitStoryList>(charaStoryState))
            .And.ContainEquivalentOf(mapper.Map<UnitStoryList>(dragonStoryState));

        AssertOnlyContains<CastleStoryList>(list.castle_story_list, castleStoryState);

        AssertOnlyContains<QuestList>(list.quest_list, questData);

        AssertOnlyContains<MaterialList>(list.material_list, materialData);

        AssertOnlyContains<BuildList>(list.build_list, buildData);

        this.output.WriteLine(
            "{0}",
            JsonSerializer.Serialize(list, new JsonSerializerOptions() { WriteIndented = true })
        );
    }

    [Fact]
    public void GetUpdateDataList_RetrievesIdentityColumns()
    {
        this.fixture.ApiContext.AddRange(
            new List<IDbHasAccountId>()
            {
                DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.Arsene),
                DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.GalaBeastVolk),
                DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.HighZodiark)
            }
        );

        UpdateDataList list = this.updateDataService.GetUpdateDataList(DeviceAccountId);

        list.dragon_list.Should().NotBeNullOrEmpty();
        list.dragon_list!.Select(x => x.dragon_key_id).Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void GetUpdateDataList_NullIfNoUpdates()
    {
        UpdateDataList list = this.updateDataService.GetUpdateDataList(DeviceAccountId);

        list.user_data.Should().BeNull();
        list.chara_list.Should().BeNull();
        list.dragon_list.Should().BeNull();
        list.dragon_reliability_list.Should().BeNull();
        list.party_list.Should().BeNull();
        list.quest_story_list.Should().BeNull();
    }

    [Fact]
    public void GetUpdateDataList_NoDataFromOtherAccounts()
    {
        this.fixture.ApiContext.PlayerCharaData.Add(new("id 1", Charas.GalaZethia));

        this.updateDataService.GetUpdateDataList("id 2").chara_list.Should().BeNull();
    }

    [Fact]
    public void GetUpdateDataList_NullAfterSave()
    {
        this.fixture.ApiContext.PlayerCharaData.Add(new(DeviceAccountId, Charas.HalloweenLowen));

        this.fixture.ApiContext.SaveChanges();

        this.updateDataService.GetUpdateDataList(DeviceAccountId).chara_list.Should().BeNull();
    }

    private void AssertOnlyContains<TNetwork>(
        IEnumerable<TNetwork> member,
        IDbHasAccountId dbEntity
    )
    {
        member
            .Should()
            .ContainSingle()
            .And.ContainEquivalentOf(this.mapper.Map<TNetwork>(dbEntity));
    }
}
