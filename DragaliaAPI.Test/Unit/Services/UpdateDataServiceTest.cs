using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Services;
using System.Text.Json;
using Xunit.Abstractions;

namespace DragaliaAPI.Test.Unit.Services;

public class UpdateDataServiceTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly ITestOutputHelper output;

    private readonly ICharaDataService charaDataService;
    private readonly IDragonDataService dragonDataService;
    private readonly IMapper mapper;
    private readonly IUpdateDataService updateDataService;

    public UpdateDataServiceTest(DbTestFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.output = output;

        this.charaDataService = new CharaDataService();
        this.dragonDataService = new DragonDataService();

        this.mapper = new MapperConfiguration(
            cfg => cfg.AddMaps(typeof(Program).Assembly)
        ).CreateMapper();
        this.updateDataService = new UpdateDataService(this.fixture.ApiContext, this.mapper);
    }

    [Fact]
    public void GetUpdateDataList_PopulatesAll()
    {
        string deviceAccountId = "new id";

        DbPlayerUserData userData = DbSavefileUserDataFactory.Create(deviceAccountId);
        DbPlayerCharaData charaData = DbPlayerCharaDataFactory.Create(
            deviceAccountId,
            this.charaDataService.GetData(Charas.GalaLeonidas)
        );
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
        DbPlayerStoryState storyState =
            new()
            {
                DeviceAccountId = deviceAccountId,
                State = 1,
                StoryId = 2,
                StoryType = StoryTypes.Quest
            };

        this.fixture.ApiContext.AddRange(
            new List<IDbHasAccountId>()
            {
                userData,
                charaData,
                dragonData,
                reliabilityData,
                partyData,
                storyState
            }
        );

        UpdateDataList list = this.updateDataService.GetUpdateDataList(deviceAccountId);

        list.user_data.Should().BeEquivalentTo(this.mapper.Map<UserData>(userData));
        list.chara_list
            .Should()
            .BeEquivalentTo(new List<Chara>() { this.mapper.Map<Chara>(charaData) });
        list.dragon_list
            .Should()
            .BeEquivalentTo(new List<Dragon>() { this.mapper.Map<Dragon>(dragonData) });
        list.dragon_reliability_list
            .Should()
            .BeEquivalentTo(
                new List<DragonReliability>()
                {
                    this.mapper.Map<DragonReliability>(reliabilityData)
                }
            );
        list.party_list
            .Should()
            .BeEquivalentTo(new List<Party>() { this.mapper.Map<Party>(partyData) });
        list.quest_story_list
            .Should()
            .BeEquivalentTo(new List<QuestStory>() { this.mapper.Map<QuestStory>(storyState) });

        this.output.WriteLine(
            "{0}",
            JsonSerializer.Serialize(list, new JsonSerializerOptions() { WriteIndented = true })
        );
    }

    [Fact]
    public void GetUpdateDataList_NullIfNoUpdates()
    {
        UpdateDataList list = this.updateDataService.GetUpdateDataList("id");

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
        this.fixture.ApiContext.Add(
            DbPlayerCharaDataFactory.Create(
                "id 1",
                this.charaDataService.GetData(Charas.GalaZethia)
            )
        );

        this.updateDataService.GetUpdateDataList("id 2").chara_list.Should().BeNull();
    }

    [Fact]
    public void GetUpdateDataList_NullAfterSave()
    {
        this.fixture.ApiContext.Add(
            DbPlayerCharaDataFactory.Create(
                "id",
                this.charaDataService.GetData(Charas.HalloweenLowen)
            )
        );

        this.fixture.ApiContext.SaveChanges();

        this.updateDataService.GetUpdateDataList("id").chara_list.Should().BeNull();
    }
}
