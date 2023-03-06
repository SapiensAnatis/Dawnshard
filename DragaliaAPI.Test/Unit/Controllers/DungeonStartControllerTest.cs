using AutoMapper;
using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Services;
using DragaliaAPI.Models.Generated;
using static DragaliaAPI.Test.TestUtils;
using DragaliaAPI.Database.Entities;
using MockQueryable.Moq;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DragaliaAPI.Database.Entities.Scaffold;

namespace DragaliaAPI.Test.Unit.Controllers;

public class DungeonStartControllerTest
{
    private readonly DungeonStartController dungeonStartController;
    private readonly Mock<IPartyRepository> mockPartyRepository;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IUnitRepository> mockUnitRepository;
    private readonly Mock<IQuestRepository> mockQuestRepository;
    private readonly Mock<IDungeonService> mockDungeonService;
    private readonly Mock<IHelperService> mockHelperService;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IBonusService> mockBonusService;
    private readonly Mock<IWeaponRepository> mockWeaponRepository;
    private readonly IMapper mapper;
    private readonly Mock<ILogger<DungeonStartController>> mockLogger;

    private const int questId = 100010103;

    public DungeonStartControllerTest()
    {
        this.mockPartyRepository = new(MockBehavior.Strict);
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockUnitRepository = new(MockBehavior.Strict);
        this.mockQuestRepository = new(MockBehavior.Strict);
        this.mockDungeonService = new(MockBehavior.Strict);
        this.mockHelperService = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);
        this.mockBonusService = new(MockBehavior.Strict);
        this.mockLogger = new();
        this.mockWeaponRepository = new(MockBehavior.Strict);

        this.mapper = new MapperConfiguration(
            cfg => cfg.AddMaps(typeof(Program).Assembly)
        ).CreateMapper();

        this.dungeonStartController = new(
            mockPartyRepository.Object,
            mockUserDataRepository.Object,
            mockUnitRepository.Object,
            mockQuestRepository.Object,
            mockDungeonService.Object,
            mockHelperService.Object,
            mockUpdateDataService.Object,
            mockBonusService.Object,
            mapper,
            mockWeaponRepository.Object,
            mockLogger.Object
        );

        dungeonStartController.SetupMockContext();

        this.mockBonusService.Setup(x => x.GetBonusList()).ReturnsAsync(new FortBonusList());

        /* this.mockPartyRepository
             .Setup(x => x.Get(DeviceAccountId))
             .Returns(new List<DbParty>()
                 {
                     new()
                     {
                         DeviceAccountId = DeviceAccountId,
                         PartyName = "Party",
                         PartyNo = 1,
                         Units = new List<DbPartyUnit>()
                         {
                             new() { UnitNo = 1, CharaId = Charas.ThePrince }
                         }
                     },
                     new()
                     {
                         DeviceAccountId = DeviceAccountId,
                         PartyName = "Party",
                         PartyNo = 2,
                         Units = new List<DbPartyUnit>()
                         {
                             new() { UnitNo = 1, CharaId = Charas.Elisanne }
                         }
                     }
                 }.AsQueryable().BuildMock());*/

        this.mockUnitRepository
            .Setup(
                x => x.BuildDetailedPartyUnit(DeviceAccountId, It.IsAny<IQueryable<DbPartyUnit>>())
            )
            .Returns(new List<DbDetailedPartyUnit>()
                {
                    new()
                    {
                        Position = 1,
                        DeviceAccountId = DeviceAccountId,
                        CharaData = new(DeviceAccountId, Charas.ThePrince),
                        CrestSlotType1CrestList = new List<DbAbilityCrest>()
                        {
                            new()
                            {
                                DeviceAccountId = DeviceAccountId,
                                AbilityCrestId = AbilityCrests.ABouquet
                            }
                        },
                        CrestSlotType2CrestList = new List<DbAbilityCrest>()
                        {
                            new()
                            {
                                DeviceAccountId = DeviceAccountId,
                                AbilityCrestId = AbilityCrests.ABouquet
                            }
                        },
                        CrestSlotType3CrestList = new List<DbAbilityCrest>()
                        {
                            new()
                            {
                                DeviceAccountId = DeviceAccountId,
                                AbilityCrestId = AbilityCrests.ABriefRepose
                            }
                        },
                        EditSkill1CharaData = new() { CharaId = Charas.Vida },
                        EditSkill2CharaData = new() { },
                        DragonData = new()
                        {
                            DeviceAccountId = DeviceAccountId,
                            DragonId = Dragons.Midgardsormr
                        },
                        DragonReliabilityLevel = 30,
                        WeaponBodyData = new()
                        {
                            DeviceAccountId = DeviceAccountId,
                            WeaponBodyId = WeaponBodies.PrimevalThunder
                        }
                    }
                }.AsQueryable().BuildMock());

