using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Extensions;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Shared;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Features.Story;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Test.Utils;
using Microsoft.Extensions.Time.Testing;
using MockQueryable;
using DragonService = DragaliaAPI.Features.Dragons.DragonService;

namespace DragaliaAPI.Test.Features.Dragons;

public class DragonServiceTest : RepositoryTestFixture
{
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IUnitRepository> mockUnitRepository;
    private readonly Mock<IInventoryRepository> mockInventoryRepository;
    private readonly Mock<IStoryRepository> mockStoryRepository;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IPaymentService> mockPaymentService;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly Mock<IMissionProgressionService> mockMissionProgressionService;
    private readonly FakeTimeProvider mockTimeProvider;

    private readonly DragonService dragonService;

    public DragonServiceTest()
    {
        this.mockUserDataRepository = new Mock<IUserDataRepository>();
        this.mockUnitRepository = new Mock<IUnitRepository>();
        this.mockInventoryRepository = new Mock<IInventoryRepository>();
        this.mockStoryRepository = new Mock<IStoryRepository>();
        this.mockUpdateDataService = new Mock<IUpdateDataService>();
        this.mockPaymentService = new(MockBehavior.Strict);
        this.mockRewardService = new(MockBehavior.Strict);
        this.mockMissionProgressionService = new(MockBehavior.Strict);
        this.mockTimeProvider = new FakeTimeProvider();

        this.dragonService = new DragonService(
            this.mockUserDataRepository.Object,
            this.mockUnitRepository.Object,
            this.mockInventoryRepository.Object,
            this.mockUpdateDataService.Object,
            this.mockStoryRepository.Object,
            LoggerTestUtils.Create<DragonService>(),
            this.mockPaymentService.Object,
            this.mockRewardService.Object,
            this.mockMissionProgressionService.Object,
            this.mockTimeProvider,
            this.ApiContext
        );

        this.mockTimeProvider.SetUtcNow(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task DoDragonGetContactData_ReturnsValidContactData()
    {
        DateTimeOffset lastReset = this.mockTimeProvider.GetLastDailyReset();

        this.SetupReliabilityMock(
            out List<DbPlayerDragonGift> gifts,
            out DbPlayerMaterial garudaEssence,
            out DbPlayerUserData userData,
            out List<DbPlayerDragonReliability> dragonRels,
            out List<DbPlayerStoryState> stories
        );
        DragonGetContactDataResponse responseData =
            await this.dragonService.DoDragonGetContactData();

        responseData.Should().NotBeNull();
        responseData.ShopGiftList.Should().NotBeNullOrEmpty();

        responseData.ShopGiftList.Count().Should().Be(5);
        ((DragonGifts)responseData.ShopGiftList.Last().DragonGiftId)
            .Should()
            .Be(DragonConstants.RotatingGifts[(int)lastReset.DayOfWeek]);
    }

    [Fact]
    public async Task DoGetDragonContactData_RotatingGiftChangesAtReset()
    {
        await this.AddRangeToDatabase(
            new List<DbPlayerDragonGift>()
            {
                new()
                {
                    ViewerId = 1,
                    DragonGiftId = DragonGifts.FloralCirclet,
                    Quantity = 1,
                },
                new()
                {
                    ViewerId = 1,
                    DragonGiftId = DragonGifts.CompellingBook,
                    Quantity = 1,
                },
            }
        );

        DateTimeOffset wednesday = new DateTimeOffset(2030, 12, 25, 19, 49, 23, TimeSpan.Zero);
        this.mockTimeProvider.SetUtcNow(wednesday);

        (await this.dragonService.DoDragonGetContactData())
            .ShopGiftList.Should()
            .Contain(x => x.DragonGiftId == (int)DragonGifts.FloralCirclet);

        DateTimeOffset thuBeforeReset = new DateTimeOffset(2030, 12, 26, 01, 49, 23, TimeSpan.Zero);
        this.mockTimeProvider.SetUtcNow(thuBeforeReset);

        (await this.dragonService.DoDragonGetContactData())
            .ShopGiftList.Should()
            .Contain(x => x.DragonGiftId == (int)DragonGifts.FloralCirclet);

        DateTimeOffset thursday = new DateTimeOffset(2030, 12, 26, 09, 49, 23, TimeSpan.Zero);
        this.mockTimeProvider.SetUtcNow(thursday);

        (await this.dragonService.DoDragonGetContactData())
            .ShopGiftList.Should()
            .Contain(x => x.DragonGiftId == (int)DragonGifts.CompellingBook);
    }

    [Fact]
    public async Task DoDragonBuyGiftToSendMultiple_DragonGift_ReturnsCorrectData()
    {
        this.SetupReliabilityMock(
            out List<DbPlayerDragonGift> gifts,
            out DbPlayerMaterial garudaEssence,
            out DbPlayerUserData userData,
            out List<DbPlayerDragonReliability> dragonRels,
            out List<DbPlayerStoryState> stories
        );

        dragonRels.Add(new DbPlayerDragonReliability(ViewerId, DragonId.Garuda));

        this.mockMissionProgressionService.Setup(x =>
            x.OnDragonBondLevelUp(DragonId.Garuda, UnitElement.Wind, 3, 4)
        );
        this.mockMissionProgressionService.Setup(x =>
            x.OnDragonBondLevelUp(DragonId.Garuda, UnitElement.Wind, 6, 10)
        );

        this.mockMissionProgressionService.Setup(x =>
            x.OnDragonGiftSent(
                DragonId.Garuda,
                It.IsIn(DragonGifts.StrawberryTart, DragonGifts.CompellingBook),
                UnitElement.Wind,
                1,
                0
            )
        );

        long startCoin = userData.Coin;
        DragonBuyGiftToSendMultipleResponse responseData =
            await this.dragonService.DoDragonBuyGiftToSendMultiple(
                new DragonBuyGiftToSendMultipleRequest()
                {
                    DragonId = DragonId.Garuda,
                    DragonGiftIdList = new List<DragonGifts>()
                    {
                        DragonGifts.StrawberryTart,
                        DragonGifts.CompellingBook,
                    },
                },
                TestContext.Current.CancellationToken
            );

        responseData.Should().NotBeNull();
        responseData.DragonGiftRewardList.Should().NotBeNullOrEmpty();

        responseData.ShopGiftList.Count().Should().Be(5);

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

        responseData.DragonGiftRewardList.Count().Should().Be(2);

        dragonRels[0].Exp.Should().Be(2400);
        dragonRels[0].Level.Should().Be(10);

        this.mockUserDataRepository.Verify(x => x.UserData);
        this.mockInventoryRepository.Setup(x => x.GetMaterial(It.IsAny<Materials>()));
        this.mockMissionProgressionService.VerifyAll();
        this.mockStoryRepository.VerifyAll();
    }

    [Fact]
    public async Task DoDragonBuyGiftToSendMultiple_DragonGiftNoLevel_ReturnsCorrectData()
    {
        this.SetupReliabilityMock(
            out List<DbPlayerDragonGift> gifts,
            out DbPlayerMaterial garudaEssence,
            out DbPlayerUserData userData,
            out List<DbPlayerDragonReliability> dragonRels,
            out List<DbPlayerStoryState> stories
        );

        DbPlayerDragonReliability dd = new DbPlayerDragonReliability(ViewerId, DragonId.Garuda);
        dd.Level = 30;
        dd.Exp = 36300;

        dragonRels.Add(dd);

        this.mockMissionProgressionService.Setup(x =>
            x.OnDragonGiftSent(DragonId.Garuda, DragonGifts.FreshBread, UnitElement.Wind, 1, 0)
        );

        long startCoin = userData.Coin;
        DragonBuyGiftToSendMultipleResponse responseData =
            await this.dragonService.DoDragonBuyGiftToSendMultiple(
                new DragonBuyGiftToSendMultipleRequest()
                {
                    DragonId = DragonId.Garuda,
                    DragonGiftIdList = new List<DragonGifts>() { DragonGifts.FreshBread },
                },
                TestContext.Current.CancellationToken
            );

        responseData.Should().NotBeNull();
        responseData.DragonGiftRewardList.Should().NotBeNullOrEmpty();

        responseData.ShopGiftList.Count().Should().Be(5);

        gifts.Where(x => x.DragonGiftId == DragonGifts.FreshBread).First().Quantity.Should().Be(0);

        responseData.DragonGiftRewardList.Count().Should().Be(1);
        responseData
            .DragonGiftRewardList.First(x => x.DragonGiftId == DragonGifts.FreshBread)
            .RewardReliabilityList.Should()
            .BeEmpty();

        dragonRels[0].Exp.Should().Be(36300);
        dragonRels[0].Level.Should().Be(30);

        this.mockUserDataRepository.Verify(x => x.UserData);
        this.mockInventoryRepository.Setup(x => x.GetMaterial(It.IsAny<Materials>()));
        this.mockMissionProgressionService.VerifyAll();
    }

    [Theory]
    [InlineData(DragonId.Garuda, DragonGifts.FourLeafClover, 10, 10000, 18)]
    [InlineData(DragonId.Puppy, DragonGifts.PupGrub, 10, 2000, 21)]
    public async Task DoDragonSendGiftMultiple_ReturnsExpecedValues(
        DragonId dragon,
        DragonGifts gift,
        int usedQuantity,
        int expectedXp,
        byte expectedLvl
    )
    {
        this.SetupReliabilityMock(
            out List<DbPlayerDragonGift> gifts,
            out DbPlayerMaterial garudaEssence,
            out DbPlayerUserData userData,
            out List<DbPlayerDragonReliability> dragonRels,
            out List<DbPlayerStoryState> stories
        );

        dragonRels.Add(new DbPlayerDragonReliability(ViewerId, dragon));

        UnitElement element = MasterAsset.DragonData[dragon].ElementalType;

        this.mockMissionProgressionService.Setup(x =>
            x.OnDragonBondLevelUp(dragon, element, expectedLvl - 1, expectedLvl)
        );

        this.mockMissionProgressionService.Setup(x =>
            x.OnDragonGiftSent(dragon, gift, element, usedQuantity, 0)
        );

        DragonSendGiftMultipleResponse responseData =
            await this.dragonService.DoDragonSendGiftMultiple(
                new DragonSendGiftMultipleRequest()
                {
                    DragonId = dragon,
                    DragonGiftId = gift,
                    Quantity = usedQuantity,
                },
                TestContext.Current.CancellationToken
            );

        responseData.Should().NotBeNull();

        responseData.ReturnGiftList.Should().NotBeNullOrEmpty();

        dragonRels[0].Exp.Should().Be(expectedXp);
        dragonRels[0].Level.Should().Be(expectedLvl);
        this.mockUnitRepository.VerifyAll();
        this.mockInventoryRepository.Setup(x => x.GetMaterial(It.IsAny<Materials>()));
        this.mockMissionProgressionService.VerifyAll();
        if (dragon != DragonId.Puppy && dragonRels[0].Level > 4)
            this.mockStoryRepository.VerifyAll();
    }

    [Fact]
    public async Task DoBuildup_AddsAugments()
    {
        DbPlayerDragonData dragonData = new DbPlayerDragonData(ViewerId, DragonId.Garuda);
        dragonData.DragonKeyId = 1;

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>() { dragonData };

        this.mockUnitRepository.SetupGet(x => x.Dragons)
            .Returns(dragonDataList.AsQueryable().BuildMock());

        DbPlayerMaterial mat = new DbPlayerMaterial()
        {
            ViewerId = ViewerId,
            MaterialId = Materials.AmplifyingDragonscale,
            Quantity = 100,
        };
        this.mockInventoryRepository.SetupGet(x => x.Materials)
            .Returns(
                new List<DbPlayerMaterial>() { mat }
                    .AsQueryable()
                    .BuildMock()
            );

        await this.dragonService.DoBuildup(
            new DragonBuildupRequest()
            {
                BaseDragonKeyId = 1,
                GrowMaterialList = new List<GrowMaterialList>()
                {
                    new GrowMaterialList()
                    {
                        Type = EntityTypes.Material,
                        Id = (int)Materials.AmplifyingDragonscale,
                        Quantity = 50,
                    },
                },
            },
            TestContext.Current.CancellationToken
        );

        dragonData.AttackPlusCount.Should().Be(50);
        mat.Quantity.Should().Be(50);

        this.mockUnitRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
    }

    [Theory]
    [InlineData(DragonId.Garuda, Materials.SucculentDragonfruit, 10, 35000, 26)]
    [InlineData(DragonId.Fubuki, Materials.Dragonfruit, 30, 4500, 10)]
    public async Task DoBuildup_BuildsUp(
        DragonId dragon,
        Materials upgradeMat,
        int usedQuantity,
        int expectedXp,
        byte expectedLvl
    )
    {
        DbPlayerDragonData dragonData = new DbPlayerDragonData(ViewerId, dragon);
        dragonData.DragonKeyId = 1;

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>() { dragonData };

        this.mockUnitRepository.SetupGet(x => x.Dragons)
            .Returns(dragonDataList.AsQueryable().BuildMock());

        UnitElement element = MasterAsset.DragonData[dragon].ElementalType;

        this.mockMissionProgressionService.Setup(x =>
            x.OnDragonLevelUp(dragon, element, expectedLvl - 1, expectedLvl)
        );

        DbPlayerMaterial mat = new DbPlayerMaterial()
        {
            ViewerId = ViewerId,
            MaterialId = upgradeMat,
            Quantity = usedQuantity,
        };
        this.mockInventoryRepository.SetupGet(x => x.Materials)
            .Returns(
                new List<DbPlayerMaterial>() { mat }
                    .AsQueryable()
                    .BuildMock()
            );

        await this.dragonService.DoBuildup(
            new DragonBuildupRequest()
            {
                BaseDragonKeyId = 1,
                GrowMaterialList = new List<GrowMaterialList>()
                {
                    new GrowMaterialList()
                    {
                        Type = EntityTypes.Material,
                        Id = (int)upgradeMat,
                        Quantity = usedQuantity,
                    },
                },
            },
            TestContext.Current.CancellationToken
        );
        dragonData.Exp.Should().Be(expectedXp);
        dragonData.Level.Should().Be(expectedLvl);
        mat.Quantity.Should().Be(0);

        this.mockUnitRepository.VerifyAll();
        this.mockMissionProgressionService.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
    }

    [Fact]
    public async Task DoDragonResetPlusCount_ResetsPlusCount()
    {
        DbPlayerDragonData dragonData = new DbPlayerDragonData(ViewerId, DragonId.Garuda);
        dragonData.DragonKeyId = 1;
        dragonData.AttackPlusCount = 50;

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>() { dragonData };

        this.mockUnitRepository.SetupGet(x => x.Dragons)
            .Returns(dragonDataList.AsQueryable().BuildMock());

        DbPlayerMaterial mat = new DbPlayerMaterial()
        {
            ViewerId = ViewerId,
            MaterialId = Materials.AmplifyingDragonscale,
            Quantity = 0,
        };

        this.mockPaymentService.Setup(x =>
                x.ProcessPayment(
                    PaymentTypes.Coin,
                    null,
                    DragonConstants.AugmentResetCost * dragonData.AttackPlusCount
                )
            )
            .Returns(Task.CompletedTask);

        this.mockRewardService.Setup(x =>
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

        this.mockRewardService.Setup(x => x.GetEntityResult()).Returns(new EntityResult());

        await this.dragonService.DoDragonResetPlusCount(
            new DragonResetPlusCountRequest()
            {
                DragonKeyId = 1,
                PlusCountType = PlusCountType.Atk,
            },
            TestContext.Current.CancellationToken
        );

        this.mockRewardService.VerifyAll();
        this.mockPaymentService.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockUnitRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
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

        DbPlayerDragonData dragonDataSacrifice = new DbPlayerDragonData(ViewerId, DragonId.Garuda);
        dragonDataSacrifice.DragonKeyId = 2;
        dragonDataSacrifice.LimitBreakCount = 0;

        DbPlayerDragonData dragonData = new DbPlayerDragonData(ViewerId, DragonId.Garuda);
        dragonData.DragonKeyId = 1;
        dragonData.LimitBreakCount = (byte)(limitBreakNr - 1);

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>()
        {
            dragonData,
            dragonDataSacrifice,
        };

        this.mockUnitRepository.SetupGet(x => x.Dragons)
            .Returns(dragonDataList.AsQueryable().BuildMock());
        this.mockUnitRepository.Setup(x => x.RemoveDragons(It.IsAny<IEnumerable<long>>()))
            .Callback(() => dragonDataList.RemoveAll(x => x.DragonKeyId == 2));

        DbPlayerMaterial mat = new DbPlayerMaterial()
        {
            ViewerId = ViewerId,
            MaterialId = targetMat,
            Quantity = 500,
        };

        this.mockInventoryRepository.Setup(x => x.GetMaterial(targetMat)).ReturnsAsync(mat);

        await this.dragonService.DoDragonLimitBreak(
            new DragonLimitBreakRequest()
            {
                BaseDragonKeyId = 1,
                LimitBreakGrowList = new List<LimitBreakGrowList>()
                {
                    new LimitBreakGrowList()
                    {
                        LimitBreakItemType = (int)lbMatType,
                        LimitBreakCount = limitBreakNr,
                        TargetId = (ulong)(lbMatType == DragonLimitBreakMatTypes.Dupe ? 2 : 0),
                    },
                },
            },
            TestContext.Current.CancellationToken
        );

        dragonData.LimitBreakCount.Should().Be(limitBreakNr);
        dragonData.Ability1Level.Should().Be(expectedA1Level);
        dragonData.Ability2Level.Should().Be(expectedA2Level);
        dragonData.Skill1Level.Should().Be(expectedS1Level);
        this.mockUnitRepository.Verify(x => x.Dragons);
        if (lbMatType == DragonLimitBreakMatTypes.Dupe)
        {
            dragonDataList.Should().NotContain(dragonDataSacrifice);

            this.mockUnitRepository.Verify(x => x.RemoveDragons(It.IsAny<IEnumerable<long>>()));
        }
        else
        {
            mat.Quantity.Should().BeLessThan(500);

            this.mockInventoryRepository.Verify(x => x.GetMaterial(targetMat));
        }
    }

    [Fact]
    public async Task DoDragonLock_Locks()
    {
        DbPlayerDragonData dragonData = new DbPlayerDragonData(ViewerId, DragonId.Garuda);
        dragonData.DragonKeyId = 1;

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>() { dragonData };

        this.mockUnitRepository.SetupGet(x => x.Dragons)
            .Returns(dragonDataList.AsQueryable().BuildMock());

        await this.dragonService.DoDragonSetLock(
            new DragonSetLockRequest() { DragonKeyId = 1, IsLock = true },
            TestContext.Current.CancellationToken
        );

        dragonData.IsLock.Should().BeTrue();

        this.mockUnitRepository.VerifyAll();
    }

    [Fact]
    public async Task DoDragonSell_AddsCoinAndDew()
    {
        DbPlayerUserData userData = new DbPlayerUserData()
        {
            ViewerId = ViewerId,
            Coin = 0,
            DewPoint = 0,
        };

        IQueryable<DbPlayerUserData> userDataList = new List<DbPlayerUserData>() { userData }
            .AsQueryable()
            .BuildMock();

        this.mockUserDataRepository.SetupGet(x => x.UserData).Returns(userDataList);

        DbPlayerDragonData dragonData = new DbPlayerDragonData(ViewerId, DragonId.Garuda);
        dragonData.DragonKeyId = 1;

        List<DbPlayerDragonData> dragonDataList = new List<DbPlayerDragonData>() { dragonData };

        this.mockUnitRepository.SetupGet(x => x.Dragons)
            .Returns(dragonDataList.AsQueryable().BuildMock());
        this.mockUnitRepository.Setup(x => x.RemoveDragons(new List<long>() { 1 }))
            .Callback(() => dragonDataList.RemoveAll(x => x.DragonKeyId == 1));

        DragonSellResponse response = await this.dragonService.DoDragonSell(
            new DragonSellRequest() { DragonKeyIdList = new List<ulong>() { 1 } },
            TestContext.Current.CancellationToken
        );

        dragonDataList.Count.Should().Be(0);
        userData.Coin.Should().Be(MasterAsset.DragonData.Get(DragonId.Garuda).SellCoin);
        userData.DewPoint.Should().Be(MasterAsset.DragonData.Get(DragonId.Garuda).SellDewPoint);

        this.mockUnitRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
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
                    Quantity = quantity,
                }
            );
        }

        this.AddRangeToDatabase(dbPlayerDragonGifts).Wait();

        garudaEssence = new DbPlayerMaterial()
        {
            ViewerId = ViewerId,
            MaterialId = Materials.GarudasEssence,
            Quantity = 0,
        };

        this.mockInventoryRepository.Setup(x => x.GetMaterial(It.IsAny<Materials>()))
            .ReturnsAsync(garudaEssence);

        userData = new DbPlayerUserData() { ViewerId = ViewerId, Coin = 100000 };

        IQueryable<DbPlayerUserData> userDataList = new List<DbPlayerUserData>() { userData }
            .AsQueryable()
            .BuildMock();

        this.mockUserDataRepository.SetupGet(x => x.UserData).Returns(userDataList);

        userDragonRels = new List<DbPlayerDragonReliability>();

        this.mockUnitRepository.SetupGet(x => x.DragonReliabilities)
            .Returns(userDragonRels.AsQueryable().BuildMock());

        stories = new List<DbPlayerStoryState>();

        this.mockStoryRepository.Setup(x =>
                x.GetOrCreateStory(
                    StoryTypes.Dragon,
                    MasterAsset.DragonStories.Get((int)DragonId.Garuda).StoryIds[0]
                )
            )
            .ReturnsAsync(
                new DbPlayerStoryState()
                {
                    ViewerId = ViewerId,
                    State = 0,
                    StoryId = MasterAsset.DragonStories.Get((int)DragonId.Garuda).StoryIds[0],
                    StoryType = StoryTypes.Dragon,
                }
            );
    }
}
