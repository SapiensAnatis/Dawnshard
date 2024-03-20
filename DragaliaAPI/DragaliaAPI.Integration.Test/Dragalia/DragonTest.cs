using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.DragonController"/>
/// </summary>
public class DragonTest : TestFixture
{
    public DragonTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.MockTimeProvider.SetUtcNow(DateTimeOffset.UtcNow);
    }

    public record DragonBuildUpTestCase(
        List<DbPlayerDragonData> SetupDragons,
        EntityTypes MatType,
        int UpgradeMat,
        int UsedQuantity,
        int ExpectedXp,
        int ExpectedLvl
    );

    public class DragonBuildUpTheoryData : TheoryData<DragonBuildUpTestCase>
    {
        public DragonBuildUpTheoryData()
        {
            #region GarudaDragonSacrifice
            List<DbPlayerDragonData> setupDragons = new List<DbPlayerDragonData>
            {
                DbPlayerDragonDataFactory.Create(0, Dragons.Garuda)
            };
            DbPlayerDragonData dragon = DbPlayerDragonDataFactory.Create(0, Dragons.Garuda);
            dragon.AttackPlusCount = 25;
            dragon.HpPlusCount = 25;
            setupDragons.Add(dragon);
            Add(new DragonBuildUpTestCase(setupDragons, EntityTypes.Dragon, -1, 1, 1500, 5));
            #endregion
            #region FubukiFruit
            Add(
                new DragonBuildUpTestCase(
                    new List<DbPlayerDragonData>()
                    {
                        DbPlayerDragonDataFactory.Create(0, Dragons.Fubuki)
                    },
                    EntityTypes.Material,
                    (int)Materials.Dragonfruit,
                    30,
                    4500,
                    10
                )
            );
            #endregion
        }
    }

    [Theory]
    [ClassData(typeof(DragonBuildUpTheoryData))]
    public async Task DragonBuildUp_ReturnsUpgradedDragonData(DragonBuildUpTestCase testCase)
    {
        ApiContext context = this.Services.GetRequiredService<ApiContext>();

        DbPlayerDragonData dbDragon = await this.AddToDatabase(testCase.SetupDragons[0]);

        DbPlayerDragonData dbDragonSacrifice = null!;
        if (testCase.MatType == EntityTypes.Dragon)
        {
            dbDragonSacrifice = await this.AddToDatabase(testCase.SetupDragons[1]);
        }

        await context.SaveChangesAsync();

        DragonBuildupRequest request = new DragonBuildupRequest()
        {
            BaseDragonKeyId = (ulong)dbDragon.DragonKeyId,
            GrowMaterialList = new List<GrowMaterialList>()
            {
                new GrowMaterialList()
                {
                    Type = testCase.MatType,
                    Id =
                        testCase.MatType == EntityTypes.Dragon
                            ? (int)dbDragonSacrifice.DragonKeyId
                            : testCase.UpgradeMat,
                    Quantity = testCase.UsedQuantity
                }
            }
        };

        DragonBuildupResponse response = (
            await this.Client.PostMsgpack<DragonBuildupResponse>("dragon/buildup", request)
        ).Data;

        response.Should().NotBeNull();
        response.UpdateDataList.Should().NotBeNull();
        response.UpdateDataList.DragonList.Should().NotBeNullOrEmpty();
        DragonList returnDragon = response
            .UpdateDataList.DragonList!.Where(x => (long)x.DragonKeyId == dbDragon.DragonKeyId)
            .First();
        returnDragon.Exp.Should().Be(testCase.ExpectedXp);
        returnDragon.Level.Should().Be(testCase.ExpectedLvl);
        if (testCase.MatType == EntityTypes.Dragon)
        {
            returnDragon.AttackPlusCount.Should().Be(dbDragonSacrifice.AttackPlusCount);
            returnDragon.HpPlusCount.Should().Be(dbDragonSacrifice.HpPlusCount);
        }
    }

    [Fact]
    public async Task DragonBuildUp_ReturnsDragonDataWithAugments()
    {
        ApiContext context = this.Services.GetRequiredService<ApiContext>();

        DbPlayerDragonData dbDragon = context
            .PlayerDragonData.Add(DbPlayerDragonDataFactory.Create(ViewerId, Dragons.Liger))
            .Entity;

        await context.SaveChangesAsync();

        DragonBuildupRequest request = new DragonBuildupRequest()
        {
            BaseDragonKeyId = (ulong)dbDragon.DragonKeyId,
            GrowMaterialList = new List<GrowMaterialList>()
            {
                new GrowMaterialList()
                {
                    Type = EntityTypes.Material,
                    Id = (int)Materials.AmplifyingDragonscale,
                    Quantity = 25
                }
            }
        };

        DragonBuildupResponse? response = (
            await this.Client.PostMsgpack<DragonBuildupResponse>("dragon/buildup", request)
        ).Data;

        response.Should().NotBeNull();
        response.UpdateDataList.Should().NotBeNull();
        response.UpdateDataList.DragonList.Should().NotBeNullOrEmpty();
        DragonList returnDragon = response
            .UpdateDataList.DragonList!.Where(x => (long)x.DragonKeyId == dbDragon.DragonKeyId)
            .First();
        returnDragon.AttackPlusCount.Should().Be(25);
    }

    [Fact]
    public async Task DragonResetPlusCount_ResetsAugments()
    {
        ApiContext context = this.Services.GetRequiredService<ApiContext>();

        DbPlayerDragonData dragon = DbPlayerDragonDataFactory.Create(ViewerId, Dragons.Maritimus);
        dragon.AttackPlusCount = 50;
        dragon = context.PlayerDragonData.Add(dragon).Entity;

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();
        DbPlayerUserData userData = await context
            .PlayerUserData.Where(x => x.ViewerId == ViewerId)
            .FirstAsync();

        long startCoin = userData.Coin;

        int augmentCount = (
            await context.PlayerMaterials.FindAsync(ViewerId, Materials.AmplifyingDragonscale)
        )!.Quantity;

        DragonResetPlusCountRequest request = new DragonResetPlusCountRequest()
        {
            DragonKeyId = (ulong)dragon.DragonKeyId,
            PlusCountType = PlusCountType.Atk
        };

        DragonSetLockResponse? response = (
            await this.Client.PostMsgpack<DragonSetLockResponse>("dragon/reset_plus_count", request)
        ).Data;

        response.Should().NotBeNull();
        response.UpdateDataList.Should().NotBeNull();
        response.UpdateDataList.DragonList.Should().NotBeNullOrEmpty();
        DragonList returnDragon = response
            .UpdateDataList.DragonList!.Where(x => (long)x.DragonKeyId == dragon.DragonKeyId)
            .First();
        returnDragon.AttackPlusCount.Should().Be(0);
        response.UpdateDataList.UserData.Should().NotBeNull();
        response.UpdateDataList.UserData.Coin.Should().Be(startCoin - (20000 * 50));
        response
            .UpdateDataList.MaterialList!.Where(x =>
                x.MaterialId == Materials.AmplifyingDragonscale
            )
            .First()
            .Quantity.Should()
            .Be(augmentCount + 50);
    }

    [Fact]
    public async Task DragonGetContactData_ReturnsValidData()
    {
        DragonGetContactDataResponse? response = (
            await this.Client.PostMsgpack<DragonGetContactDataResponse>("dragon/get_contact_data")
        ).Data;

        response.Should().NotBeNull();
        response.ShopGiftList.Should().NotBeNullOrEmpty();
        response.ShopGiftList.Count().Should().Be(5);
    }

    [Fact]
    public async Task DragonBuyGiftToSendMultiple_IncreasesReliabilityAndReturnsGifts()
    {
        this.ApiContext.PlayerDragonReliability.Add(
            DbPlayerDragonReliabilityFactory.Create(ViewerId, Dragons.HighChthonius)
        );

        await this.ApiContext.SaveChangesAsync();

        this.ApiContext.ChangeTracker.Clear();
        DbPlayerUserData userData = await this
            .ApiContext.PlayerUserData.Where(x => x.ViewerId == ViewerId)
            .FirstAsync();

        long startCoin = userData.Coin;

        DragonBuyGiftToSendMultipleRequest request = new DragonBuyGiftToSendMultipleRequest()
        {
            DragonId = Dragons.HighChthonius,
            DragonGiftIdList = new List<DragonGifts>()
            {
                DragonGifts.FreshBread,
                DragonGifts.TastyMilk
            }
        };

        DragonBuyGiftToSendMultipleResponse? response = (
            await this.Client.PostMsgpack<DragonBuyGiftToSendMultipleResponse>(
                "dragon/buy_gift_to_send_multiple",
                request
            )
        ).Data;

        response.Should().NotBeNull();
        response
            .ShopGiftList.Where(x => (DragonGifts)x.DragonGiftId == DragonGifts.FreshBread)
            .First()
            .IsBuy.Should()
            .Be(false);
        response
            .ShopGiftList.Where(x => (DragonGifts)x.DragonGiftId == DragonGifts.TastyMilk)
            .First()
            .IsBuy.Should()
            .Be(false);

        response.DragonGiftRewardList.Should().NotBeNullOrEmpty();
        response.UpdateDataList.UserData.Coin.Should().Be(startCoin - 1500);
        DragonReliabilityList dragonData = response
            .UpdateDataList.DragonReliabilityList!.Where(x => x.DragonId == Dragons.HighChthonius)
            .First();
        dragonData.ReliabilityTotalExp.Should().Be(400);
        dragonData.ReliabilityLevel.Should().Be(3);
    }

    [Fact]
    public async Task DragonBuyGiftToSend_IncreasesReliabilityAndReturnsGifts()
    {
        await this.AddToDatabase(
            new DbPlayerDragonReliability() { DragonId = Dragons.HighJupiter, Level = 1 }
        );

        DragonBuyGiftToSendRequest request = new DragonBuyGiftToSendRequest()
        {
            DragonId = Dragons.HighJupiter,
            DragonGiftId = DragonGifts.HeartyStew
        };

        DragonBuyGiftToSendResponse? response = (
            await this.Client.PostMsgpack<DragonBuyGiftToSendResponse>(
                "dragon/buy_gift_to_send",
                request
            )
        ).Data;

        response.Should().NotBeNull();
        response
            .ShopGiftList.Where(x => (DragonGifts)x.DragonGiftId == DragonGifts.HeartyStew)
            .First()
            .IsBuy.Should()
            .Be(false);

        response.ReturnGiftList.Should().NotBeNullOrEmpty();
        DragonReliabilityList dragonData = response
            .UpdateDataList.DragonReliabilityList!.Where(x => x.DragonId == Dragons.HighJupiter)
            .First();
        dragonData.ReliabilityTotalExp.Should().Be(1000);
        dragonData.ReliabilityLevel.Should().Be(6);
        dragonData
            .LastContactTime.Should()
            .BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task DragonSendGiftMultiple_IncreasesReliabilityAndReturnsGifts()
    {
        await this.AddToDatabase(
            DbPlayerDragonReliabilityFactory.Create(ViewerId, Dragons.HighMercury)
        );

        DragonSendGiftMultipleRequest request = new DragonSendGiftMultipleRequest()
        {
            DragonId = Dragons.HighMercury,
            DragonGiftId = DragonGifts.FourLeafClover,
            Quantity = 10
        };

        DragonSendGiftMultipleResponse? response = (
            await this.Client.PostMsgpack<DragonSendGiftMultipleResponse>(
                "dragon/send_gift_multiple",
                request
            )
        ).Data;

        response.Should().NotBeNull();
        response.ReturnGiftList.Should().NotBeNullOrEmpty();
        DragonReliabilityList dragonData = response
            .UpdateDataList.DragonReliabilityList!.Where(x => x.DragonId == Dragons.HighMercury)
            .First();
        dragonData.ReliabilityTotalExp.Should().Be(10000);
        dragonData.ReliabilityLevel.Should().Be(18);
    }

    [Fact]
    public async Task DragonSendGiftMultiple_CompletesMissionsCorrectly()
    {
        int missionId = 302600; // Reach Bond Lv. 30 with a Dragon

        await this.AddToDatabase(
            new DbPlayerMission()
            {
                Type = MissionType.Drill,
                Id = missionId,
                State = MissionState.InProgress
            }
        );

        await this.AddRangeToDatabase(
            [
                new DbPlayerDragonReliability() { DragonId = Dragons.Apollo, },
                new DbPlayerDragonReliability() { DragonId = Dragons.Kagutsuchi }
            ]
        );

        await this.Client.PostMsgpack<DragonSendGiftMultipleResponse>(
            "dragon/send_gift_multiple",
            new DragonSendGiftMultipleRequest()
            {
                DragonId = Dragons.Apollo,
                DragonGiftId = DragonGifts.FourLeafClover,
                Quantity = 2,
            }
        );

        this.ApiContext.PlayerMissions.AsNoTracking()
            .First(x => x.Id == missionId)
            .Progress.Should()
            .Be(9);

        await this.Client.PostMsgpack<DragonSendGiftMultipleResponse>(
            "dragon/send_gift_multiple",
            new DragonSendGiftMultipleRequest()
            {
                DragonId = Dragons.Kagutsuchi,
                DragonGiftId = DragonGifts.FourLeafClover,
                Quantity = 1,
            }
        );

        this.ApiContext.PlayerMissions.AsNoTracking()
            .First(x => x.Id == missionId)
            .Progress.Should()
            .Be(9, "the progress is based on the highest level reached");

        var completeMissionResponse = (
            await this.Client.PostMsgpack<DragonSendGiftMultipleResponse>(
                "dragon/send_gift_multiple",
                new DragonSendGiftMultipleRequest()
                {
                    DragonId = Dragons.Kagutsuchi,
                    DragonGiftId = DragonGifts.FourLeafClover,
                    Quantity = 200,
                }
            )
        ).Data;

        completeMissionResponse
            .UpdateDataList.MissionNotice.DrillMissionNotice.NewCompleteMissionIdList.Should()
            .Contain(missionId);

        this.ApiContext.PlayerMissions.AsNoTracking()
            .First(x => x.Id == missionId)
            .Should()
            .BeEquivalentTo(
                new DbPlayerMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId,
                    Type = MissionType.Drill,
                    Progress = 30,
                    State = MissionState.Completed
                }
            );
    }

    [Fact]
    public async Task DragonSendGift_IncreasesReliabilityAndReturnsGifts()
    {
        await this.AddToDatabase(DbPlayerDragonReliabilityFactory.Create(ViewerId, Dragons.Puppy));

        DragonSendGiftRequest request = new DragonSendGiftRequest()
        {
            DragonId = Dragons.Puppy,
            DragonGiftId = DragonGifts.PupGrub
        };

        DragonSendGiftResponse? response = (
            await this.Client.PostMsgpack<DragonSendGiftResponse>("dragon/send_gift", request)
        ).Data;

        response.Should().NotBeNull();
        DragonReliabilityList dragonData = response
            .UpdateDataList.DragonReliabilityList!.Where(x => x.DragonId == Dragons.Puppy)
            .First();
        dragonData.ReliabilityTotalExp.Should().Be(200);
        dragonData.ReliabilityLevel.Should().Be(3);
    }

    [Fact]
    public async Task DragonSendGift_AllStoriesUnlocked_DoesNotThrow()
    {
        await this.AddToDatabase(
            DbPlayerDragonReliabilityFactory.Create(ViewerId, Dragons.MidgardsormrZero)
        );

        foreach (int storyId in MasterAsset.DragonStories[(int)Dragons.MidgardsormrZero].storyIds)
        {
            await this.AddToDatabase(
                new DbPlayerStoryState()
                {
                    ViewerId = ViewerId,
                    StoryId = storyId,
                    State = StoryState.Read,
                    StoryType = StoryTypes.Dragon
                }
            );
        }

        DragonSendGiftMultipleRequest request =
            new()
            {
                DragonId = Dragons.MidgardsormrZero,
                DragonGiftId = DragonGifts.FourLeafClover,
                Quantity = 100
            };

        DragaliaResponse<DragonSendGiftMultipleResponse>? response = (
            await this.Client.PostMsgpack<DragonSendGiftMultipleResponse>(
                "dragon/send_gift_multiple",
                request
            )
        );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
    }

    public record DragonLimitBreakTestCase(
        List<DbPlayerDragonData> SetupDragons,
        byte LimitBreakNr,
        DragonLimitBreakMatTypes LbMatType
    );

    public class DragonLimitBreakTheoryData : TheoryData<DragonLimitBreakTestCase>
    {
        public DragonLimitBreakTheoryData()
        {
            #region JuggernautDupe
            Add(
                new DragonLimitBreakTestCase(
                    new List<DbPlayerDragonData>()
                    {
                        DbPlayerDragonDataFactory.Create(0, Dragons.Juggernaut),
                        DbPlayerDragonDataFactory.Create(0, Dragons.Juggernaut)
                    },
                    1,
                    DragonLimitBreakMatTypes.Dupe
                )
            );
            #endregion
            #region MidgardsormrStone
            Add(
                new DragonLimitBreakTestCase(
                    new List<DbPlayerDragonData>()
                    {
                        DbPlayerDragonDataFactory.Create(0, Dragons.Midgardsormr)
                    },
                    1,
                    DragonLimitBreakMatTypes.Stone
                )
            );
            #endregion
            #region CupidSpheres
            Add(
                new DragonLimitBreakTestCase(
                    new List<DbPlayerDragonData>()
                    {
                        DbPlayerDragonDataFactory.Create(0, Dragons.Cupid)
                    },
                    1,
                    DragonLimitBreakMatTypes.Spheres
                )
            );
            #endregion
            #region HighBrunSpheresLB5
            Add(
                new DragonLimitBreakTestCase(
                    new List<DbPlayerDragonData>()
                    {
                        DbPlayerDragonDataFactory.Create(0, Dragons.HighBrunhilda)
                    },
                    5,
                    DragonLimitBreakMatTypes.SpheresLB5
                )
            );
            #endregion
        }
    }

    [Theory]
    [ClassData(typeof(DragonLimitBreakTheoryData))]
    public async Task DragonLimitBreak_LimitBreaks(DragonLimitBreakTestCase testCase)
    {
        DbPlayerDragonData dbDragon = await this.AddToDatabase(testCase.SetupDragons[0]);

        DbPlayerDragonData dbDragonSacrifice = null!;
        if (testCase.LbMatType == DragonLimitBreakMatTypes.Dupe)
        {
            dbDragonSacrifice = await this.AddToDatabase(testCase.SetupDragons[1]);
        }

        DragonLimitBreakRequest request = new DragonLimitBreakRequest()
        {
            BaseDragonKeyId = (ulong)dbDragon.DragonKeyId,
            LimitBreakGrowList = new List<LimitBreakGrowList>()
            {
                new LimitBreakGrowList()
                {
                    LimitBreakCount = testCase.LimitBreakNr,
                    LimitBreakItemType = (int)testCase.LbMatType,
                    TargetId =
                        testCase.LbMatType == DragonLimitBreakMatTypes.Dupe
                            ? (ulong)dbDragonSacrifice!.DragonKeyId
                            : 0
                }
            }
        };

        DragonBuildupResponse? response = (
            await this.Client.PostMsgpack<DragonBuildupResponse>("dragon/limit_break", request)
        ).Data;

        response.Should().NotBeNull();
        response.UpdateDataList.Should().NotBeNull();
        response.UpdateDataList.DragonList.Should().NotBeNullOrEmpty();
        DragonList returnDragon = response
            .UpdateDataList.DragonList!.Where(x => (long)x.DragonKeyId == dbDragon.DragonKeyId)
            .First();
        returnDragon.LimitBreakCount.Should().Be(testCase.LimitBreakNr);
        if (testCase.LbMatType == DragonLimitBreakMatTypes.Dupe)
        {
            response.DeleteDataList.Should().NotBeNull();
            response.DeleteDataList.DeleteDragonList.Should().NotBeNullOrEmpty();
            response
                .DeleteDataList.DeleteDragonList.Should()
                .Contain(x => (long)x.DragonKeyId == dbDragonSacrifice!.DragonKeyId);
        }
        else
        {
            response.UpdateDataList.MaterialList.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task DragonSetLock_ReturnsLockDragonData()
    {
        DbPlayerDragonData dragon = await this.AddToDatabase(
            DbPlayerDragonDataFactory.Create(ViewerId, Dragons.HighZodiark)
        );

        DragonSetLockRequest request = new DragonSetLockRequest()
        {
            DragonKeyId = (ulong)dragon.DragonKeyId,
            IsLock = true
        };

        DragonSetLockResponse? response = (
            await this.Client.PostMsgpack<DragonSetLockResponse>("dragon/set_lock", request)
        ).Data;

        response.Should().NotBeNull();
        response.UpdateDataList.Should().NotBeNull();
        response.UpdateDataList.DragonList.Should().NotBeNullOrEmpty();
        DragonList returnDragon = response
            .UpdateDataList.DragonList!.Where(x => (long)x.DragonKeyId == dragon.DragonKeyId)
            .First();
        returnDragon.IsLock.Should().BeTrue();
    }

    [Fact]
    public async Task DragonSell_SuccessfulSale()
    {
        DbPlayerDragonData dragon = await this.AddToDatabase(
            DbPlayerDragonDataFactory.Create(ViewerId, Dragons.GaibhneCreidhne)
        );

        DragonData dragonData = MasterAsset.DragonData.Get(Dragons.GaibhneCreidhne);

        DbPlayerUserData uData = await this
            .ApiContext.PlayerUserData.AsNoTracking()
            .Where(x => x.ViewerId == ViewerId)
            .FirstAsync();

        long startCoin = uData.Coin;
        long startDew = uData.DewPoint;

        DragonSellRequest request = new DragonSellRequest()
        {
            DragonKeyIdList = new List<ulong>() { (ulong)dragon.DragonKeyId }
        };

        DragonSellResponse? response = (
            await this.Client.PostMsgpack<DragonSellResponse>("dragon/sell", request)
        ).Data;

        response.Should().NotBeNull();
        response.DeleteDataList.Should().NotBeNull();
        response.DeleteDataList.DeleteDragonList.Should().NotBeNullOrEmpty();
        response.UpdateDataList.UserData.Coin.Should().Be(startCoin + dragonData.SellCoin);
        response
            .UpdateDataList.UserData.DewPoint.Should()
            .Be((int)startDew + dragonData.SellDewPoint);
    }

    [Fact]
    public async Task DragonSell_Multi_SuccessfulSale()
    {
        ApiContext context = this.Services.GetRequiredService<ApiContext>();

        DbPlayerDragonData dragonSimurgh = context
            .PlayerDragonData.Add(DbPlayerDragonDataFactory.Create(ViewerId, Dragons.Simurgh))
            .Entity;

        DbPlayerDragonData dragonStribog = context
            .PlayerDragonData.Add(DbPlayerDragonDataFactory.Create(ViewerId, Dragons.Stribog))
            .Entity;

        dragonStribog.LimitBreakCount = 4;

        await context.SaveChangesAsync();
        DragonData dragonDataSimurgh = MasterAsset.DragonData.Get(Dragons.Simurgh);
        DragonData dragonDataStribog = MasterAsset.DragonData.Get(Dragons.Stribog);

        context.ChangeTracker.Clear();
        DbPlayerUserData uData = await context
            .PlayerUserData.Where(x => x.ViewerId == ViewerId)
            .FirstAsync();

        long startCoin = uData.Coin;
        long startDew = uData.DewPoint;

        DragonSellRequest request = new DragonSellRequest()
        {
            DragonKeyIdList = new List<ulong>()
            {
                (ulong)dragonSimurgh.DragonKeyId,
                (ulong)dragonStribog.DragonKeyId
            }
        };

        DragonSellResponse? response = (
            await this.Client.PostMsgpack<DragonSellResponse>("dragon/sell", request)
        ).Data;

        response.Should().NotBeNull();
        response.DeleteDataList.Should().NotBeNull();
        response.DeleteDataList.DeleteDragonList.Should().NotBeNullOrEmpty();
        response
            .UpdateDataList.UserData.Coin.Should()
            .Be(startCoin + dragonDataSimurgh.SellCoin + (dragonDataStribog.SellCoin * 5));
        response
            .UpdateDataList.UserData.DewPoint.Should()
            .Be(
                (int)startDew
                    + dragonDataSimurgh.SellDewPoint
                    + (dragonDataStribog.SellDewPoint * 5)
            );
    }
}
