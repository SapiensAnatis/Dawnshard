using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Models.Generated;
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

        this.fixture.ApiContext.AddRange(
            new List<IDbHasAccountId>()
            {
                userData,
                charaData,
                dragonData,
                reliabilityData,
                partyData,
                storyState,
                materialData,
                questData
            }
        );

        UpdateDataList list = this.updateDataService.GetUpdateDataList(deviceAccountId);

        list.user_data.Should().BeEquivalentTo(this.mapper.Map<UserData>(userData));
        list.chara_list
            .Should()
            .BeEquivalentTo(new List<CharaList>() { this.mapper.Map<CharaList>(charaData) });
        list.dragon_list
            .Should()
            .BeEquivalentTo(new List<DragonList>() { this.mapper.Map<DragonList>(dragonData) });
        list.dragon_reliability_list
            .Should()
            .BeEquivalentTo(
                new List<DragonReliabilityList>()
                {
                    this.mapper.Map<DragonReliabilityList>(reliabilityData)
                }
            );
        list.party_list
            .Should()
            .BeEquivalentTo(new List<PartyList>() { this.mapper.Map<PartyList>(partyData) });
        list.quest_story_list
            .Should()
            .BeEquivalentTo(
                new List<QuestStoryList>() { this.mapper.Map<QuestStoryList>(storyState) }
            );
        list.quest_list
            .Should()
            .BeEquivalentTo(new List<QuestList> { this.mapper.Map<QuestList>(questData) });
        list.material_list
            .Should()
            .BeEquivalentTo(
                new List<MaterialList>() { this.mapper.Map<MaterialList>(materialData) }
            );

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
                DbPlayerDragonDataFactory.Create("id", Dragons.Arsene),
                DbPlayerDragonDataFactory.Create("id", Dragons.GalaBeastVolk),
                DbPlayerDragonDataFactory.Create("id", Dragons.HighZodiark)
            }
        );

        UpdateDataList list = this.updateDataService.GetUpdateDataList("id");

        list.dragon_list.Should().NotBeNullOrEmpty();
        list.dragon_list!.Select(x => x.dragon_key_id).Should().OnlyHaveUniqueItems();
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
