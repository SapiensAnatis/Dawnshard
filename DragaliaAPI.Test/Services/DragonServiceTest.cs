using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Test.Utils;
using MockQueryable.Moq;
using NSubstitute;
using static DragaliaAPI.Test.UnitTestUtils;

namespace DragaliaAPI.Test.Services;

public class DragonServiceTest
{
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IUnitRepository> mockUnitRepository;
    private readonly Mock<IInventoryRepository> mockInventoryRepository;
    private readonly Mock<IStoryRepository> mockStoryRepository;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IPaymentService> mockPaymentService;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly Mock<IMissionProgressionService> mockMissionProgressionService;
    private readonly Mock<TimeProvider> mockTimeProvider;

    private readonly DragonService dragonService;

    public DragonServiceTest()
    {
        mockUserDataRepository = new Mock<IUserDataRepository>();
        mockUnitRepository = new Mock<IUnitRepository>();
        mockInventoryRepository = new Mock<IInventoryRepository>();
        mockStoryRepository = new Mock<IStoryRepository>();
        mockUpdateDataService = new Mock<IUpdateDataService>();
        mockPaymentService = new(MockBehavior.Strict);
        mockRewardService = new(MockBehavior.Strict);
        mockMissionProgressionService = new(MockBehavior.Strict);
        mockTimeProvider = new(MockBehavior.Strict);

        dragonService = new DragonService(
            mockUserDataRepository.Object,
            mockUnitRepository.Object,
            mockInventoryRepository.Object,
            mockUpdateDataService.Object,
            mockStoryRepository.Object,
            LoggerTestUtils.Create<DragonService>(),
            mockPaymentService.Object,
            mockRewardService.Object,
            mockMissionProgressionService.Object,
            new ResetHelper(this.mockTimeProvider.Object)
        );

        this.mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task DoDragonGetContactData_ReturnsValidContactData()
    {
        DateTimeOffset lastReset = new ResetHelper(this.mockTimeProvider.Object).LastDailyReset;

        SetupReliabilityMock(
            out List<DbPlayerDragonGift> gifts,
            out DbPlayerMaterial garudaEssence,
            out DbPlayerUserData userData,
            out List<DbPlayerDragonReliability> dragonRels,
            out List<DbPlayerStoryState> stories
        );
        DragonGetContactDataData responseData = await dragonService.DoDragonGetContactData(
            new DragonGetContactDataRequest()
        );

        responseData.Should().NotBeNull();
        responseData.shop_gift_list.Should().NotBeNullOrEmpty();

        responseData.shop_gift_list.Count().Should().Be(5);
        ((DragonGifts)responseData.shop_gift_list.Last().dragon_gift_id)
            .Should()
            .Be(DragonConstants.RotatingGifts[(int)lastReset.DayOfWeek]);
    }

    [Fact]
    public async Task DoGetDragonContactData_RotatingGiftChangesAtReset()
    {
        this.mockInventoryRepository.SetupGet(x => x.DragonGifts)
            .Returns(
                new List<DbPlayerDragonGift>()
                {
                    new() { DragonGiftId = DragonGifts.FloralCirclet, Quantity = 1, },
                    new() { DragonGiftId = DragonGifts.CompellingBook, Quantity = 1, }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        DateTimeOffset wednesday = new DateTimeOffset(2023, 12, 27, 19, 49, 23, TimeSpan.Zero);
        this.mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(wednesday);

        (await this.dragonService.DoDragonGetContactData(new DragonGetContactDataRequest() { }))
            .shop_gift_list.Should()
            .Contain(x => x.dragon_gift_id == (int)DragonGifts.FloralCirclet);

        DateTimeOffset thuBeforeReset = new DateTimeOffset(2023, 12, 28, 01, 49, 23, TimeSpan.Zero);
        this.mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(thuBeforeReset);

        (await this.dragonService.DoDragonGetContactData(new DragonGetContactDataRequest() { }))
            .shop_gift_list.Should()
            .Contain(x => x.dragon_gift_id == (int)DragonGifts.FloralCirclet);

        DateTimeOffset thursday = new DateTimeOffset(2023, 12, 28, 09, 49, 23, TimeSpan.Zero);
        this.mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(thursday);

        (await this.dragonService.DoDragonGetContactData(new DragonGetContactDataRequest() { }))
            .shop_gift_list.Should()
            .Contain(x => x.dragon_gift_id == (int)DragonGifts.CompellingBook);
    }

    [Fact]
    public async Task DoDragonBuyGiftToSendMultiple_DragonGift_ReturnsCorrectData()
    {
        SetupReliabilityMock(
            out List<DbPlayerDragonGift> gifts,
            out DbPlayerMaterial garudaEssence,
            out DbPlayerUserData userData,
            out List<DbPlayerDragonReliability> dragonRels,
            out List<DbPlayerStoryState> stories
        );

        dragonRels.Add(DbPlayerDragonReliabilityFactory.Create(ViewerId, Dragons.Garuda));

        mockMissionProgressionService.Setup(
            x => x.OnDragonBondLevelUp(Dragons.Garuda, UnitElement.Wind, 3, 4)
        );
        mockMissionProgressionService.Setup(
            x => x.OnDragonBondLevelUp(Dragons.Garuda, UnitElement.Wind, 6, 10)
        );

        mockMissionProgressionService.Setup(
            x =>
                x.OnDragonGiftSent(
                    Dragons.Garuda,
                    It.IsIn(DragonGifts.StrawberryTart, DragonGifts.CompellingBook),
                    UnitElement.Wind,
                    1,
                    0
                )
        );

        long startCoin = userData.Coin;
        DragonBuyGiftToSendMultipleData responseData =
            await dragonService.DoDragonBuyGiftToSendMultiple(
                new DragonBuyGiftToSendMultipleRequest()
                {
                    dragon_id = Dragons.Garuda,
                    dragon_gift_id_list = new List<DragonGifts>()
                    {
                        DragonGifts.StrawberryTart,
                        DragonGifts.CompellingBook
                    }
                }
            );

        responseData.Should().NotBeNull();
        responseData.dragon_gift_reward_list.Should().NotBeNullOrEmpty();

        responseData.shop_gift_list.Count().Should().Be(5);

        gifts
            .Where(x => x.DragonGiftId == DragonGifts.StrawberryTart)
            .First()
            .Quantity.Should()
            .Be(0);
        gifts
            .Where(x => x.DragonGiftId == DragonGifts.CompellingBook)
            .First()
            .Quantity.Should()
            .Be(0);

        responseData.dragon_gift_reward_list.Count().Should().Be(2);

        dragonRels[0].Exp.Should().Be(2400);
        dragonRels[0].Level.Should().Be(10);

        mockUserDataRepository.Verify(x => x.UserData);
        mockInventoryRepository.Verify(x => x.DragonGifts);
        mockInventoryRepository.Setup(x => x.GetMaterial(It.IsAny<Materials>()));
        mockMissionProgressionService.VerifyAll();
        mockStoryRepository.VerifyAll();
    }

    [Fact]
    public async Task DoDragonBuyGiftToSendMultiple_DragonGiftNoLevel_ReturnsCorrectData()
    {
        SetupReliabilityMock(
            out List<DbPlayerDragonGift> gifts,
            out DbPlayerMaterial garudaEssence,
            out DbPlayerUserData userData,
            out List<DbPlayerDragonReliability> dragonRels,
            out List<DbPlayerStoryState> stories
        );

        DbPlayerDragonReliability dd = DbPlayerDragonReliabilityFactory.Create(
            ViewerId,
            Dragons.Garuda
        );
        dd.Level = 30;
        dd.Exp = 36300;

        dragonRels.Add(dd);

        mockMissionProgressionService.Setup(
            x => x.OnDragonGiftSent(Dragons.Garuda, DragonGifts.FreshBread, UnitElement.Wind, 1, 0)
        );

        long startCoin = userData.Coin;
        DragonBuyGiftToSendMultipleData responseData =
            await dragonService.DoDragonBuyGiftToSendMultiple(
                new DragonBuyGiftToSendMultipleRequest()
                {
                    dragon_id = Dragons.Garuda,
                    dragon_gift_id_list = new List<DragonGifts>() { DragonGifts.FreshBread }
                }
            );

        responseData.Should().NotBeNull();
        responseData.dragon_gift_reward_list.Should().NotBeNullOrEmpty();

        responseData.shop_gift_list.Count().Should().Be(5);

        gifts.Where(x => x.DragonGiftId == DragonGifts.FreshBread).First().Quantity.Should().Be(0);

        responseData.dragon_gift_reward_list.Count().Should().Be(1);
        responseData
            .dragon_gift_reward_list.First(x => x.dragon_gift_id == DragonGifts.FreshBread)
            .reward_reliability_list.Should()
            .BeEmpty();

        dragonRels[0].Exp.Should().Be(36300);
        dragonRels[0].Level.Should().Be(30);

        mockUserDataRepository.Verify(x => x.UserData);
        mockInventoryRepository.Verify(x => x.DragonGifts);
        mockInventoryRepository.Setup(x => x.GetMaterial(It.IsAny<Materials>()));
        mockMissionProgressionService.VerifyAll();
    }

    [Theory]
    [InlineData(Dragons.Garuda, DragonGifts.FourLeafClover, 10, 10000, 18)]
    [InlineData(Dragons.Puppy, DragonGifts.PupGrub, 10, 2000, 21)]
    public async Task DoDragonSendGiftMultiple_ReturnsExpecedValues(
        Dragons dragon,
        DragonGifts gift,
        int usedQuantity,
        int expectedXp,
        byte expectedLvl
    )
    {
        SetupReliabilityMock(
            out List<DbPlayerDragonGift> gifts,
            out DbPlayerMaterial garudaEssence,
            out DbPlayerUserData userData,
            out List<DbPlayerDragonReliability> dragonRels,
            out List<DbPlayerStoryState> stories
        );

        dragonRels.Add(DbPlayerDragonReliabilityFactory.Create(ViewerId, dragon));

        UnitElement element = MasterAsset.DragonData[dragon].ElementalType;

        mockMissionProgressionService.Setup(
            x => x.OnDragonBondLevelUp(dragon, element, expectedLvl - 1, expectedLvl)
        );

        mockMissionProgressionService.Setup(
            x => x.OnDragonGiftSent(dragon, gift, element, usedQuantity, 0)
        );

        DragonSendGiftMultipleData responseData = await dragonService.DoDragonSendGiftMultiple(
            new DragonSendGiftMultipleRequest()
            {
                dragon_id = dragon,
                dragon_gift_id = gift,
                quantity = usedQuantity
            }
        );

        responseData.Should().NotBeNull();

        responseData.return_gift_list.Should().NotBeNullOrEmpty();

        dragonRels[0].Exp.Should().Be(expectedXp);
        dragonRels[0].Level.Should().Be(expectedLvl);
        mockUnitRepository.VerifyAll();
        mockInventoryRepository.Verify(x => x.DragonGifts);
        mockInventoryRepository.Setup(x => x.GetMaterial(It.IsAny<Materials>()));
        mockMissionProgressionService.VerifyAll();
        if (dragon != Dragons.Puppy && dragonRels[0].Level > 4)
            mockStoryRepository.VerifyAll();
    }

    [Fact]
    public async Task DoBuildup_AddsAugments()
    {
        DbPlayerDragonData dragonData = DbPlayerDragonDataFactory.Create(ViewerId, Dragons.Garuda);
        dragonData.DragonKeyId = 1;

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>() { dragonData };

        mockUnitRepository
            .SetupGet(x => x.Dragons)
            .Returns(dragonDataList.AsQueryable().BuildMock());

        DbPlayerMaterial mat = new DbPlayerMaterial()
        {
            ViewerId = ViewerId,
            MaterialId = Materials.AmplifyingDragonscale,
            Quantity = 100
        };
        mockInventoryRepository
            .SetupGet(x => x.Materials)
            .Returns(
                new List<DbPlayerMaterial>() { mat }
                    .AsQueryable()
                    .BuildMock()
            );

        await dragonService.DoBuildup(
            new DragonBuildupRequest()
            {
                base_dragon_key_id = 1,
                grow_material_list = new List<GrowMaterialList>()
                {
                    new GrowMaterialList()
                    {
                        type = EntityTypes.Material,
                        id = (int)Materials.AmplifyingDragonscale,
                        quantity = 50
                    }
                }
            }
        );

        dragonData.AttackPlusCount.Should().Be(50);
        mat.Quantity.Should().Be(50);

        mockUnitRepository.VerifyAll();
        mockInventoryRepository.VerifyAll();
    }

    [Theory]
    [InlineData(Dragons.Garuda, Materials.SucculentDragonfruit, 10, 35000, 26)]
    [InlineData(Dragons.Fubuki, Materials.Dragonfruit, 30, 4500, 10)]
    public async Task DoBuildup_BuildsUp(
        Dragons dragon,
        Materials upgradeMat,
        int usedQuantity,
        int expectedXp,
        byte expectedLvl
    )
    {
        DbPlayerDragonData dragonData = DbPlayerDragonDataFactory.Create(ViewerId, dragon);
        dragonData.DragonKeyId = 1;

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>() { dragonData };

        mockUnitRepository
            .SetupGet(x => x.Dragons)
            .Returns(dragonDataList.AsQueryable().BuildMock());

        UnitElement element = MasterAsset.DragonData[dragon].ElementalType;

        mockMissionProgressionService.Setup(
            x => x.OnDragonLevelUp(dragon, element, expectedLvl - 1, expectedLvl)
        );

        DbPlayerMaterial mat = new DbPlayerMaterial()
        {
            ViewerId = ViewerId,
            MaterialId = upgradeMat,
            Quantity = usedQuantity
        };
        mockInventoryRepository
            .SetupGet(x => x.Materials)
            .Returns(
                new List<DbPlayerMaterial>() { mat }
                    .AsQueryable()
                    .BuildMock()
            );

        await dragonService.DoBuildup(
            new DragonBuildupRequest()
            {
                base_dragon_key_id = 1,
                grow_material_list = new List<GrowMaterialList>()
                {
                    new GrowMaterialList()
                    {
                        type = EntityTypes.Material,
                        id = (int)upgradeMat,
                        quantity = usedQuantity
                    }
                }
            }
        );
        dragonData.Exp.Should().Be(expectedXp);
        dragonData.Level.Should().Be(expectedLvl);
        mat.Quantity.Should().Be(0);

        mockUnitRepository.VerifyAll();
        mockMissionProgressionService.VerifyAll();
        mockInventoryRepository.VerifyAll();
    }

    [Fact]
    public async Task DoDragonResetPlusCount_ResetsPlusCount()
    {
        DbPlayerDragonData dragonData = DbPlayerDragonDataFactory.Create(ViewerId, Dragons.Garuda);
        dragonData.DragonKeyId = 1;
        dragonData.AttackPlusCount = 50;

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>() { dragonData };

        mockUnitRepository
            .SetupGet(x => x.Dragons)
            .Returns(dragonDataList.AsQueryable().BuildMock());

        DbPlayerMaterial mat = new DbPlayerMaterial()
        {
            ViewerId = ViewerId,
            MaterialId = Materials.AmplifyingDragonscale,
            Quantity = 0
        };

        mockPaymentService
            .Setup(
                x =>
                    x.ProcessPayment(
                        PaymentTypes.Coin,
                        null,
                        DragonConstants.AugmentResetCost * dragonData.AttackPlusCount
                    )
            )
            .Returns(Task.CompletedTask);

        mockRewardService
            .Setup(
                x =>
                    x.GrantReward(
                        new Entity(
                            EntityTypes.Material,
                            (int)Materials.AmplifyingDragonscale,
                            50,
                            null,
                            null,
                            null
                        )
                    )
            )
            .ReturnsAsync(RewardGrantResult.Added);

        mockRewardService.Setup(x => x.GetEntityResult()).Returns(new EntityResult());

        await dragonService.DoDragonResetPlusCount(
            new DragonResetPlusCountRequest()
            {
                dragon_key_id = 1,
                plus_count_type = PlusCountType.Atk
            }
        );

        mockRewardService.VerifyAll();
        mockPaymentService.VerifyAll();
        mockUserDataRepository.VerifyAll();
        mockUnitRepository.VerifyAll();
        mockInventoryRepository.VerifyAll();
    }

    [Theory]
    [InlineData(1, DragonLimitBreakMatTypes.Dupe, 2, 2, 1)]
    [InlineData(4, DragonLimitBreakMatTypes.Stone, 5, 5, 2)]
    [InlineData(2, DragonLimitBreakMatTypes.Spheres, 3, 3, 1)]
    [InlineData(5, DragonLimitBreakMatTypes.SpheresLB5, 6, 6, 2)]
    public async Task DoDragonLimitBreak_LimitBreaks(
        byte limitBreakNr,
        DragonLimitBreakMatTypes lbMatType,
        byte expectedA1Level,
        byte expectedA2Level,
        byte expectedS1Level
    )
    {
        Materials targetMat =
            lbMatType == DragonLimitBreakMatTypes.Stone
                ? Materials.SunlightStone
                : Materials.GarudasEssence;

        DbPlayerDragonData dragonDataSacrifice = DbPlayerDragonDataFactory.Create(
            ViewerId,
            Dragons.Garuda
        );
        dragonDataSacrifice.DragonKeyId = 2;
        dragonDataSacrifice.LimitBreakCount = 0;

        DbPlayerDragonData dragonData = DbPlayerDragonDataFactory.Create(ViewerId, Dragons.Garuda);
        dragonData.DragonKeyId = 1;
        dragonData.LimitBreakCount = (byte)(limitBreakNr - 1);

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>()
        {
            dragonData,
            dragonDataSacrifice
        };

        mockUnitRepository
            .SetupGet(x => x.Dragons)
            .Returns(dragonDataList.AsQueryable().BuildMock());
        mockUnitRepository
            .Setup(x => x.RemoveDragons(It.IsAny<IEnumerable<long>>()))
            .Callback(() => dragonDataList.RemoveAll(x => x.DragonKeyId == 2));

        DbPlayerMaterial mat = new DbPlayerMaterial()
        {
            ViewerId = ViewerId,
            MaterialId = targetMat,
            Quantity = 500
        };

        mockInventoryRepository.Setup(x => x.GetMaterial(targetMat)).ReturnsAsync(mat);

        await dragonService.DoDragonLimitBreak(
            new DragonLimitBreakRequest()
            {
                base_dragon_key_id = 1,
                limit_break_grow_list = new List<LimitBreakGrowList>()
                {
                    new LimitBreakGrowList()
                    {
                        limit_break_item_type = (int)lbMatType,
                        limit_break_count = limitBreakNr,
                        target_id = (ulong)(lbMatType == DragonLimitBreakMatTypes.Dupe ? 2 : 0)
                    }
                }
            }
        );

        dragonData.LimitBreakCount.Should().Be(limitBreakNr);
        dragonData.Ability1Level.Should().Be(expectedA1Level);
        dragonData.Ability2Level.Should().Be(expectedA2Level);
        dragonData.Skill1Level.Should().Be(expectedS1Level);
        mockUnitRepository.Verify(x => x.Dragons);
        if (lbMatType == DragonLimitBreakMatTypes.Dupe)
        {
            dragonDataList.Should().NotContain(dragonDataSacrifice);

            mockUnitRepository.Verify(x => x.RemoveDragons(It.IsAny<IEnumerable<long>>()));
        }
        else
        {
            mat.Quantity.Should().BeLessThan(500);

            mockInventoryRepository.Verify(x => x.GetMaterial(targetMat));
        }
    }

    [Fact]
    public async Task DoDragonLock_Locks()
    {
        DbPlayerDragonData dragonData = DbPlayerDragonDataFactory.Create(ViewerId, Dragons.Garuda);
        dragonData.DragonKeyId = 1;

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>() { dragonData };

        mockUnitRepository
            .SetupGet(x => x.Dragons)
            .Returns(dragonDataList.AsQueryable().BuildMock());

        await dragonService.DoDragonSetLock(
            new DragonSetLockRequest() { dragon_key_id = 1, is_lock = true }
        );

        dragonData.IsLock.Should().BeTrue();

        mockUnitRepository.VerifyAll();
    }

    [Fact]
    public async Task DoDragonSell_AddsCoinAndDew()
    {
        DbPlayerUserData userData = new DbPlayerUserData()
        {
            ViewerId = ViewerId,
            Coin = 0,
            DewPoint = 0
        };

        IQueryable<DbPlayerUserData> userDataList = new List<DbPlayerUserData>() { userData }
            .AsQueryable()
            .BuildMock();

        mockUserDataRepository.SetupGet(x => x.UserData).Returns(userDataList);

        DbPlayerDragonData dragonData = DbPlayerDragonDataFactory.Create(ViewerId, Dragons.Garuda);
        dragonData.DragonKeyId = 1;

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>() { dragonData };

        mockUnitRepository
            .SetupGet(x => x.Dragons)
            .Returns(dragonDataList.AsQueryable().BuildMock());
        mockUnitRepository
            .Setup(x => x.RemoveDragons(new List<long>() { 1 }))
            .Callback(() => dragonDataList.RemoveAll(x => x.DragonKeyId == 1));

        DragonSellData response = await dragonService.DoDragonSell(
            new DragonSellRequest() { dragon_key_id_list = new List<ulong>() { 1 } }
        );

        dragonDataList.Count().Should().Be(0);
        userData.Coin.Should().Be(MasterAsset.DragonData.Get(Dragons.Garuda).SellCoin);
        userData.DewPoint.Should().Be(MasterAsset.DragonData.Get(Dragons.Garuda).SellDewPoint);

        mockUnitRepository.VerifyAll();
        mockUserDataRepository.VerifyAll();
    }

    private void SetupReliabilityMock(
        out List<DbPlayerDragonGift> dbPlayerDragonGifts,
        out DbPlayerMaterial garudaEssence,
        out DbPlayerUserData userData,
        out List<DbPlayerDragonReliability> userDragonRels,
        out List<DbPlayerStoryState> stories
    )
    {
        dbPlayerDragonGifts = new List<DbPlayerDragonGift>();
        foreach (DragonGifts gift in Enum.GetValues<DragonGifts>())
        {
            int quantity = gift < DragonGifts.FourLeafClover ? 1 : 100;
            dbPlayerDragonGifts.Add(
                new DbPlayerDragonGift()
                {
                    ViewerId = ViewerId,
                    DragonGiftId = gift,
                    Quantity = quantity
                }
            );
        }

        mockInventoryRepository
            .SetupGet(x => x.DragonGifts)
            .Returns(dbPlayerDragonGifts.AsQueryable().BuildMock());

        garudaEssence = new DbPlayerMaterial()
        {
            ViewerId = ViewerId,
            MaterialId = Materials.GarudasEssence,
            Quantity = 0
        };

        mockInventoryRepository
            .Setup(x => x.GetMaterial(It.IsAny<Materials>()))
            .ReturnsAsync(garudaEssence);

        userData = new DbPlayerUserData() { ViewerId = ViewerId, Coin = 100000 };

        IQueryable<DbPlayerUserData> userDataList = new List<DbPlayerUserData>() { userData }
            .AsQueryable()
            .BuildMock();

        mockUserDataRepository.SetupGet(x => x.UserData).Returns(userDataList);

        userDragonRels = new List<DbPlayerDragonReliability>();

        mockUnitRepository
            .SetupGet(x => x.DragonReliabilities)
            .Returns(userDragonRels.AsQueryable().BuildMock());

        stories = new List<DbPlayerStoryState>();

        mockStoryRepository.SetupGet(x => x.Stories).Returns(stories.AsQueryable().BuildMock());
        mockStoryRepository
            .Setup(
                x =>
                    x.GetOrCreateStory(
                        StoryTypes.Dragon,
                        MasterAsset.DragonStories.Get((int)Dragons.Garuda).storyIds[0]
                    )
            )
            .ReturnsAsync(
                new DbPlayerStoryState()
                {
                    ViewerId = ViewerId,
                    State = 0,
                    StoryId = MasterAsset.DragonStories.Get((int)Dragons.Garuda).storyIds[0],
                    StoryType = StoryTypes.Dragon
                }
            );
    }
}
