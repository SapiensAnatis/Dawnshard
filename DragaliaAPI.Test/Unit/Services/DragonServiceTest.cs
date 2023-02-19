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
            mockStoryRepository.Object
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
        DragonGifts rotatingGift = DragonConstants.rotatingGifts[
            (int)DateTimeOffset.UtcNow.DayOfWeek
        ];
        long startCoin = userData.Coin;
        DragonBuyGiftToSendMultipleData responseData =
            await dragonService.DoDragonBuyGiftToSendMultiple(
                new DragonBuyGiftToSendMultipleRequest()
                {
                    dragon_id = Dragons.Garuda,
                    dragon_gift_id_list = new List<DragonGifts>()
                    {
                        DragonGifts.StrawberryTart,
                        rotatingGift
                    }
                },
                DeviceAccountId
            );

        responseData.Should().NotBeNull();
        responseData.dragon_gift_reward_list.Should().NotBeNullOrEmpty();

        responseData.shop_gift_list.Count().Should().Be(5);

        AtgenShopGiftList tart = responseData.shop_gift_list.ToList()[2];
        tart.dragon_gift_id.Should().Be((int)DragonGifts.StrawberryTart);
        tart.is_buy.Should().Be(0);

        AtgenShopGiftList rotatingGiftReturn = responseData.shop_gift_list.Last();
        rotatingGiftReturn.dragon_gift_id.Should().Be((int)rotatingGift);
        rotatingGiftReturn.is_buy.Should().Be(0);

        responseData.dragon_gift_reward_list.Count().Should().Be(2);

        int shouldXp =
            DragonConstants.favorVals[DragonGifts.StrawberryTart]
            + (int)(
                DragonConstants.favorVals[rotatingGift]
                * (
                    (
                        MasterAsset.DragonData.Get(Dragons.Garuda).FavoriteType != null
                        && DragonConstants.rotatingGifts[
                            MasterAsset.DragonData.Get(Dragons.Garuda).FavoriteType
                        ] == rotatingGift
                            ? DragonConstants.favMulti
                            : 1
                    )
                )
            );
        dragonRels[0].Exp.Should().Be(shouldXp);
        dragonRels[0].Level
            .Should()
            .Be(
                (byte)(
                    DragonConstants.bondXpLimits.IndexOf(
                        DragonConstants.bondXpLimits.Last(x => !(x > dragonRels[0].Exp))
                    ) + 1
                )
            );

        userData.Coin
            .Should()
            .Be(
                startCoin
                    - DragonConstants.buyGiftPrices[DragonGifts.StrawberryTart]
                    - DragonConstants.buyGiftPrices[rotatingGift]
            );
    }

    [Fact]
    public async Task DoDragonSendGiftMultiple_Puppy()
    {
        SetupReliabilityMock(
            out List<DbPlayerDragonGift> gifts,
            out DbPlayerCurrency rupies,
            out DbPlayerMaterial garudaEssence,
            out DbPlayerUserData userData,
            out List<DbPlayerDragonReliability> dragonRels,
            out List<DbPlayerStoryState> stories
        );

        dragonRels.Add(DbPlayerDragonReliabilityFactory.Create(DeviceAccountId, Dragons.Puppy));

        DragonSendGiftMultipleData responseData = await dragonService.DoDragonSendGiftMultiple(
            new DragonSendGiftMultipleRequest()
            {
                dragon_id = Dragons.Puppy,
                dragon_gift_id = DragonGifts.PupGrub,
                quantity = 10
            },
            DeviceAccountId
        );

        responseData.Should().NotBeNull();

        responseData.return_gift_list.Count().Should().Be(1);

        int shouldXp = DragonConstants.favorVals[DragonGifts.PupGrub] * 10;
        dragonRels[1].Exp.Should().Be(shouldXp);
        dragonRels[1].Level
            .Should()
            .Be(
                (byte)(
                    DragonConstants.bondXpLimitsPuppy.IndexOf(
                        DragonConstants.bondXpLimitsPuppy.Last(x => !(x > dragonRels[1].Exp))
                    ) + 1
                )
            );
    }

    [Fact]
    public async Task DoBuildup_BuildsUp()
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
            MaterialId = Materials.SucculentDragonfruit,
            Quantity = 100
        };

        mockInventoryRepository
            .Setup(x => x.GetMaterial(DeviceAccountId, Materials.SucculentDragonfruit))
            .ReturnsAsync(mat);
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
                        id = (int)Materials.SucculentDragonfruit,
                        quantity = 10
                    }
                }
            },
            DeviceAccountId
        );

        int shouldXp = UpgradeMaterials.buildupXpValues[Materials.SucculentDragonfruit] * 10;
        dragonData.Exp.Should().Be(shouldXp);
        dragonData.Level
            .Should()
            .Be(
                (byte)(
                    DragonConstants.XpLimits.IndexOf(
                        DragonConstants.XpLimits.Last(x => !(x > shouldXp))
                    ) + 1
                )
            );
        mat.Quantity.Should().Be(90);
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

        mockUserDataRepository.Setup(x => x.GetUserData(ViewerId)).Returns(userDataList);

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
            DeviceAccountId,
            ViewerId
        );

        userData.Coin.Should().Be(0);
    }

    [Fact]
    public async Task DoDragonLimitBreak_LimitBreaks()
    {
        DbPlayerDragonData dragonData = DbPlayerDragonDataFactory.Create(
            DeviceAccountId,
            Dragons.Garuda
        );
        dragonData.DragonKeyId = 1;
        dragonData.LimitBreakCount = 1;

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>() { dragonData };

        mockUnitRepository
            .Setup(x => x.GetAllDragonData(DeviceAccountId))
            .Returns(dragonDataList.AsQueryable().BuildMock());

        DbPlayerMaterial mat = new DbPlayerMaterial()
        {
            DeviceAccountId = DeviceAccountId,
            MaterialId = Materials.SunlightStone,
            Quantity = 1
        };

        mockInventoryRepository
            .Setup(x => x.GetMaterial(DeviceAccountId, Materials.SunlightStone))
            .ReturnsAsync(mat);

        await dragonService.DoDragonLimitBreak(
            new DragonLimitBreakRequest()
            {
                base_dragon_key_id = 1,
                limit_break_grow_list = new List<LimitBreakGrowList>()
                {
                    new LimitBreakGrowList()
                    {
                        limit_break_item_type = (int)DragonLimitBreakMatTypes.Stone,
                        limit_break_count = 2,
                        target_id = 0
                    }
                }
            },
            DeviceAccountId
        );

        dragonData.LimitBreakCount.Should().Be(2);
        mat.Quantity.Should().Be(0);
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

        mockUserDataRepository.Setup(x => x.GetUserData(ViewerId)).Returns(userDataList);

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
            DeviceAccountId,
            ViewerId
        );

        dragonDataList.Count().Should().Be(0);
        userData.Coin.Should().Be(MasterAsset.DragonData.Get(Dragons.Garuda).SellCoin);
        userData.DewPoint.Should().Be(MasterAsset.DragonData.Get(Dragons.Garuda).SellDewPoint);
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

        userDragonRels = new List<DbPlayerDragonReliability>()
        {
            DbPlayerDragonReliabilityFactory.Create(DeviceAccountId, Dragons.Garuda)
        };

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

    private void SetupUpgradeMock() { }
}