        this.mockHelperService
            .Setup(x => x.GetHelpers())
            .ReturnsAsync(
                new QuestGetSupportUserListData()
                {
                    support_user_list = new List<UserSupportList>() { TestData.supportListEuden },
                    support_user_detail_list = new List<AtgenSupportUserDetailList>()
                    {
                        new() { viewer_id = 1000, is_friend = true, },
                    }
                }
            );

        this.mockHelperService
            .Setup(
                x =>
                    x.BuildHelperData(
                        It.Is<UserSupportList>(x => x.viewer_id == 1000),
                        It.Is<AtgenSupportUserDetailList>(x => x.viewer_id == 1000)
                    )
            )
            .Returns(new AtgenSupportData() { viewer_id = 1000 });

        this.mockDungeonService
            .Setup(x => x.StartDungeon(It.Is<DungeonSession>(x => x.QuestData.Id == questId)))
            .ReturnsAsync("key");
    }

    [Fact]
    public async Task Start_QuestAlreadyCompleted_DoesNotUpdateQuestInDb()
    {
        this.mockQuestRepository
            .Setup(x => x.GetQuests(DeviceAccountId))
            .Returns(new List<DbQuest>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        QuestId = questId,
                        State = 3
                    }
                }.AsQueryable().BuildMock());

        this.mockUpdateDataService
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList());

        this.mockPartyRepository
            .Setup(x => x.GetPartyUnits(DeviceAccountId, new List<int>() { 1 }))
            .Returns(new List<DbPartyUnit>().AsQueryable().BuildMock());

        ActionResult<DragaliaResponse<object>> response = await this.dungeonStartController.Start(
            new DungeonStartStartRequest()
            {
                quest_id = questId,
                party_no_list = new List<int>() { 1 },
            }
        );

        DungeonStartStartData? data = response.GetData<DungeonStartStartData>();
        data.Should().NotBeNull();

        data!.ingame_data.dungeon_key.Should().Be("key");
        data!.ingame_data.viewer_id.Should().Be(1);
        data!.ingame_data.quest_id.Should().Be(questId);

        // Automapper complaining about this for some reason
        // PartyUnitList expectedUnit = mapper.Map<PartyUnitList>(detailedUnit);
        // response3!.ingame_data.party_info.party_unit_list
        //     .Should()
        //     .ContainEquivalentOf(expectedUnit);

        this.mockPartyRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockUnitRepository.VerifyAll();
        this.mockQuestRepository.VerifyAll();
        this.mockDungeonService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task Start_QuestNotCompleted_UpdatesQuestInDb()
    {
        this.mockQuestRepository
            .Setup(x => x.GetQuests(DeviceAccountId))
            .Returns(new List<DbQuest>().AsQueryable().BuildMock());

        this.mockQuestRepository
            .Setup(x => x.UpdateQuestState(DeviceAccountId, questId, 2))
            .Returns(Task.CompletedTask);

        this.mockUpdateDataService
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList());

        this.mockPartyRepository
            .Setup(x => x.GetPartyUnits(DeviceAccountId, new List<int>() { 1 }))
            .Returns(new List<DbPartyUnit>().AsQueryable().BuildMock());

        ActionResult<DragaliaResponse<object>> response = await this.dungeonStartController.Start(
            new DungeonStartStartRequest()
            {
                quest_id = questId,
                party_no_list = new List<int>() { 1 },
            }
        );

        DungeonStartStartData? data = response.GetData<DungeonStartStartData>();
        data.Should().NotBeNull();

        this.mockPartyRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockUnitRepository.VerifyAll();
        this.mockQuestRepository.VerifyAll();
        this.mockDungeonService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task Start_MultipleParties_BuildsCorrectPartyList()
    {
        this.mockQuestRepository
            .Setup(x => x.GetQuests(DeviceAccountId))
            .Returns(new List<DbQuest>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        QuestId = questId,
                        State = 3
                    }
                }.AsQueryable().BuildMock());

        this.mockUpdateDataService
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList());

        this.mockPartyRepository
            .Setup(x => x.GetPartyUnits(DeviceAccountId, new List<int>() { 1, 2 }))
            .Returns(new List<DbPartyUnit>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        CharaId = Charas.ThePrince,
                        UnitNo = 1,
                        PartyNo = 1
                    },
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        CharaId = Charas.Elisanne,
                        UnitNo = 1,
                        PartyNo = 2
                    }
                }.AsQueryable().BuildMock());

        this.mockUnitRepository
            .Setup(
                x => x.BuildDetailedPartyUnit(DeviceAccountId, It.IsAny<IQueryable<DbPartyUnit>>())
            )
            .Returns(new List<DbDetailedPartyUnit>()
                {
                    new()
                    {
                        Position = 1,
                        DeviceAccountId = DeviceAccountId,
                        CharaData = new(DeviceAccountId, Charas.ThePrince),
                        CrestSlotType1CrestList = new List<DbAbilityCrest>()
                        {
                            new()
                            {
                                DeviceAccountId = DeviceAccountId,
                                AbilityCrestId = AbilityCrests.ABouquet
                            }
                        },
                        CrestSlotType2CrestList = new List<DbAbilityCrest>()
                        {
                            new()
                            {
                                DeviceAccountId = DeviceAccountId,
                                AbilityCrestId = AbilityCrests.ABouquet
                            }
                        },
                        CrestSlotType3CrestList = new List<DbAbilityCrest>()
                        {
                            new()
                            {
                                DeviceAccountId = DeviceAccountId,
                                AbilityCrestId = AbilityCrests.ABriefRepose
                            }
                        },
                        EditSkill1CharaData = new() { CharaId = Charas.Vida },
                        EditSkill2CharaData = new() { },
                        DragonData = new()
                        {
                            DeviceAccountId = DeviceAccountId,
                            DragonId = Dragons.Midgardsormr
                        },
                        DragonReliabilityLevel = 30,
                        WeaponBodyData = new()
                        {
                            DeviceAccountId = DeviceAccountId,
                            WeaponBodyId = WeaponBodies.Infernoblaze
                        }
                    },
                    new()
                    {
                        Position = 1,
                        DeviceAccountId = DeviceAccountId,
                        CharaData = new(DeviceAccountId, Charas.Elisanne),
                        CrestSlotType1CrestList = new List<DbAbilityCrest>()
                        {
                            new()
                            {
                                DeviceAccountId = DeviceAccountId,
                                AbilityCrestId = AbilityCrests.SweetSurprise,
                            }
                        },
                        CrestSlotType2CrestList = new List<DbAbilityCrest>()
                        {
                            new()
                            {
                                DeviceAccountId = DeviceAccountId,
                                AbilityCrestId = AbilityCrests.FromWhenceHeComes,
                            }
                        },
                        CrestSlotType3CrestList = new List<DbAbilityCrest>()
                        {
                            new()
                            {
                                DeviceAccountId = DeviceAccountId,
                                AbilityCrestId = AbilityCrests.AnAncientOath
                            }
                        },
                        EditSkill1CharaData = new() { CharaId = Charas.Isaac },
                        EditSkill2CharaData = new() { },
                        DragonData = new()
                        {
                            DeviceAccountId = DeviceAccountId,
                            DragonId = Dragons.GalaBeastCiella
                        },
                        DragonReliabilityLevel = 30,
                        WeaponBodyData = new()
                        {
                            DeviceAccountId = DeviceAccountId,
                            WeaponBodyId = WeaponBodies.MegaLance
                        }
                    }
                }.AsQueryable().BuildMock());

        this.mockWeaponRepository
            .Setup(x => x.GetPassiveAbilities(WeaponBodies.Infernoblaze))
            .Returns(new List<DbWeaponPassiveAbility>()
                {
                    new() { DeviceAccountId = DeviceAccountId, WeaponPassiveAbilityId = 1 }
                }.AsQueryable().BuildMock());

        this.mockWeaponRepository
            .Setup(x => x.GetPassiveAbilities(WeaponBodies.MegaLance))
            .Returns(new List<DbWeaponPassiveAbility>() { }.AsQueryable().BuildMock());

        ActionResult<DragaliaResponse<object>> response = await this.dungeonStartController.Start(
            new DungeonStartStartRequest()
            {
                quest_id = questId,
                party_no_list = new List<int>() { 1, 2 },
            }
        );

        DungeonStartStartData? data = response.GetData<DungeonStartStartData>();
        data.Should().NotBeNull();

        data!.ingame_data.party_info.party_unit_list
            .ElementAt(0)
            .chara_data!.chara_id.Should()
            .Be(Charas.ThePrince);
        data!.ingame_data.party_info.party_unit_list.ElementAt(0).position.Should().Be(1);
        data!.ingame_data.party_info.party_unit_list
            .ElementAt(0)
            .game_weapon_passive_ability_list.Should()
            .BeEquivalentTo(
                new List<WeaponPassiveAbilityList>() { new() { weapon_passive_ability_id = 1 } }
            );

        data!.ingame_data.party_info.party_unit_list
            .ElementAt(1)
            .chara_data!.chara_id.Should()
            .Be(Charas.Elisanne);
        data!.ingame_data.party_info.party_unit_list.ElementAt(1).position.Should().Be(5);
        data!.ingame_data.party_info.party_unit_list
            .ElementAt(1)
            .game_weapon_passive_ability_list.Should()
            .BeEquivalentTo(new List<WeaponPassiveAbilityList>() { });

        this.mockPartyRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockUnitRepository.VerifyAll();
        this.mockQuestRepository.VerifyAll();
        this.mockDungeonService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task StartDungeonWithSupportIncludesSupportData()
    {
        this.mockQuestRepository
            .Setup(x => x.GetQuests(DeviceAccountId))
            .Returns(new List<DbQuest>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        QuestId = questId,
                        State = 3
                    }
                }.AsQueryable().BuildMock());

        this.mockUpdateDataService
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList());

        this.mockPartyRepository
            .Setup(x => x.GetPartyUnits(DeviceAccountId, new List<int>() { 1 }))
            .Returns(new List<DbPartyUnit>() { }.AsQueryable().BuildMock());

        ActionResult<DragaliaResponse<object>> response = await this.dungeonStartController.Start(
            new DungeonStartStartRequest()
            {
                quest_id = questId,
                party_no_list = new List<int>() { 1 },
                support_viewer_id = 1000
            }
        );

        DungeonStartStartData? data = response.GetData<DungeonStartStartData>();
        data.Should().NotBeNull();

        data!.ingame_data.party_info.support_data
            .Should()
            .BeEquivalentTo(new AtgenSupportData() { viewer_id = 1000 });

        this.mockPartyRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockUnitRepository.VerifyAll();
        this.mockQuestRepository.VerifyAll();
        this.mockDungeonService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task StartDungeonWithoutSupportDoesntIncludeSupportData()
    {
        this.mockQuestRepository
            .Setup(x => x.GetQuests(DeviceAccountId))
            .Returns(new List<DbQuest>()
                {
                    new()
                    {
                        DeviceAccountId = DeviceAccountId,
                        QuestId = questId,
                        State = 3
                    }
                }.AsQueryable().BuildMock());

        this.mockUpdateDataService
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList());

        this.mockPartyRepository
            .Setup(x => x.GetPartyUnits(DeviceAccountId, new List<int>() { 1 }))
            .Returns(new List<DbPartyUnit>() { }.AsQueryable().BuildMock());

        ActionResult<DragaliaResponse<object>> response = await this.dungeonStartController.Start(
            new DungeonStartStartRequest()
            {
                quest_id = questId,
                party_no_list = new List<int>() { 1 },
                support_viewer_id = 0
            }
        );

        DungeonStartStartData? data = response.GetData<DungeonStartStartData>();
        data.Should().NotBeNull();

        data!.ingame_data.party_info.support_data.Should().BeNull();

        this.mockPartyRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockUnitRepository.VerifyAll();
        this.mockQuestRepository.VerifyAll();
        this.mockDungeonService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }
}
