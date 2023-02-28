using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Test.Utils;
using MockQueryable.Moq;
using static DragaliaAPI.Test.TestUtils;

namespace DragaliaAPI.Test.Unit.Services;

public class DragonServiceTest
{
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IUnitRepository> mockUnitRepository;
    private readonly Mock<IInventoryRepository> mockInventoryRepository;
    private readonly Mock<IStoryRepository> mockStoryRepository;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;

    private readonly DragonService dragonService;

    public DragonServiceTest()
    {
        mockUserDataRepository = new Mock<IUserDataRepository>();
        mockUnitRepository = new Mock<IUnitRepository>();
        mockInventoryRepository = new Mock<IInventoryRepository>();
        mockStoryRepository = new Mock<IStoryRepository>();
        mockUpdateDataService = new Mock<IUpdateDataService>();

        dragonService = new DragonService(
            mockUserDataRepository.Object,
            mockUnitRepository.Object,
            mockInventoryRepository.Object,
            mockUpdateDataService.Object,
            mockStoryRepository.Object,
            LoggerTestUtils.Create<DragonService>()
        );
    }

    [Fact]
    public async Task DoDragonGetContactData_ReturnsValidContactData()
    {
        SetupReliabilityMock(
            out List<DbPlayerDragonGift> gifts,
            out DbPlayerCurrency rupies,
            out DbPlayerMaterial garudaEssence,
            out DbPlayerUserData userData,
            out List<DbPlayerDragonReliability> dragonRels,
            out List<DbPlayerStoryState> stories
        );
        DragonGetContactDataData responseData = await dragonService.DoDragonGetContactData(
            new DragonGetContactDataRequest(),
            DeviceAccountId
        );

        responseData.Should().NotBeNull();
        responseData.shop_gift_list.Should().NotBeNullOrEmpty();

        responseData.shop_gift_list.Count().Should().Be(5);
        ((DragonGifts)responseData.shop_gift_list.Last().dragon_gift_id)
            .Should()
            .Be(DragonConstants.rotatingGifts[(int)DateTimeOffset.UtcNow.DayOfWeek]);
    }

    [Fact]
    public async Task DoDragonBuyGiftToSendMultiple_DragonGift_ReturnsCorrectData()
    {
        SetupReliabilityMock(
            out List<DbPlayerDragonGift> gifts,
            out DbPlayerCurrency rupies,
            out DbPlayerMaterial garudaEssence,
            out DbPlayerUserData userData,
            out List<DbPlayerDragonReliability> dragonRels,
            out List<DbPlayerStoryState> stories
        );

        dragonRels.Add(DbPlayerDragonReliabilityFactory.Create(DeviceAccountId, Dragons.Garuda));

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
                },
                DeviceAccountId
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

        mockUserDataRepository.Verify(x => x.LookupUserData());
        mockInventoryRepository.Verify(x => x.GetDragonGifts(DeviceAccountId));
        mockInventoryRepository.Setup(x => x.GetMaterial(DeviceAccountId, It.IsAny<Materials>()));
        mockStoryRepository.VerifyAll();
    }

    [Fact]
    public async Task DoDragonBuyGiftToSendMultiple_DragonGiftNoLevel_ReturnsCorrectData()
    {
        SetupReliabilityMock(
            out List<DbPlayerDragonGift> gifts,
            out DbPlayerCurrency rupies,
            out DbPlayerMaterial garudaEssence,
            out DbPlayerUserData userData,
            out List<DbPlayerDragonReliability> dragonRels,
            out List<DbPlayerStoryState> stories
        );

        DbPlayerDragonReliability dd = DbPlayerDragonReliabilityFactory.Create(
            DeviceAccountId,
            Dragons.Garuda
        );
        dd.Level = 30;
        dd.Exp = 36300;

        dragonRels.Add(dd);

        long startCoin = userData.Coin;
        DragonBuyGiftToSendMultipleData responseData =
            await dragonService.DoDragonBuyGiftToSendMultiple(
                new DragonBuyGiftToSendMultipleRequest()
                {
                    dragon_id = Dragons.Garuda,
                    dragon_gift_id_list = new List<DragonGifts>() { DragonGifts.FreshBread }
                },
                DeviceAccountId
            );

        responseData.Should().NotBeNull();
        responseData.dragon_gift_reward_list.Should().NotBeNullOrEmpty();

        responseData.shop_gift_list.Count().Should().Be(5);

        gifts.Where(x => x.DragonGiftId == DragonGifts.FreshBread).First().Quantity.Should().Be(0);

        responseData.dragon_gift_reward_list.Count().Should().Be(1);
        responseData.dragon_gift_reward_list
            .First(x => x.dragon_gift_id == DragonGifts.FreshBread)
            .reward_reliability_list.Should()
            .BeEmpty();

        dragonRels[0].Exp.Should().Be(36300);
        dragonRels[0].Level.Should().Be(30);

        mockUserDataRepository.Verify(x => x.LookupUserData());
        mockInventoryRepository.Verify(x => x.GetDragonGifts(DeviceAccountId));
        mockInventoryRepository.Setup(x => x.GetMaterial(DeviceAccountId, It.IsAny<Materials>()));
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
            out DbPlayerCurrency rupies,
            out DbPlayerMaterial garudaEssence,
            out DbPlayerUserData userData,
            out List<DbPlayerDragonReliability> dragonRels,
            out List<DbPlayerStoryState> stories
        );

        dragonRels.Add(DbPlayerDragonReliabilityFactory.Create(DeviceAccountId, dragon));

        DragonSendGiftMultipleData responseData = await dragonService.DoDragonSendGiftMultiple(
            new DragonSendGiftMultipleRequest()
            {
                dragon_id = dragon,
                dragon_gift_id = gift,
                quantity = usedQuantity
            },
            DeviceAccountId
        );

        responseData.Should().NotBeNull();

        responseData.return_gift_list.Should().NotBeNullOrEmpty();

        dragonRels[0].Exp.Should().Be(expectedXp);
        dragonRels[0].Level.Should().Be(expectedLvl);
        mockUnitRepository.VerifyAll();
        mockInventoryRepository.Verify(x => x.GetDragonGifts(DeviceAccountId));
        mockInventoryRepository.Setup(x => x.GetMaterial(DeviceAccountId, It.IsAny<Materials>()));
        if (dragon != Dragons.Puppy && dragonRels[0].Level > 4)
            mockStoryRepository.VerifyAll();
    }

    [Fact]
    public async Task DoBuildup_AddsAugments()
    {
        DbPlayerDragonData dragonData = DbPlayerDragonDataFactory.Create(
            DeviceAccountId,
            Dragons.Garuda
        );
        dragonData.DragonKeyId = 1;

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>() { dragonData };

        mockUnitRepository
            .Setup(x => x.GetAllDragonData(DeviceAccountId))
            .Returns(dragonDataList.AsQueryable().BuildMock());

        DbPlayerMaterial mat = new DbPlayerMaterial()
        {
            DeviceAccountId = DeviceAccountId,
            MaterialId = Materials.AmplifyingDragonscale,
            Quantity = 100
        };
        mockInventoryRepository
            .Setup(x => x.GetMaterials(DeviceAccountId))
            .Returns(new List<DbPlayerMaterial>() { mat }.AsQueryable().BuildMock());

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
            },
            DeviceAccountId
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
        DbPlayerDragonData dragonData = DbPlayerDragonDataFactory.Create(DeviceAccountId, dragon);
        dragonData.DragonKeyId = 1;

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>() { dragonData };

        mockUnitRepository
            .Setup(x => x.GetAllDragonData(DeviceAccountId))
            .Returns(dragonDataList.AsQueryable().BuildMock());

        DbPlayerMaterial mat = new DbPlayerMaterial()
        {
            DeviceAccountId = DeviceAccountId,
            MaterialId = upgradeMat,
            Quantity = usedQuantity
        };
        mockInventoryRepository
            .Setup(x => x.GetMaterials(DeviceAccountId))
            .Returns(new List<DbPlayerMaterial>() { mat }.AsQueryable().BuildMock());

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
            },
            DeviceAccountId
        );
        dragonData.Exp.Should().Be(expectedXp);
        dragonData.Level.Should().Be(expectedLvl);
        mat.Quantity.Should().Be(0);

        mockUnitRepository.VerifyAll();
        mockInventoryRepository.VerifyAll();
    }

    [Fact]
    public async Task DoDragonResetPlusCount_ResetsPlusCount()
    {
        DbPlayerUserData userData = new DbPlayerUserData()
        {
            DeviceAccountId = DeviceAccountId,
            Coin = 20000 * 50
        };

        IQueryable<DbPlayerUserData> userDataList = new List<DbPlayerUserData>() { userData }
            .AsQueryable()
            .BuildMock();

        mockUserDataRepository.Setup(x => x.LookupUserData()).ReturnsAsync(userData);

        DbPlayerDragonData dragonData = DbPlayerDragonDataFactory.Create(
            DeviceAccountId,
            Dragons.Garuda
        );
        dragonData.DragonKeyId = 1;
        dragonData.AttackPlusCount = 50;

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>() { dragonData };

        mockUnitRepository
            .Setup(x => x.GetAllDragonData(DeviceAccountId))
            .Returns(dragonDataList.AsQueryable().BuildMock());

        DbPlayerMaterial mat = new DbPlayerMaterial()
        {
            DeviceAccountId = DeviceAccountId,
            MaterialId = Materials.AmplifyingDragonscale,
            Quantity = 0
        };

        mockInventoryRepository
            .Setup(x => x.GetMaterial(DeviceAccountId, Materials.AmplifyingDragonscale))
            .ReturnsAsync(mat);

        await dragonService.DoDragonResetPlusCount(
            new DragonResetPlusCountRequest()
            {
                dragon_key_id = 1,
                plus_count_type = (int)UpgradeEnhanceTypes.AtkPlus
            },
            DeviceAccountId
        );

        userData.Coin.Should().Be(0);

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
            DeviceAccountId,
            Dragons.Garuda
        );
        dragonDataSacrifice.DragonKeyId = 2;
        dragonDataSacrifice.LimitBreakCount = 0;

        DbPlayerDragonData dragonData = DbPlayerDragonDataFactory.Create(
            DeviceAccountId,
            Dragons.Garuda
        );
        dragonData.DragonKeyId = 1;
        dragonData.LimitBreakCount = (byte)(limitBreakNr - 1);

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>()
        {
            dragonData,
            dragonDataSacrifice
        };

        mockUnitRepository
            .Setup(x => x.GetAllDragonData(DeviceAccountId))
            .Returns(dragonDataList.AsQueryable().BuildMock());
        mockUnitRepository
            .Setup(x => x.RemoveDragons(DeviceAccountId, It.IsAny<IEnumerable<long>>()))
            .Callback(() => dragonDataList.RemoveAll(x => x.DragonKeyId == 2));

        DbPlayerMaterial mat = new DbPlayerMaterial()
        {
            DeviceAccountId = DeviceAccountId,
            MaterialId = targetMat,
            Quantity = 500
        };

        mockInventoryRepository
            .Setup(x => x.GetMaterial(DeviceAccountId, targetMat))
            .ReturnsAsync(mat);

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
            },
            DeviceAccountId
        );

        dragonData.LimitBreakCount.Should().Be(limitBreakNr);
        dragonData.Ability1Level.Should().Be(expectedA1Level);
        dragonData.Ability2Level.Should().Be(expectedA2Level);
        dragonData.Skill1Level.Should().Be(expectedS1Level);
        mockUnitRepository.Verify(x => x.GetAllDragonData(DeviceAccountId));
        if (lbMatType == DragonLimitBreakMatTypes.Dupe)
        {
            dragonDataList.Should().NotContain(dragonDataSacrifice);

            mockUnitRepository.Verify(
                x => x.RemoveDragons(DeviceAccountId, It.IsAny<IEnumerable<long>>())
            );
        }
        else
        {
            mat.Quantity.Should().BeLessThan(500);

            mockInventoryRepository.Verify(x => x.GetMaterial(DeviceAccountId, targetMat));
        }
    }

    [Fact]
    public async Task DoDragonLock_Locks()
    {
        DbPlayerDragonData dragonData = DbPlayerDragonDataFactory.Create(
            DeviceAccountId,
            Dragons.Garuda
        );
        dragonData.DragonKeyId = 1;

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>() { dragonData };

        mockUnitRepository
            .Setup(x => x.GetAllDragonData(DeviceAccountId))
            .Returns(dragonDataList.AsQueryable().BuildMock());

        await dragonService.DoDragonSetLock(
            new DragonSetLockRequest() { dragon_key_id = 1, is_lock = true },
            DeviceAccountId
        );

        dragonData.IsLock.Should().BeTrue();

        mockUnitRepository.VerifyAll();
    }

    [Fact]
    public async Task DoDragonSell_AddsCoinAndDew()
    {
        DbPlayerUserData userData = new DbPlayerUserData()
        {
            DeviceAccountId = DeviceAccountId,
            Coin = 0,
            DewPoint = 0
        };

        IQueryable<DbPlayerUserData> userDataList = new List<DbPlayerUserData>() { userData }
            .AsQueryable()
            .BuildMock();

        mockUserDataRepository.Setup(x => x.LookupUserData()).ReturnsAsync(userData);

        DbPlayerDragonData dragonData = DbPlayerDragonDataFactory.Create(
            DeviceAccountId,
            Dragons.Garuda
        );
        dragonData.DragonKeyId = 1;

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>() { dragonData };

        mockUnitRepository
            .Setup(x => x.GetAllDragonData(DeviceAccountId))
            .Returns(dragonDataList.AsQueryable().BuildMock());
        mockUnitRepository
            .Setup(x => x.RemoveDragons(DeviceAccountId, new List<long>() { 1 }))
            .Callback(() => dragonDataList.RemoveAll(x => x.DragonKeyId == 1));

        DragonSellData response = await dragonService.DoDragonSell(
            new DragonSellRequest() { dragon_key_id_list = new List<ulong>() { 1 } },
            DeviceAccountId
        );

        dragonDataList.Count().Should().Be(0);
        userData.Coin.Should().Be(MasterAsset.DragonData.Get(Dragons.Garuda).SellCoin);
        userData.DewPoint.Should().Be(MasterAsset.DragonData.Get(Dragons.Garuda).SellDewPoint);

        mockUnitRepository.VerifyAll();
        mockUserDataRepository.VerifyAll();
    }

    private void SetupReliabilityMock(
        out List<DbPlayerDragonGift> dbPlayerDragonGifts,
        out DbPlayerCurrency userRupies,
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
                    DeviceAccountId = DeviceAccountId,
                    DragonGiftId = gift,
                    Quantity = quantity
                }
            );
        }

        mockInventoryRepository
            .Setup(x => x.GetDragonGifts(DeviceAccountId))
            .Returns(dbPlayerDragonGifts.AsQueryable().BuildMock());

        userRupies = new DbPlayerCurrency()
        {
            DeviceAccountId = DeviceAccountId,
            CurrencyType = CurrencyTypes.Rupies,
            Quantity = 100000
        };

        mockInventoryRepository
            .Setup(x => x.GetCurrency(DeviceAccountId, CurrencyTypes.Rupies))
            .ReturnsAsync(userRupies);

        garudaEssence = new DbPlayerMaterial()
        {
            DeviceAccountId = DeviceAccountId,
            MaterialId = Materials.GarudasEssence,
            Quantity = 0
        };

        mockInventoryRepository
            .Setup(x => x.GetMaterial(DeviceAccountId, It.IsAny<Materials>()))
            .ReturnsAsync(garudaEssence);

        userData = new DbPlayerUserData() { DeviceAccountId = DeviceAccountId, Coin = 100000 };

        IQueryable<DbPlayerUserData> userDataList = new List<DbPlayerUserData>() { userData }
            .AsQueryable()
            .BuildMock();

        mockUserDataRepository.Setup(x => x.GetUserData(DeviceAccountId)).Returns(userDataList);
        mockUserDataRepository.Setup(x => x.LookupUserData()).ReturnsAsync(userData);

        userDragonRels = new List<DbPlayerDragonReliability>();

        mockUnitRepository
            .Setup(x => x.GetAllDragonReliabilityData(DeviceAccountId))
            .Returns(userDragonRels.AsQueryable().BuildMock());

        stories = new List<DbPlayerStoryState>();

        mockStoryRepository
            .Setup(x => x.GetStoryList(DeviceAccountId))
            .Returns(stories.AsQueryable().BuildMock());
        mockStoryRepository
            .Setup(
                x =>
                    x.GetOrCreateStory(
                        DeviceAccountId,
                        StoryTypes.Dragon,
                        MasterAsset.DragonStories.Get((int)Dragons.Garuda).storyIds[0]
                    )
            )
            .ReturnsAsync(
                new DbPlayerStoryState()
                {
                    DeviceAccountId = DeviceAccountId,
                    State = 0,
                    StoryId = MasterAsset.DragonStories.Get((int)Dragons.Garuda).storyIds[0],
                    StoryType = StoryTypes.Dragon
                }
            );
    }
}
